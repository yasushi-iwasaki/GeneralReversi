#define ARRAY_SORT
//#define LINQ_SORT

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace GeneralReversi
{
    using Position = Int32;
    using Vector = Int32;

    public enum State : byte
    {
        Outer,
        Empty,
        Black,
        White
    }

    public enum Pattern : byte
    {
        Cross,
        Parallel
    }

    public static class PatternState
    {
        public static State[,] array = {
                               {State.White, State.Black, State.Black, State.White},
                               {State.Black, State.White, State.Black, State.White}};
    }

    public static class Extension
    {
        public static State Not(this State self)
        {
            switch (self)
            {
                case State.Black:
                    return State.White;
                case State.White:
                    return State.Black;
                default:
                    return self;
            }
        }

        public static int Sign(this State self)
        {
            switch (self)
            {
                case State.Black:
                    return 1;
                case State.White:
                    return -1;
                default:
                    return 0;
            }
        }

        public static string ToStringExtension<T>(this IEnumerable<T> self)
        {
            string ret = "{ ";

            foreach (T t in self)
            {
                ret += t + " ";
            }

            ret += "}";

            return ret;
        }

        public static IEnumerable<int> Ordering<T>(this IEnumerable<T> self, int length)
        {
            return self
            .Zip(Enumerable.Range(0, length), (value, index) => new { Value = value, Index = index })
            .OrderBy(element => element.Value)
            .Select(element => element.Index);
        }

        public static IEnumerable<int> Ordering<T>(this IEnumerable<T> self, Random random, int length)
        {
            return self
            .Zip(Enumerable.Range(0, length), (value, index) => new { Value = value, Index = index })
            .OrderBy(element => element.Value)
            .ThenBy(element => random.Next())
            .Select(element => element.Index);
        }

        public static IEnumerable<int> OrderingDescending<T>(this IEnumerable<T> self, int length)
        {
            return self
            .Zip(Enumerable.Range(0, length), (value, index) => new { Value = value, Index = index })
            .OrderByDescending(element => element.Value)
            .Select(element => element.Index);
        }

        public static IEnumerable<int> OrderingDescending<T>(this IEnumerable<T> self, Random random, int length)
        {
            return self
            .Zip(Enumerable.Range(0, length), (value, index) => new { Value = value, Index = index })
            .OrderByDescending(element => element.Value)
            .ThenBy(element => random.Next())
            .Select(element => element.Index);
        }
    }

    public class LinkedPosition
    {
        public LinkedPosition previous;
        public LinkedPosition next;
        public Position position;
        public Vector[] directions;
        public bool corner;
        public static LinkedPosition Out = new LinkedPosition(Board.Out, null, false);

        public LinkedPosition(Position position, Vector[] directions, bool corner)
        {
            this.position = position;
            this.directions = directions;
            this.corner = corner;
        }
    }
    
    public class LinkedPositions
    {
        public LinkedPosition first;
        public LinkedPosition last;
        public int count;

        public LinkedPositions()
        {
        }

        public LinkedPositions(LinkedPositions linkedPositions)
        {
            for (LinkedPosition linkedPosition = linkedPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
            {
                this.AddLast(new LinkedPosition(linkedPosition.position, linkedPosition.directions, linkedPosition.corner));
            }
        }

        public void AddFirst(LinkedPosition linkedPosition)
        {
            count++;

            if (first == null)
            {
                first = linkedPosition;

                return;
            }

            first.previous = linkedPosition;
            linkedPosition.next = first;
            first = linkedPosition;
        }

        public void AddLast(LinkedPosition linkedPosition)
        {
            count++;

            if (first == null)
            {
                first = last = linkedPosition;

                return;
            }

            last.next = linkedPosition;
            linkedPosition.previous = last;
            last = linkedPosition;
        }

        public void Remove(LinkedPosition linkedPosition)
        {
            count--;

            if (linkedPosition.previous == null)
            {
                first = linkedPosition.next;
            }
            else
            {
                linkedPosition.previous.next = linkedPosition.next;
            }

            if (linkedPosition.next == null)
            {
                last = linkedPosition.previous;
            }
            else
            {
                linkedPosition.next.previous = linkedPosition.previous;
            }
        }

        public void Unremove(LinkedPosition linkedPosition)
        {
            count++;

            if (linkedPosition.previous == null)
            {
                first = linkedPosition;
            }
            else
            {
                linkedPosition.previous.next = linkedPosition;
            }

            if (linkedPosition.next == null)
            {
                last = linkedPosition;
            }
            else
            {
                linkedPosition.next.previous = linkedPosition;
            }
        }
    }

    public class MoveInformation
    {
        public State turn;
        public LinkedPosition linkedPosition;
        public Position[] reversedPositions;

        public MoveInformation(State turn, LinkedPosition linkedPosition, Position[] reversedPositions)
        {
            this.turn = turn;
            this.linkedPosition = linkedPosition;
            this.reversedPositions = reversedPositions;
        }
    }

    public class Board : ICloneable
    {
        public static int size;
        public State[] array;
        public int countBlack;
        public int countWhite;
        public int score;
        public LinkedPositions emptyPositions;
        public static Vector[,][] directions = new Vector[3,3][];
        public static Position[] corners;
        public int index;
        public MoveInformation[] moveInformations;
        public Position[] reversedPositions;
        public int reversedPositionsCount;
        public const Position Out = -1;

        public override bool Equals(object o)
        {
            Board b = (Board)o;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Position position = Position(i, j);

                    if (array[position] != b.array[position])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int GetHashCodeSub(int x, int y)
        {
            int ret = 0;
            int ctr = 0;
            const int prime = 31;
            int[] pow3 = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907 };

            for (int i = x; i < x + 4; i++)
            {
                for (int j = y; j < y + 4; j++)
                {
                    ret = ret * prime + (int)array[Position(i, j)] * pow3[ctr++];
                }
            }

            return ret;
        }

        public override int GetHashCode()
        {
            int ret = 0;
            int[] array;

            if (size <= 8)
            {
                array = new int[] { 0, size - 4 };
            }
            else if (size <= 12)
            {
                array = new int[] { 0, (size - 4) / 2, size - 4 };
            }
            else
            {
                array = new int[] { 0, (size - 4) / 4, (size - 4) / 2, 3 * (size - 4) / 4, size - 4 };
            }

            foreach (int i in array)
            {
                foreach (int j in array)
                {
                    ret += GetHashCodeSub(i, j);
                }
            }

            return ret;
        }

        public Board CloneArray()
        {
            return new Board(array);
        }

        public Board Clone()
        {
            return new Board(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public static Position Position(int x, int y)
        {
            return (size + 1) * (x + 1) + (y + 1);
        }

        public static int PositionX(Position position)
        {
            return position / (size + 1) - 1;
        }

        public static int PositionY(Position position)
        {
            return position % (size + 1) - 1;
        }

        public static string PositionString(Position position)
        {
            int x = PositionX(position);
            int y = PositionY(position);

            return "(" + x + ", " + y + ")";
        }

        public static Vector Vector(int x, int y)
        {
            return (size + 1) * x + y;
        }

        public static int VectorX(Vector vector)
        {
            return vector / (size + 1);
        }

        public static int VectorY(Vector vector)
        {
            return vector % (size + 1);
        }

        public static string VectorString(Vector vector)
        {
            int x = VectorX(vector);
            int y = VectorY(vector);

            return "(" + x + ", " + y + ")";
        }

        private static Vector[,][] Directions()
        {
            Vector[] directions0 = new Vector[8];
            int ctr = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    directions0[ctr++] = Vector(i, j);
                }
            }

            Vector[,][] ret = new Vector[3, 3][];
            ret[0, 0] = new Vector[] { directions0[4], directions0[6], directions0[7] };
            ret[0, 1] = new Vector[] { directions0[3], directions0[4], directions0[5], directions0[6], directions0[7] };
            ret[0, 2] = new Vector[] { directions0[3], directions0[5], directions0[6] };
            ret[1, 0] = new Vector[] { directions0[1], directions0[2], directions0[4], directions0[6], directions0[7] };
            ret[1, 1] = directions0;
            ret[1, 2] = new Vector[] { directions0[0], directions0[1], directions0[3], directions0[5], directions0[6] };
            ret[2, 0] = new Vector[] { directions0[1], directions0[2], directions0[4] };
            ret[2, 1] = new Vector[] { directions0[0], directions0[1], directions0[2], directions0[3], directions0[4] };
            ret[2, 2] = new Vector[] { directions0[0], directions0[1], directions0[3] };

            return ret;
        }

        public Board(State[] array)
        {
            this.array = (State[])array.Clone();
        }

        public Board(Board board)
        {
            this.array = (State[])board.array.Clone();
            this.countBlack = board.countBlack;
            this.countWhite = board.countWhite;
            this.score = board.score;
            this.emptyPositions = new LinkedPositions(board.emptyPositions);
            this.index = board.index;
            this.moveInformations = (MoveInformation[])board.moveInformations.Clone();
            reversedPositions = new Position[4 * size];
            reversedPositionsCount = 0;
        }

        public Board(int size, Pattern pattern)
        {
            Board.size = size;
            directions = Directions();
            corners = Corners();
            moveInformations = new MoveInformation[size * size - 4];
            reversedPositions = new Position[4 * size];
            reversedPositionsCount = 0;

            array = new State[(size + 1) * (size + 2) + 1];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    array[Position(i, j)] = State.Empty;
                }
            }

            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    int x = size / 2 - 1 + i;
                    int y = size / 2 - 1 + j;

                    array[Position(x, y)] = PatternState.array[(int)pattern, 2 * i + j];
                }
            }

            countBlack = 2;
            countWhite = 2;
            score = 0;

            emptyPositions = new LinkedPositions();
            int c0 = size / 2 - 1;
            int c1 = size / 2;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i == c0 || i == c1) && (j == c0 || j == c1))
                    {
                        continue;
                    }

                    Position position = Position(i, j);
                    int x = PositionX(position);
                    int y = PositionY(position);

                    int k;
                    int l;

                    if (0 <= x && x <= 1)
                    {
                        k = 0;
                    }
                    else if (2 <= x && x <= size - 3)
                    {
                        k = 1;
                    }
                    else
                    {
                        k = 2;
                    }

                    if (0 <= y && y <= 1)
                    {
                        l = 0;
                    }
                    else if (2 <= y && y <= size - 3)
                    {
                        l = 1;
                    }
                    else
                    {
                        l = 2;
                    }

                    emptyPositions.AddLast(new LinkedPosition(position, directions[k, l], corners.Contains(position)));
                }
            }
        }

        public override string ToString()
        {
            const string empty = "-";
            const string black = "X";
            const string white = "O";

            string ret = "";

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    switch (array[Position(i, j)])
                    {
                        case State.Empty:
                            ret += empty;

                            break;
                        case State.Black:
                            ret += black;

                            break;
                        case State.White:
                            ret += white;

                            break;
                        default:
                            break;
                    }

                    ret += " ";
                }

                ret += "\n";
            }

            return ret;
        }

        unsafe public int CountReverse(State turn, LinkedPosition linkedPosition)
        {
            int ret = 0;
            State notTurn = turn.Not();

            fixed (State* pointerArray = &array[linkedPosition.position])
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    State* pointer = pointerArray + direction;

                    while (*pointer == notTurn)
                    {
                        pointer += direction;
                    }

                    if (*pointer == turn)
                    {
                        for (pointer -= direction; *pointer == notTurn; pointer -= direction)
                        {
                            ret++;
                        }
                    }
                }
            }

            return ret;
        }

        unsafe public Board Move(State turn, LinkedPosition linkedPosition)
        {
            array[linkedPosition.position] = turn;

            if (turn == State.Black)
            {
                countBlack++;
            }
            else
            {
                countWhite++;
            }

            emptyPositions.Remove(linkedPosition);

            State notTurn = turn.Not();

            fixed (State* pointerArray = &array[linkedPosition.position])
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    State* pointer = pointerArray + direction;

                    while (*pointer == notTurn)
                    {
                        pointer += direction;
                    }

                    if (*pointer == turn)
                    {
                        for (pointer -= direction; *pointer == notTurn; pointer -= direction)
                        {
                            *pointer = turn;
                            reversedPositions[reversedPositionsCount++] = linkedPosition.position + (Vector)(pointer - pointerArray);
                        }
                    }
                }
            }

            if (turn == State.Black)
            {
                countBlack += reversedPositionsCount;
                countWhite -= reversedPositionsCount;
            }
            else
            {
                countWhite += reversedPositionsCount;
                countBlack -= reversedPositionsCount;
            }

            score = countBlack - countWhite;

            Position[] reversedPositionsCopy = new Position[reversedPositionsCount];
            //Array.Copy(reversedPositions, reversedPositionsCopy, reversedPositionsCount);
            Buffer.BlockCopy(reversedPositions, 0, reversedPositionsCopy, 0, reversedPositionsCount * sizeof(Position));

            moveInformations[index++] = new MoveInformation(turn, linkedPosition, reversedPositionsCopy);
            reversedPositionsCount = 0;

            return this;
        }

        public Board Unmove()
        {
            index--;
            State turn = moveInformations[index].turn;
            LinkedPosition linkedPosition = moveInformations[index].linkedPosition;
            Position[] reversedPositions = moveInformations[index].reversedPositions;

            array[linkedPosition.position] = State.Empty;

            if (turn == State.Black)
            {
                countBlack--;
            }
            else
            {
                countWhite--;
            }

            emptyPositions.Unremove(linkedPosition);

            State notTurn = turn.Not();

            foreach (Position reversedPosition in reversedPositions)
            {
                array[reversedPosition] = notTurn;
            }

            if (turn == State.Black)
            {
                countBlack -= reversedPositions.Length;
                countWhite += reversedPositions.Length;
            }
            else
            {
                countWhite -= reversedPositions.Length;
                countBlack += reversedPositions.Length;
            }

            score = countBlack - countWhite;

            return this;
        }

        unsafe public LinkedPosition[] MovablePositions(State turn)
        {
            State notTurn = turn.Not();
            LinkedPosition[] movablePositions = new LinkedPosition[emptyPositions.count];
            int movablePositionsCount = 0;

            for (LinkedPosition linkedPosition = emptyPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
            {
                fixed (State* pointerArray = &array[linkedPosition.position])
                {
                    foreach (Vector direction in linkedPosition.directions)
                    {
                        State* pointer0 = pointerArray + direction;
                        State* pointer = pointer0;

                        while (*pointer == notTurn)
                        {
                            pointer += direction;
                        }

                        if (pointer != pointer0 && *pointer == turn)
                        {
                            movablePositions[movablePositionsCount++] = linkedPosition;

                            break;
                        }
                    }
                }
            }

            LinkedPosition[] movablePositionsCopy = new LinkedPosition[movablePositionsCount];
            Array.Copy(movablePositions, movablePositionsCopy, movablePositionsCount);

            return movablePositionsCopy;
        }

        unsafe public int Movable(State turn)
        {
            State notTurn = turn.Not();
            int ret = 0;

            for (LinkedPosition linkedPosition = emptyPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
            {
                fixed (State* pointerArray = &array[linkedPosition.position])
                {
                    foreach (Vector direction in linkedPosition.directions)
                    {
                        State* pointer0 = pointerArray + direction;
                        State* pointer = pointer0;

                        while (*pointer == notTurn)
                        {
                            pointer += direction;
                        }

                        if (pointer != pointer0 && *pointer == turn)
                        {
                            ret++;

                            break;
                        }
                    }
                }
            }

            return ret;
        }

        unsafe public int WeightedMovable(State turn)
        {
            State notTurn = turn.Not();
            int ret = 0;

            for (LinkedPosition linkedPosition = emptyPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
            {
                fixed (State* pointerArray = &array[linkedPosition.position])
                {
                    foreach (Vector direction in linkedPosition.directions)
                    {
                        State* pointer0 = pointerArray + direction;
                        State* pointer = pointer0;

                        while (*pointer == notTurn)
                        {
                            pointer += direction;
                        }

                        if (pointer != pointer0 && *pointer == turn)
                        {
                            if (linkedPosition.corner)
                            {
                                ret += 2;
                            }
                            else
                            {
                                ret++;
                            }

                            break;
                        }
                    }
                }
            }

            return ret;
        }

        unsafe public bool CanMove(State turn)
        {
            State notTurn = turn.Not();

            for (LinkedPosition linkedPosition = emptyPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
            {
                fixed (State* pointerArray = &array[linkedPosition.position])
                {
                    foreach (Vector direction in linkedPosition.directions)
                    {
                        State* pointer0 = pointerArray + direction;
                        State* pointer = pointer0;

                        while (*pointer == notTurn)
                        {
                            pointer += direction;
                        }

                        if (pointer != pointer0 && *pointer == turn)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Position[] Corners()
        {
            return new Position[] { Position(0, 0), Position(0, size - 1), Position(size - 1, 0), Position(size - 1, size - 1) };
        }
    }

    public abstract class Player
    {
        public abstract LinkedPosition Choose(Board board, State turn);
    }

    public class HumanPlayer : Player
    {
        public override LinkedPosition Choose(Board board, State turn)
        {
            while (true)
            {
                Position position;

                try
                {
                    Console.Write("X: ");
                    int x = int.Parse(Console.ReadLine());

                    Console.Write("Y: ");
                    int y = int.Parse(Console.ReadLine());

                    position = Board.Position(x, y);
                }
                catch (FormatException fe)
                {
                    Console.WriteLine(fe);

                    continue;
                }

                LinkedPosition ret = Array.Find(Exe.movablePositions, linkedPosition => linkedPosition.position == position);

                if (ret != null)
                {
                    return ret;
                }
                else
                {
                    Console.WriteLine("Unmovable");
                }
            }
        }
    }

    public class RandomPlayer : Player
    {
        public override LinkedPosition Choose(Board board, State turn)
        {
            int index = Exe.random.Next(Exe.movablePositions.Length);

            return Exe.movablePositions[index];
        }
    }

    public class MonteCarloTreeNode
    {
        public LinkedPosition linkedPosition;
        public State turn;
        public int search;
        public double reward;
        public double ucb1;
        public MonteCarloTreeNode parent;
        public MonteCarloTreeNode[] children;

        public MonteCarloTreeNode(LinkedPosition linkedPosition, State turn)
        {
            this.linkedPosition = linkedPosition;
            this.turn = turn;
            this.search = 0;
            this.reward = 0;
            this.ucb1 = double.NaN;
            this.parent = null;
            this.children = null;
        }

        public void SetChildren(Board board)
        {
            LinkedPosition[] movablePositions = board.MovablePositions(turn);
            MonteCarloTreeNode[] children;
            State notTurn = turn.Not();

            if (movablePositions.Length == 0)
            {
                if (!board.CanMove(notTurn))
                {
                    children = null;
                }
                else
                {
                    children = new MonteCarloTreeNode[1];
                    children[0] = new MonteCarloTreeNode(LinkedPosition.Out, notTurn);
                    children[0].parent = this;
                }
            }
            else
            {
                children = new MonteCarloTreeNode[movablePositions.Length];

                for (int i = 0; i < movablePositions.Length; i++)
                {
                    children[i] = new MonteCarloTreeNode(movablePositions[i], notTurn);
                    children[i].parent = this;
                }
            }

            this.children = children;
        }
    }

    public class MonteCarloTreeSearchPlayer : Player
    {
        protected LinkedPosition[] movablePositions;
        protected int iteration;
        protected static Random random;

        public virtual void Set()
        {
            movablePositions = Exe.movablePositions;
            iteration = Exe.iteration;
            random = Exe.random;
        }

        public virtual void InitializeProgress()
        {
        }

        public virtual void SetProgress(int value)
        {
        }

        public virtual void WriteLog(MonteCarloTreeNode root)
        {
        }
        
        static double PlayOut(Board board, State turn)
        {
            Board b = board.Clone();
            State t = turn;
            LinkedPosition linkedPosition;
            LinkedPosition[] movablePositions;

            while (true)
            {
                State notT = t.Not();

                if (b.CanMove(t))
                {
                    movablePositions = b.MovablePositions(t);
                    linkedPosition = movablePositions[random.Next(movablePositions.Length)];
                    b.Move(t, linkedPosition);
                }
                else if (!b.CanMove(notT))
                {
                    break;
                }

                t = notT;
            }

            int sign = Math.Sign(b.score);

            switch (sign)
            {
                case 1:
                    return 1;
                case -1:
                    return 0;
                default:
                    return 0.5;
            }
        }

        double Ucb1(int total, int test, double win)
        {
            if (test == 0)
            {
                return double.PositiveInfinity;
            }

            return win / test + Math.Sqrt(2 * Math.Log(total / test));
        }

        public override LinkedPosition Choose(Board board, State turn)
        {
            const int threshold = 1;
            MonteCarloTreeNode root = new MonteCarloTreeNode(LinkedPosition.Out, turn);
            MonteCarloTreeNode node = root;
            Board b = board.Clone();
            root.SetChildren(b);
            Stack<bool> stack = new Stack<bool>();

            Set();
            InitializeProgress();

            while (root.search < iteration)
            {
                if (node.children == null)
                {
                    double reward = PlayOut(b, node.turn);

                    while (node != root)
                    {
                        node.search++;

                        if (node.search == threshold)
                        {
                            node.SetChildren(b);
                        }

                        node.reward += reward;

                        node = node.parent;

                        if (stack.Pop())
                        {
                            b.Unmove();
                        }
                    }

                    root.search++;
                }
                else
                {
                    MonteCarloTreeNode nodeMax = null;
                    double ucb1Max = double.NegativeInfinity;

                    foreach (MonteCarloTreeNode child in node.children)
                    {
                        if (node.turn == State.Black)
                        {
                            child.ucb1 = Ucb1(node.search, child.search, child.reward);
                        }
                        else
                        {
                            child.ucb1 = Ucb1(node.search, child.search, child.search - child.reward);
                        }

                        if (child.ucb1 > ucb1Max)
                        {
                            ucb1Max = child.ucb1;
                            nodeMax = child;
                        }
                    }

                    node = nodeMax;

                    if (node.linkedPosition != LinkedPosition.Out)
                    {
                        b.Move(node.turn.Not(), node.linkedPosition);
                        stack.Push(true);
                    }
                    else
                    {
                        stack.Push(false);
                    }
                }

                SetProgress(root.search);
            }

            int index = -1;
            int searchMax = int.MinValue;

            for (int i = 0; i < movablePositions.Length; i++)
            {
                if (root.children[i].search > searchMax)
                {
                    searchMax = root.children[i].search;
                    index = i;
                }
            }

            WriteLog(root);

            return movablePositions[index];
        }
    }

    public delegate int EvaluationFunction(Board board, State turn);

    public static class EvaluationFunctions
    {
        public static int Score(Board board, State turn)
        {
            return board.score;
        }

        public static int Movable(Board board, State turn)
        {
            return board.Movable(State.Black) - board.Movable(State.White);
        }

        public static int Movable2(Board board, State turn)
        {
            return turn.Sign() * board.Movable(turn);
        }

        public static int Movable3(Board board, State turn)
        {
            return turn.Sign() * board.WeightedMovable(turn);
        }

        public static int Corner(Board board, State turn)
        {
            const int scoreCorner = 1000;
            State[] states = Board.corners.Select(position => board.array[position]).ToArray();
            int countBlack = states.Count(state => state == State.Black);
            int countWhite = states.Count(state => state == State.White);

            return (countBlack - countWhite) * scoreCorner + Movable(board, turn);
        }

        public static int Stable(Board board, State turn)
        {
            const int scorePerfect = int.MaxValue - 1;
            const int scoreCorner = 1000000;
            const int scoreEdge = 1000;
            const int scoreDiagonal = 1000;
            const int penaltyCornerNeighborEdge = -100000;
            const int penaltyCornerNeighborDiagonal = -100000;
            int ret = 0;
            int countDiagonal;

            if (board.countBlack == 0)
            {
                return -scorePerfect;
            }

            if (board.countWhite == 0)
            {
                return scorePerfect;
            }

            foreach (Position corner in Board.corners)
            {
                Func<int, int> transform1 = i => i == 0 ? 1 : -1;
                Vector[] directions = { Board.Vector(transform1(Board.PositionX(corner)), 0), Board.Vector(0, transform1(Board.PositionY(corner))) };

                State stateCorner = board.array[corner];

                if (stateCorner == State.Empty)
                {
                    Position p;

                    foreach (Vector direction in directions)
                    {
                        p = corner + direction;

                        ret += board.array[p].Sign() * penaltyCornerNeighborEdge;
                    }

                    p = corner + directions[0] + directions[1];

                    ret += board.array[p].Sign() * penaltyCornerNeighborDiagonal;

                    continue;
                }

                bool[,] condition1 = new bool[Board.size, Board.size];
                bool[,] condition2 = new bool[Board.size, Board.size];

                condition1[Board.PositionX(corner), Board.PositionY(corner)] = condition2[Board.PositionX(corner), Board.PositionY(corner)] = true;

                ret += stateCorner.Sign() * scoreCorner;

                foreach (Vector direction in directions)
                {
                    for (Position p = corner + direction; board.array[p] == stateCorner; p += direction)
                    {
                        condition1[Board.PositionX(p), Board.PositionY(p)] = condition2[Board.PositionX(p), Board.PositionY(p)] = true;

                        ret += stateCorner.Sign() * scoreEdge;
                    }
                }

                int signX = transform1(Board.PositionX(corner));
                int signY = transform1(Board.PositionY(corner));

                Func<int, int> transform2 = i => i >= 0 ? i : (Board.size - 1) + i;

                for (int c = 2; c < Board.size; c++)
                {
                    int directionEdge1X = 0;
                    int directionEdge1Y = -1;

                    int directionEdge2X = -1;
                    int directionEdge2Y = 0;

                    int directionDiagonal1X = -1;
                    int directionDiagonal1Y = 1;

                    int directionDiagonal2X = 1;
                    int directionDiagonal2Y = -1;

                    for (int i = 1; i < c; i++)
                    {
                        int p1X = transform2(signX * i);
                        int p1Y = transform2(signY * (c - i));

                        int p2X = transform2(signX * (c - i));
                        int p2Y = transform2(signY * i);

                        int sEdge1X = p1X + signX * directionEdge1X;
                        int sEdge1Y = p1Y + signY * directionEdge1Y;

                        int sEdge2X = p2X + signX * directionEdge2X;
                        int sEdge2Y = p2Y + signY * directionEdge2Y;

                        int sDiagonal1X = p1X + signX * directionDiagonal1X;
                        int sDiagonal1Y = p1Y + signY * directionDiagonal1Y;

                        int sDiagonal2X = p2X + signX * directionDiagonal2X;
                        int sDiagonal2Y = p2Y + signY * directionDiagonal2Y;

                        condition1[p1X, p1Y] = condition1[sEdge1X, sEdge1Y] && condition1[sDiagonal1X, sDiagonal1Y] && board.array[Board.Position(p1X, p1Y)] == stateCorner;
                        condition2[p2X, p2Y] = condition2[sEdge2X, sEdge2Y] && condition2[sDiagonal2X, sDiagonal2Y] && board.array[Board.Position(p2X, p2Y)] == stateCorner;
                    }

                    countDiagonal = 0;

                    for (int i = 1; i < c; i++)
                    {
                        int pX = transform2(signX * i);
                        int pY = transform2(signY * (c - i));

                        if (condition1[pX, pY] || condition2[pX, pY])
                        {
                            countDiagonal++;
                        }
                    }

                    if (countDiagonal == 0)
                    {
                        break;
                    }

                    ret += stateCorner.Sign() * scoreDiagonal * countDiagonal;
                }
            }

            ret += Movable(board, turn);

            return ret;
        }
    }

    public class AlphaBetaInformation
    {
        public int value;
        public LinkedPosition linkedPosition;

        public AlphaBetaInformation()
        {
            value = 0;
            linkedPosition = LinkedPosition.Out;
        }

        public AlphaBetaInformation(int value, LinkedPosition linkedPosition)
        {
            this.value = value;
            this.linkedPosition = linkedPosition;
        }

        public AlphaBetaInformation Set(int value, LinkedPosition linkedPosition)
        {
            this.value = value;
            this.linkedPosition = linkedPosition;

            return this;
        }
    }

    public class DescendingComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return -x.CompareTo(y);
        }
    }

    public class AlphaBetaPlayer : Player
    {
        public ulong countEvaluation;
        public int ctr;
        public int emptyPositionsCount;
        public bool mtdf = false;
        public bool iterativeDeepening = false;
        public bool solver = false;
        protected int[][] value;
        protected int[][] order;
        protected static DescendingComparer descendingComparer = new DescendingComparer();
        protected EvaluationFunction evaluationFunction;
        protected int depth;
        protected int orderThreshold;
        protected bool doRandom;
        protected Random random;

        public virtual void Set()
        {
            evaluationFunction = Exe.evaluationFunction;
            depth = Exe.alphaBetaDepth;
            orderThreshold = Exe.alphaBetaOrderThreshold;
            doRandom = Exe.alphaBetaRandom;
            random = Exe.random;
        }

        public virtual void InitializeProgress()
        {
        }

        public virtual void WriteLog()
        {
        }

        public virtual void WriteLogAndSetProgress(int order, int value)
        {
        }

        protected static void Shuffle<T>(T[] value, T[] order, int length, Random random)
        {
            int n = length;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T tmp;

                tmp = value[k];
                value[k] = value[n];
                value[n] = tmp;

                tmp = order[k];
                order[k] = order[n];
                order[n] = tmp;
            }
        }

        private AlphaBetaInformation Last1(Board board, State turn, State notTurn)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            int currentScore = turn.Sign() * board.score;
            LinkedPosition linkedPosition = board.emptyPositions.first;
            int countReverse = board.CountReverse(turn, linkedPosition);

            countEvaluation++;

            if (countReverse > 0)
            {
                return ret.Set(currentScore + 2 * countReverse + 1, linkedPosition);
            }

            countReverse = board.CountReverse(notTurn, linkedPosition);

            if (countReverse > 0)
            {
                return ret.Set(currentScore - 2 * countReverse - 1, LinkedPosition.Out);
            }

            return ret.Set(currentScore, LinkedPosition.Out);
        }

        private AlphaBetaInformation Last2(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[2];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last1(board, notTurn, turn).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last2(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last3(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[3];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last2(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last3(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last4(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[4];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last3(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last4(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last5(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[5];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last4(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last5(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last6(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[6];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;
            linkedPositions[5] = linkedPositions[4].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last5(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last6(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last7(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[7];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;
            linkedPositions[5] = linkedPositions[4].next;
            linkedPositions[6] = linkedPositions[5].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last6(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last7(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last8(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[8];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;
            linkedPositions[5] = linkedPositions[4].next;
            linkedPositions[6] = linkedPositions[5].next;
            linkedPositions[7] = linkedPositions[6].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last7(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last8(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last9(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[9];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;
            linkedPositions[5] = linkedPositions[4].next;
            linkedPositions[6] = linkedPositions[5].next;
            linkedPositions[7] = linkedPositions[6].next;
            linkedPositions[8] = linkedPositions[7].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last8(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last9(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        private AlphaBetaInformation Last10(Board board, State turn, State notTurn, int alpha, int beta)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            int value;
            int max = -int.MaxValue;
            LinkedPosition[] linkedPositions = new LinkedPosition[10];
            linkedPositions[0] = board.emptyPositions.first;
            linkedPositions[1] = linkedPositions[0].next;
            linkedPositions[2] = linkedPositions[1].next;
            linkedPositions[3] = linkedPositions[2].next;
            linkedPositions[4] = linkedPositions[3].next;
            linkedPositions[5] = linkedPositions[4].next;
            linkedPositions[6] = linkedPositions[5].next;
            linkedPositions[7] = linkedPositions[6].next;
            linkedPositions[8] = linkedPositions[7].next;
            linkedPositions[9] = linkedPositions[8].next;

            foreach (LinkedPosition linkedPosition in linkedPositions)
            {
                foreach (Vector direction in linkedPosition.directions)
                {
                    Position p;
                    Position p0;

                    for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                    if (p != p0 && board.array[p] == turn)
                    {
                        LinkedPosition movablePosition = linkedPosition;

                        board.Move(turn, movablePosition);
                        value = -Last9(board, notTurn, turn, -beta, -alpha).value;
                        board.Unmove();

                        if (value > max)
                        {
                            max = value;
                            ret.Set(max, movablePosition);
                            alpha = Math.Max(alpha, max);

                            if (max >= beta)
                            {
                                return ret;
                            }
                        }

                        if (condition)
                        {
                            WriteLogAndSetProgress(ctr, value);
                        }

                        break;
                    }
                }
            }

            if (max == -int.MaxValue)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    return ret.Set(turn.Sign() * board.score, LinkedPosition.Out);
                }

                return ret.Set(-Last10(board, notTurn, turn, -beta, -alpha).value, LinkedPosition.Out);
            }

            return ret;
        }

        public AlphaBetaInformation AlphaBeta(Board board, State turn, int depth, int alpha, int beta, EvaluationFunction evaluationFunction, int orderThreshold, bool doRandom)
        {
            State notTurn = turn.Not();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            EvaluationFunction orderEvaluationFunction = new EvaluationFunction(EvaluationFunctions.Movable3);

            if (depth == 0)
            {
                countEvaluation++;

                return new AlphaBetaInformation(turn.Sign() * evaluationFunction(board, turn), LinkedPosition.Out);
            }

            if (depth > orderThreshold)
            {
                LinkedPosition[] movablePositions = board.MovablePositions(turn);

                for (int i = 0; i < movablePositions.Length; i++)
                {
                    board.Move(turn, movablePositions[i]);
                    value[depth][i] = orderEvaluationFunction(board, notTurn);
                    board.Unmove();
                }

                if (doRandom)
                {
#if ARRAY_SORT
                    for (int i = 0; i < movablePositions.Length; i++)
                    {
                        order[depth][i] = i;
                    }

                    Shuffle(value[depth], order[depth], movablePositions.Length, random);

                    if (turn == State.Black)
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length, descendingComparer);
                    }
                    else
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length);
                    }
#elif LINQ_SORT
                    int[] orderSource = turn == State.Black ? value[depth].OrderingDescending(random, movablePositions.Length).ToArray() : value[depth].Ordering(random, movablePositions.Length).ToArray();
                    Array.Copy(orderSource, order[depth], movablePositions.Length);
                    //Buffer.BlockCopy(orderSource, 0, order[depth], 0, movablePositions.Length * sizeof(int));
#endif
                }
                else
                {
#if ARRAY_SORT
                    for (int i = 0; i < movablePositions.Length; i++)
                    {
                        order[depth][i] = i;
                    }

                    if (turn == State.Black)
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length, descendingComparer);
                    }
                    else
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length);
                    }
#elif LINQ_SORT
                    int[] orderSource = turn == State.Black ? value[depth].OrderingDescending(movablePositions.Length).ToArray() : value[depth].Ordering(movablePositions.Length).ToArray();
                    Array.Copy(orderSource, order[depth], movablePositions.Length);
                    //Buffer.BlockCopy(orderSource, 0, order[depth], 0, movablePositions.Length * sizeof(int));
#endif
                }

                int max = -int.MaxValue;
                AlphaBetaInformation ret = new AlphaBetaInformation();

                for (int i = 0; i < movablePositions.Length; i++)
                {
                    board.Move(turn, movablePositions[order[depth][i]]);
                    value[depth][order[depth][i]] = -AlphaBeta(board, notTurn, depth - 1, -beta, -alpha, evaluationFunction, orderThreshold, doRandom).value;
                    board.Unmove();

                    if (value[depth][order[depth][i]] > max)
                    {
                        max = value[depth][order[depth][i]];
                        ret.Set(max, movablePositions[order[depth][i]]);
                        alpha = Math.Max(alpha, max);

                        if (max >= beta)
                        {
                            return ret;
                        }
                    }

                    if (condition)
                    {
                        WriteLogAndSetProgress(order[depth][i], value[depth][order[depth][i]]);
                    }
                }

                if (movablePositions.Length == 0)
                {
                    if (!board.CanMove(notTurn))
                    {
                        countEvaluation++;

                        return ret.Set(turn.Sign() * evaluationFunction(board, turn), LinkedPosition.Out);
                    }

                    return ret.Set(-AlphaBeta(board, notTurn, depth, -beta, -alpha, evaluationFunction, orderThreshold, doRandom).value, LinkedPosition.Out);
                }

                return ret;
            }
            else if (solver && depth <= 10)
            {
                switch (depth)
                {
                    case 10:
                        return Last10(board, turn, notTurn, alpha, beta);
                    case 9:
                        return Last9(board, turn, notTurn, alpha, beta);
                    case 8:
                        return Last8(board, turn, notTurn, alpha, beta);
                    case 7:
                        return Last7(board, turn, notTurn, alpha, beta);
                    case 6:
                        return Last6(board, turn, notTurn, alpha, beta);
                    case 5:
                        return Last5(board, turn, notTurn, alpha, beta);
                    case 4:
                        return Last4(board, turn, notTurn, alpha, beta);
                    case 3:
                        return Last3(board, turn, notTurn, alpha, beta);
                    case 2:
                        return Last2(board, turn, notTurn, alpha, beta);
                    case 1:
                        return Last1(board, turn, notTurn);
                    default:
                        return null;
                }
            }
            else
            {
                int value;
                int max = -int.MaxValue;
                AlphaBetaInformation ret = new AlphaBetaInformation();

                for (LinkedPosition linkedPosition = board.emptyPositions.first; linkedPosition != null; linkedPosition = linkedPosition.next)
                {
                    foreach (Vector direction in linkedPosition.directions)
                    {
                        Position p;
                        Position p0;

                        for (p = p0 = linkedPosition.position + direction; board.array[p] == notTurn; p += direction) ;

                        if (p != p0 && board.array[p] == turn)
                        {
                            LinkedPosition movablePosition = linkedPosition;

                            board.Move(turn, movablePosition);
                            value = -AlphaBeta(board, notTurn, depth - 1, -beta, -alpha, evaluationFunction, orderThreshold, doRandom).value;
                            board.Unmove();

                            if (value > max)
                            {
                                max = value;
                                ret.Set(max, movablePosition);
                                alpha = Math.Max(alpha, max);

                                if (max >= beta)
                                {
                                    return ret;
                                }
                            }

                            if (condition)
                            {
                                WriteLogAndSetProgress(ctr, value);
                            }

                            break;
                        }
                    }
                }

                if (max == -int.MaxValue)
                {
                    if (!board.CanMove(notTurn))
                    {
                        countEvaluation++;

                        return ret.Set(turn.Sign() * evaluationFunction(board, turn), LinkedPosition.Out);
                    }

                    return ret.Set(-AlphaBeta(board, notTurn, depth, -beta, -alpha, evaluationFunction, orderThreshold, doRandom).value, LinkedPosition.Out);
                }

                return ret;
            }
        }

        public override LinkedPosition Choose(Board board, State turn)
        {
            int alpha = -int.MaxValue;
            int beta = int.MaxValue;

            Set();

            countEvaluation = 0;
            ctr = 0;
            emptyPositionsCount = board.emptyPositions.count;
            value = new int[depth + 1][];
            order = new int[depth + 1][];

            for (int d = 0; d <= depth; d++)
            {
                value[d] = new int[emptyPositionsCount];
                order[d] = new int[emptyPositionsCount];
            }

            InitializeProgress();

            AlphaBetaInformation abi = AlphaBeta(board, turn, depth, alpha, beta, evaluationFunction, orderThreshold, doRandom);

            WriteLog();

            return abi.linkedPosition;
        }
    }

    public class Bound
    {
        public int lower;
        public int upper;

        public Bound(int lower, int upper)
        {
            this.lower = lower;
            this.upper = upper;
        }
    }

    public class Transposition
    {
        public Bound bound;
        public LinkedPosition linkedPosition;

        public Transposition(Bound bound, LinkedPosition linkedPosition)
        {
            this.bound = bound;
            this.linkedPosition = linkedPosition;
        }
    }

    public class AlphaBetaTranspositionPlayer : AlphaBetaPlayer
    {
        public Dictionary<Tuple<Board, State, int, EvaluationFunction>, Transposition> transposition = new Dictionary<Tuple<Board, State, int, EvaluationFunction>, Transposition>();
        public int countTranspositionUse;
        public int transpositionThreshold;

        public override void Set()
        {
            evaluationFunction = Exe.evaluationFunction;
            depth = Exe.alphaBetaDepth;
            orderThreshold = Exe.alphaBetaOrderThreshold;
            transpositionThreshold = Exe.alphaBetaTranspositionThreshold;
            doRandom = Exe.alphaBetaRandom;
            random = Exe.random;
        }

        public AlphaBetaInformation AlphaBetaTransposition(Board board, State turn, int depth, int alpha, int beta, EvaluationFunction evaluationFunction, int orderThreshold, bool doRandom)
        {
            if (depth <= transpositionThreshold)
            {
                return AlphaBeta(board, turn, depth, alpha, beta, evaluationFunction, orderThreshold, doRandom);
            }

            AlphaBetaInformation ret = new AlphaBetaInformation();
            State notTurn = turn.Not();
            bool condition = !mtdf && !iterativeDeepening && board.emptyPositions.count == emptyPositionsCount;
            LinkedPosition[] movablePositions = board.MovablePositions(turn);
            EvaluationFunction orderEvaluationFunction = new EvaluationFunction(EvaluationFunctions.Movable3);
            Tuple<Board, State, int, EvaluationFunction> key = Tuple.Create(board.CloneArray(), turn, depth, evaluationFunction);

            if (transposition.ContainsKey(key))
            {
                Bound bound = transposition[key].bound;
                LinkedPosition linkedPosition = transposition[key].linkedPosition;

                if (bound.lower >= beta)
                {
                    countTranspositionUse++;

                    return ret.Set(bound.lower, linkedPosition);
                }

                if (bound.upper <= alpha)
                {
                    countTranspositionUse++;

                    return ret.Set(bound.upper, linkedPosition);
                }

                if (bound.lower == bound.upper)
                {
                    countTranspositionUse++;

                    return ret.Set(bound.lower, linkedPosition);
                }

                alpha = Math.Max(alpha, bound.lower);
                beta = Math.Min(beta, bound.upper);
            }

            int g = -int.MaxValue;

            if (depth == 0)
            {
                countEvaluation++;

                g = turn.Sign() * evaluationFunction(board, turn);

                goto End;
            }

            if (depth > orderThreshold)
            {
                for (int i = 0; i < movablePositions.Length; i++)
                {
                    board.Move(turn, movablePositions[i]);
                    value[depth][i] = orderEvaluationFunction(board, notTurn);
                    board.Unmove();
                }

                if (doRandom)
                {
#if ARRAY_SORT
                    for (int i = 0; i < movablePositions.Length; i++)
                    {
                        order[depth][i] = i;
                    }

                    Shuffle(value[depth], order[depth], movablePositions.Length, random);

                    if (turn == State.Black)
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length, descendingComparer);
                    }
                    else
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length);
                    }
#elif LINQ_SORT
                    int[] orderSource = turn == State.Black ? value[depth].OrderingDescending(random, movablePositions.Length).ToArray() : value[depth].Ordering(random, movablePositions.Length).ToArray();
                    Array.Copy(orderSource, order[depth], movablePositions.Length);
                    //Buffer.BlockCopy(orderSource, 0, order[depth], 0, movablePositions.Length * sizeof(int));
#endif
                }
                else
                {
#if ARRAY_SORT
                    for (int i = 0; i < movablePositions.Length; i++)
                    {
                        order[depth][i] = i;
                    }

                    if (turn == State.Black)
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length, descendingComparer);
                    }
                    else
                    {
                        Array.Sort(value[depth], order[depth], 0, movablePositions.Length);
                    }
#elif LINQ_SORT
                    int[] orderSource = turn == State.Black ? value[depth].OrderingDescending(movablePositions.Length).ToArray() : value[depth].Ordering(movablePositions.Length).ToArray();
                    Array.Copy(orderSource, order[depth], movablePositions.Length);
                    //Buffer.BlockCopy(orderSource, 0, order[depth], 0, movablePositions.Length * sizeof(int));
#endif
                }
            }
            else
            {
                for (int i = 0; i < movablePositions.Length; i++)
                {
                    order[depth][i] = i;
                }
            }

            int a = alpha;

            for (int i = 0; i < movablePositions.Length; i++)
            {
                board.Move(turn, movablePositions[order[depth][i]]);
                value[depth][order[depth][i]] = -AlphaBetaTransposition(board, notTurn, depth - 1, -beta, -a, evaluationFunction, orderThreshold, doRandom).value;
                board.Unmove();

                if (value[depth][order[depth][i]] > g)
                {
                    g = value[depth][order[depth][i]];
                    ret.Set(g, movablePositions[order[depth][i]]);
                    a = Math.Max(a, g);

                    if (g >= beta)
                    {
                        transposition[key] = new Transposition(new Bound(g, int.MaxValue), ret.linkedPosition);

                        return ret;
                    }
                }

                if (condition)
                {
                    WriteLogAndSetProgress(order[depth][i], value[depth][order[depth][i]]);
                }
            }

            if (movablePositions.Length == 0)
            {
                if (!board.CanMove(notTurn))
                {
                    countEvaluation++;

                    g = turn.Sign() * evaluationFunction(board, turn);

                    goto End;
                }

                g = -AlphaBetaTransposition(board, notTurn, depth, -beta, -alpha, evaluationFunction, orderThreshold, doRandom).value;

                goto End;
            }

        End:

            if (g <= alpha)
            {
                transposition[key] = new Transposition(new Bound(-int.MaxValue, g), ret.linkedPosition);
            }
            else if (g >= beta)
            {
                transposition[key] = new Transposition(new Bound(g, int.MaxValue), ret.linkedPosition);
            }
            else
            {
                transposition[key] = new Transposition(new Bound(g, g), ret.linkedPosition);
            }

            ret.value = g;

            return ret;
        }

        public override LinkedPosition Choose(Board board, State turn)
        {
            int alpha = -int.MaxValue;
            int beta = int.MaxValue;

            Set();

            countEvaluation = 0;
            ctr = 0;
            emptyPositionsCount = board.emptyPositions.count;
            value = new int[depth + 1][];
            order = new int[depth + 1][];
            countTranspositionUse = 0;

            for (int d = 0; d <= depth; d++)
            {
                value[d] = new int[emptyPositionsCount];
                order[d] = new int[emptyPositionsCount];
            }

            InitializeProgress();

            AlphaBetaInformation abi = AlphaBetaTransposition(board, turn, depth, alpha, beta, evaluationFunction, orderThreshold, doRandom);

            WriteLog();

            return abi.linkedPosition;
        }
    }

    public class AlphaBetaMTDfPlayer : AlphaBetaTranspositionPlayer
    {
        public int countLoop;

        public override void Set()
        {
            evaluationFunction = Exe.evaluationFunction;
            depth = Exe.alphaBetaDepth;
            orderThreshold = Exe.alphaBetaOrderThreshold;
            transpositionThreshold = Exe.alphaBetaTranspositionThreshold;
            doRandom = Exe.alphaBetaRandom;
            random = Exe.random;
        }

        public virtual void WriteLogAndSetProgress(int countLoop, int lowerBound, int upperBound, Position position)
        {
        }
        
        public AlphaBetaInformation MTDf(Board board, State turn, int depth, int f, EvaluationFunction evaluationFunction, int orderThreshold, bool doRandom)
        {
            AlphaBetaInformation ret = new AlphaBetaInformation();
            AlphaBetaInformation tmp = ret;
            const int epsilon = 1;
            int g = f;
            int lowerBound = -int.MaxValue;
            int upperBound = int.MaxValue;
            bool flag = false;

            while (lowerBound < upperBound)
            {
                int beta = g == lowerBound ? g + epsilon : g;
                tmp = ret;
                ret = AlphaBetaTransposition(board, turn, depth, beta - epsilon, beta, evaluationFunction, orderThreshold, doRandom);

                g = ret.value;

                if (g < beta)
                {
                    upperBound = g;
                    flag = true;
                }
                else
                {
                    lowerBound = g;
                    flag = false;
                }

                countLoop++;

                WriteLogAndSetProgress(countLoop, lowerBound, upperBound, ret.linkedPosition.position);
            }

            if (flag)
            {
                ret = tmp;
            }

            return ret;
        }

        public override LinkedPosition Choose(Board board, State turn)
        {
            int f = 0;

            Set();

            countEvaluation = 0;
            ctr = 0;
            emptyPositionsCount = board.emptyPositions.count;
            mtdf = true;
            value = new int[depth + 1][];
            order = new int[depth + 1][];
            countTranspositionUse = 0;
            countLoop = 0;

            for (int d = 0; d <= depth; d++)
            {
                value[d] = new int[emptyPositionsCount];
                order[d] = new int[emptyPositionsCount];
            }

            InitializeProgress();

            AlphaBetaInformation abi = MTDf(board, turn, depth, f, evaluationFunction, orderThreshold, doRandom);

            WriteLog();

            return abi.linkedPosition;
        }
    }

    public class IterativeDeepeningTranspositionPlayer : AlphaBetaTranspositionPlayer
    {
        public override void Set()
        {
            evaluationFunction = Exe.evaluationFunction;
            depth = Exe.iterativeDeepeningDepth;
            orderThreshold = Exe.iterativeDeepeningOrderThreshold;
            transpositionThreshold = Exe.iterativeDeepeningTranspositionThreshold;
            doRandom = Exe.iterativeDeepeningRandom;
            random = Exe.random;
        }

        public virtual void WriteLogAndSetProgress(int depth, ulong countEvaluation, int countTranspositionUse, int transpositionCount, int value, Position position)
        {
        }
        
        public override LinkedPosition Choose(Board board, State turn)
        {
            Set();

            AlphaBetaInformation abi = new AlphaBetaInformation();
            State notTurn = turn.Not();

            InitializeProgress();

            ctr = 0;

            for (int depth = 1; depth <= this.depth; depth++)
            {
                countEvaluation = 0;
                emptyPositionsCount = board.emptyPositions.count;
                iterativeDeepening = true;
                value = new int[depth + 1][];
                order = new int[depth + 1][];
                countTranspositionUse = 0;

                for (int d = 0; d <= depth; d++)
                {
                    value[d] = new int[emptyPositionsCount];
                    order[d] = new int[emptyPositionsCount];
                }

                abi = AlphaBetaTransposition(board, turn, depth, -int.MaxValue, int.MaxValue, evaluationFunction, orderThreshold, doRandom);

                WriteLogAndSetProgress(depth, countEvaluation, countTranspositionUse, transposition.Count, abi.value, abi.linkedPosition.position);

                if (depth == this.depth)
                {
                    break;
                }
            }

            return abi.linkedPosition;
        }
    }

    public class IterativeDeepeningMTDfPlayer : AlphaBetaMTDfPlayer
    {
        public override void Set()
        {
            evaluationFunction = Exe.evaluationFunction;
            depth = Exe.iterativeDeepeningDepth;
            orderThreshold = Exe.iterativeDeepeningOrderThreshold;
            transpositionThreshold = Exe.iterativeDeepeningTranspositionThreshold;
            doRandom = Exe.iterativeDeepeningRandom;
            random = Exe.random;
        }

        public virtual void WriteLogAndSetProgress(int depth, ulong countEvaluation, int countTranspositionUse, int transpositionCount, int value, Position position)
        {
        }
        
        public override LinkedPosition Choose(Board board, State turn)
        {
            Set();

            AlphaBetaInformation abi = new AlphaBetaInformation();
            State notTurn = turn.Not();

            InitializeProgress();

            ctr = 0;

            for (int depth = 1; depth <= this.depth; depth++)
            {
                countEvaluation = 0;
                emptyPositionsCount = board.emptyPositions.count;
                iterativeDeepening = true;
                value = new int[depth + 1][];
                order = new int[depth + 1][];
                countTranspositionUse = 0;
                countLoop = 0;

                for (int d = 0; d <= depth; d++)
                {
                    value[d] = new int[emptyPositionsCount];
                    order[d] = new int[emptyPositionsCount];
                }

                abi = MTDf(board, turn, depth, abi.value, evaluationFunction, orderThreshold, doRandom);

                WriteLogAndSetProgress(depth, countEvaluation, countTranspositionUse, transposition.Count, abi.value, abi.linkedPosition.position);

                if (depth == this.depth)
                {
                    break;
                }
            }

            return abi.linkedPosition;
        }
    }

    public class SolverTranspositionPlayer : AlphaBetaTranspositionPlayer
    {
        protected Player basePlayer;

        public override void Set()
        {
            depth = Exe.solverDepth;
            orderThreshold = Exe.solverOrderThreshold;
            transpositionThreshold = Exe.solverTranspositionThreshold;
            doRandom = Exe.solverRandom;
            random = Exe.random;
            basePlayer = Exe.basePlayer;
        }

        public override LinkedPosition Choose(Board board, State turn)
        {
            Set();
            
            int depth = board.emptyPositions.count;

            if (depth > this.depth)
            {
                return basePlayer.Choose(board, turn);
            }

            int alpha = -int.MaxValue;
            int beta = int.MaxValue;
            EvaluationFunction evaluationFunction = new EvaluationFunction(EvaluationFunctions.Score);

            countEvaluation = 0;
            ctr = 0;
            emptyPositionsCount = board.emptyPositions.count;
            value = new int[depth + 1][];
            order = new int[depth + 1][];
            countTranspositionUse = 0;
            solver = true;

            for (int d = 0; d <= depth; d++)
            {
                value[d] = new int[emptyPositionsCount];
                order[d] = new int[emptyPositionsCount];
            }

            InitializeProgress();

            AlphaBetaInformation abi = AlphaBetaTransposition(board, turn, depth, alpha, beta, evaluationFunction, orderThreshold, doRandom);

            WriteLog();

            return abi.linkedPosition;
        }
    }

    public class SolverMTDfPlayer : AlphaBetaMTDfPlayer
    {
        protected Player basePlayer;

        public override void Set()
        {
            depth = Exe.solverDepth;
            orderThreshold = Exe.solverOrderThreshold;
            transpositionThreshold = Exe.solverTranspositionThreshold;
            doRandom = Exe.solverRandom;
            random = Exe.random;
            basePlayer = Exe.basePlayer;
        }

        public override void WriteLog()
        {
            Console.WriteLine("evaluation = " + countEvaluation);
            Console.WriteLine("transpositionUse = " + countTranspositionUse);
            Console.WriteLine("transpositionCount = " + transposition.Count);
        }

        public override void WriteLogAndSetProgress(int countLoop, int lowerBound, int upperBound, Position position)
        {
            Console.WriteLine("loop = " + countLoop + "\t" + "lowerBound = " + lowerBound + "\t" + "upperBound = " + upperBound + "\t" + "position = " + Board.PositionString(position));
        }
        
        public override LinkedPosition Choose(Board board, State turn)
        {
            Set();

            int depth = board.emptyPositions.count;

            if (depth > this.depth)
            {
                return basePlayer.Choose(board, turn);
            }

            int f = 0;
            EvaluationFunction evaluationFunction = new EvaluationFunction(EvaluationFunctions.Score);

            countEvaluation = 0;
            ctr = 0;
            emptyPositionsCount = board.emptyPositions.count;
            value = new int[depth + 1][];
            order = new int[depth + 1][];
            countTranspositionUse = 0;
            solver = true;
            mtdf = true;
            countLoop = 0;

            for (int d = 0; d <= depth; d++)
            {
                value[d] = new int[emptyPositionsCount];
                order[d] = new int[emptyPositionsCount];
            }

            InitializeProgress();

            AlphaBetaInformation abi = MTDf(board, turn, depth, f, evaluationFunction, orderThreshold, doRandom);

            WriteLog();

            return abi.linkedPosition;
        }
    }
    
    class Exe
    {
        public static Random random;
        public static int iteration;
        public static EvaluationFunction evaluationFunction = new EvaluationFunction(EvaluationFunctions.Stable);
        public static int alphaBetaDepth;
        public static int alphaBetaOrderThreshold;
        public static int alphaBetaTranspositionThreshold;
        public static bool alphaBetaRandom;
        public static int iterativeDeepeningDepth;
        public static int iterativeDeepeningOrderThreshold;
        public static int iterativeDeepeningTranspositionThreshold;
        public static bool iterativeDeepeningRandom;
        public static int solverDepth;
        public static int solverOrderThreshold;
        public static int solverTranspositionThreshold;
        public static bool solverRandom;
        public static Player basePlayer;
        public static LinkedPosition[] movablePositions;

        static void StartReversi()
        {
            int size = 8;
            Pattern pattern = Pattern.Cross;
            int phase = 0;
            State turn = State.Black;
            Board board = new Board(size, pattern);
            Dictionary<State, Player> players = new Dictionary<State, Player>();
            //players[State.Black] = new HumanPlayer();
            //players[State.Black] = new RandomPlayer();
            //players[State.Black] = new MonteCarloTreeSearchPlayer();
            //players[State.Black] = new AlphaBetaPlayer();
            //players[State.Black] = new AlphaBetaTranspositionPlayer();
            //players[State.Black] = new AlphaBetaMTDfPlayer();
            //players[State.Black] = new IterativeDeepeningTranspositionPlayer();
            //players[State.Black] = new IterativeDeepeningMTDfPlayer();
            //players[State.Black] = new SolverTranspositionPlayer();
            players[State.Black] = new SolverMTDfPlayer();
            //players[State.White] = new HumanPlayer();
            players[State.White] = new RandomPlayer();
            //players[State.White] = new MonteCarloTreeSearchPlayer();
            //players[State.White] = new AlphaBetaPlayer();
            //players[State.White] = new AlphaBetaTranspositionPlayer();
            //players[State.White] = new AlphaBetaMTDfPlayer();
            //players[State.White] = new IterativeDeepeningTranspositionPlayer();
            //players[State.White] = new IterativeDeepeningMTDfPlayer();
            //players[State.White] = new SolverTranspositionPlayer();
            //players[State.White] = new SolverMTDfPlayer();
            int seed = 1;
            random = new Random(seed);
            iteration = 100;
            evaluationFunction = new EvaluationFunction(EvaluationFunctions.Stable);
            alphaBetaDepth = 10;
            alphaBetaOrderThreshold = 1;
            alphaBetaTranspositionThreshold = 1;
            alphaBetaRandom = false;
            iterativeDeepeningDepth = 10;
            iterativeDeepeningOrderThreshold = 1;
            iterativeDeepeningTranspositionThreshold = 1;
            iterativeDeepeningRandom = false;
            solverDepth = 20;
            solverOrderThreshold = 6;
            solverTranspositionThreshold = 9;
            solverRandom = false;
            basePlayer = new RandomPlayer();
            movablePositions = board.MovablePositions(turn);

            while (true)
            {
                State notTurn = turn.Not();

                Console.WriteLine("Phase: " + phase);
                Console.WriteLine("Turn: " + turn);
                Console.WriteLine("Black " + board.countBlack + "\t" + "White " + board.countWhite);
                Console.WriteLine("Movable Positions: " + movablePositions.Select(linkedPosition => Board.PositionString(linkedPosition.position)).ToStringExtension());
                Console.WriteLine(board);

                if (board.CanMove(turn))
                {
                    LinkedPosition linkedPosition = players[turn].Choose(board, turn);
                    board.Move(turn, linkedPosition);
                    Console.WriteLine(turn + " Moves " + Board.PositionString(linkedPosition.position));
                }
                else
                {
                    if (!board.CanMove(notTurn))
                    {
                        Console.WriteLine("End");

                        break;
                    }

                    Console.WriteLine("Pass");
                }

                turn = notTurn;
                phase++;

                movablePositions = board.MovablePositions(turn);
            }
        }

        static void Set(Board board, string boardString)
        {
            int ctr = 0;

            board.countBlack = 0;
            board.countWhite = 0;

            int c0 = Board.size / 2 - 1;
            int c1 = Board.size / 2;

            for (int i = 0; i < Board.size; i++)
            {
                for (int j = 0; j < Board.size; j++)
                {
                    Position position = Board.Position(i, j);
                    LinkedPosition linkedPosition = null;

                    for (LinkedPosition lp = board.emptyPositions.first; lp != null; lp = lp.next)
                    {
                        if (lp.position == position)
                        {
                            linkedPosition = lp;

                            break;
                        }
                    }

                    bool flag = false;

                    if ((i == c0 || i == c1) && (j == c0 || j == c1))
                    {
                        flag = true;
                    }

                    switch (boardString[ctr++])
                    {
                        case '-':
                            board.array[position] = State.Empty;

                            break;
                        case 'X':
                            board.array[position] = State.Black;
                            board.countBlack++;

                            if (!flag)
                            {
                                board.emptyPositions.Remove(linkedPosition);
                            }

                            break;
                        case 'O':
                            board.array[position] = State.White;
                            board.countWhite++;

                            if (!flag)
                            {
                                board.emptyPositions.Remove(linkedPosition);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void Test(string title, string boardString, State turn, int orderThreshold, int transpositionThreshold)
        {
            Console.WriteLine("/*** " + title + " ***/");

            Console.WriteLine();
            
            Board board = new Board(8, Pattern.Cross);
            Set(board, boardString);
            Console.WriteLine(board);

            Console.WriteLine("Black = " + board.countBlack);
            Console.WriteLine("White = " + board.countWhite);
            Console.WriteLine("Empty = " + board.emptyPositions.count);

            Console.WriteLine("Turn = " + turn);

            SolverMTDfPlayer solverMTDfPlayer = new SolverMTDfPlayer();
            Console.WriteLine("SolverPlayer = " + solverMTDfPlayer);

            solverDepth = board.emptyPositions.count;
            Console.WriteLine("SolverDepth = " + solverDepth);

            solverOrderThreshold = orderThreshold;
            Console.WriteLine("SolverOrderThreshold = " + solverOrderThreshold);

            solverTranspositionThreshold = transpositionThreshold;
            Console.WriteLine("SolverTranspositionThreshold = " + solverTranspositionThreshold);

            Console.WriteLine();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            LinkedPosition linkedPosition = solverMTDfPlayer.Choose(board, turn);
            stopwatch.Stop();

            Console.WriteLine("Time [ms] = " + stopwatch.ElapsedMilliseconds);

            Console.WriteLine();
        }
        
        public static void Main(string[] args)
        {
            //StartReversi();

            Test("FFO#40", "O--OOOOX-OOOOOOXOOXXOOOXOOXOOOXXOOOOOOXX---OOOOX----O--X--------", State.Black, 6, 9);
            Test("FFO#41", "-OOOOO----OOOOX--OOOOOO-XXXXXOO--XXOOX--OOXOXX----OXXO---OOO--O-", State.Black, 6, 9);
            Test("FFO#42", "--OOO-------XX-OOOOOOXOO-OOOOXOOX-OOOXXO---OOXOO---OOOXO--OOOO--", State.Black, 6, 9);
            Test("FFO#43", "--XXXXX---XXXX---OOOXX---OOXXXX--OOXXXO-OOOOXOO----XOX----XXXXX-", State.White, 6, 9);
            Test("FFO#44", "--O-X-O---O-XO-O-OOXXXOOOOOOXXXOOOOOXX--XXOOXO----XXXX-----XXX--", State.White, 6, 9);
            Test("FFO#45", "---XXXX-X-XXXO--XXOXOO--XXXOXO--XXOXXO---OXXXOO-O-OOOO------OO--", State.Black, 6, 9);
            Test("FFO#46", "---XXX----OOOX----OOOXX--OOOOXXX--OOOOXX--OXOXXX--XXOO---XXXX-O-", State.Black, 6, 9);
            Test("FFO#47", "-OOOOO----OOOO---OOOOX--XXXXXX---OXOOX--OOOXOX----OOXX----XXXX--", State.White, 6, 9);
            Test("FFO#48", "-----X--X-XXX---XXXXOO--XOXOOXX-XOOXXX--XOOXX-----OOOX---XXXXXX-", State.White, 6, 9);
            //Test("FFO#49", "--OX-O----XXOO--OOOOOXX-OOOOOX--OOOXOXX-OOOOXX-----OOX----X-O---", State.Black, 6, 9);
            //Test("FFO#50", "----X-----XXX----OOOXOOO-OOOXOOO-OXOXOXO-OOXXOOO--OOXO----O--O--", State.Black, 6, 9);
            //Test("FFO#51", "----O-X------X-----XXXO-OXXXXXOO-XXOOXOOXXOXXXOO--OOOO-O----OO--", State.White, 6, 9);
            //Test("FFO#52", "---X-------OX--X--XOOXXXXXXOXXXXXXXOOXXXXXXOOOXX--XO---X--------", State.White, 6, 9);
            //Test("FFO#53", "----OO-----OOO---XXXXOOO--XXOOXO-XXXXXOO--OOOXOO--X-OX-O-----X--", State.Black, 6, 9);
            //Test("FFO#54", "--OOO---XXOO----XXXXOOOOXXXXOX--XXXOXX--XXOOO------OOO-----O----", State.Black, 6, 9);
            //Test("FFO#55", "--------X-X------XXXXOOOOOXOXX--OOOXXXX-OOXXXX--O-OOOX-----OO---", State.Black, 6, 9);
            //Test("FFO#56", "--XXXXX---XXXX---OOOXX---OOXOX---OXXXXX-OOOOOXO----OXX----------", State.White, 6, 9);
            //Test("FFO#57", "-------------------XXOOO--XXXOOO--XXOXOO-OOOXXXO--OXOO-O-OOOOO--", State.Black, 6, 9);
            //Test("FFO#58", "--XOOO----OOO----OOOXOO--OOOOXO--OXOXXX-OOXXXX----X-XX----------", State.Black, 6, 9);
            //Test("FFO#59", "-----------------------O--OOOOO---OOOOOXOOOOXXXX--XXOOXX--XX-O-X", State.Black, 6, 9);

            Console.ReadLine();
        }
    }
}
