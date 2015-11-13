using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using GeneralReversi;
using ExtendedLabelLibrary;

namespace GeneralReversiGui
{
    using Position = Int32;

    public partial class Form1 : Form
    {
        public Form1(bool log)
        {
            InitializeComponent();

            this.log = log;
        }

        private bool log;

        private Label start_Label;
        private Label reset_Label;
        private Label size_Label;
        private Label pattern_Label;
        private Label blackPlayer_Label;
        private Label whitePlayer_Label;
        private Label last_Label;
        private Label movable_Label;
        private Label pass_Label;
        private Label delay_Label;
        private Label history_Label;
        private Label seed_Label;
        private Label iteration_Label;
        private Label evaluationFunction_Label;
        private Label alphaBetaDepth_Label;
        private Label alphaBetaOrderThreshold_Label;
        private Label alphaBetaTranspositionThreshold_Label;
        private Label alphaBetaRandom_Label;
        private Label iterativeDeepeningDepth_Label;
        private Label iterativeDeepeningOrderThreshold_Label;
        private Label iterativeDeepeningTranspositionThreshold_Label;
        private Label iterativeDeepeningRandom_Label;
        private Label basePlayer_Label;
        private Label solverDepth_Label;
        private Label solverOrderThreshold_Label;
        private Label solverTranspositionThreshold_Label;
        private Label solverRandom_Label;
        private Label clear_Label;
        private Label input_Label;
        private Label output_Label;
        private Label about_Label;

        public static CheckBox start_CheckBox;
        private Button reset_Button;
        private ComboBox size_ComboBox;
        private ComboBox pattern_ComboBox;
        private ComboBox blackPlayer_ComboBox;
        private ComboBox whitePlayer_ComboBox;
        private CheckBox last_CheckBox;
        private CheckBox movable_CheckBox;
        private CheckBox pass_CheckBox;
        private ComboBox delay_ComboBox;
        private ComboBox history_ComboBox;
        private ComboBox seed_ComboBox;
        private ComboBox iteration_ComboBox;
        public static ComboBox evaluationFunction_ComboBox;
        private ComboBox alphaBetaDepth_ComboBox;
        private ComboBox alphaBetaOrderThreshold_ComboBox;
        private ComboBox alphaBetaTranspositionThreshold_ComboBox;
        public static CheckBox alphaBetaRandom_CheckBox;
        private ComboBox iterativeDeepeningDepth_ComboBox;
        private ComboBox iterativeDeepeningOrderThreshold_ComboBox;
        private ComboBox iterativeDeepeningTranspositionThreshold_ComboBox;
        public static CheckBox iterativeDeepeningRandom_CheckBox;
        private ComboBox basePlayer_ComboBox;
        private ComboBox solverDepth_ComboBox;
        private ComboBox solverOrderThreshold_ComboBox;
        private ComboBox solverTranspositionThreshold_ComboBox;
        public static CheckBox solverRandom_CheckBox;
        private Button clear_Button;
        private Button input_Button;
        private Button output_Button;
        private Button about_Button;

        private PictureBox board_PictureBox;
        private PictureBox information_PictureBox;
        private ExtendedLabel phase_ExtendedLabel;
        private ExtendedLabel text_ExtendedLabel;
        public static ProgressBar _ProgressBar;
        public static TextBox log_TextBox;

        private TableLayoutPanel all_TableLayoutPanel;
        private TableLayoutPanel configuration_TableLayoutPanel;
        private TabControl boardAndLog_TabControl;
        private TabPage board_TabPage;
        private TabPage log_TabPage;
        private TableLayoutPanel board_TableLayoutPanel;

        int side;
        int boardOffset;
        int size;
        Pattern pattern;
        int phase;
        State turn;
        Board board;
        Board boardForResize;
        Dictionary<State, Player> players;
        LinkedPosition linkedPosition;
        public static LinkedPosition linkedPositionByHuman;
        public static int delay;
        int seed;
        public static Random random;
        public static int iteration;
        public static EvaluationFunction evaluationFunction;
        public static int alphaBetaDepth;
        public static int alphaBetaOrderThreshold;
        public static int alphaBetaTranspositionThreshold;
        public static int iterativeDeepeningDepth;
        public static int iterativeDeepeningOrderThreshold;
        public static int iterativeDeepeningTranspositionThreshold;
        public static Player basePlayer;
        public static int solverDepth;
        public static int solverOrderThreshold;
        public static int solverTranspositionThreshold;
        Thread thread;
        Thread thread2;
        public static LinkedPosition[] movablePositions;

        const double WidthRatioConfigurationToAll = 0.45;
        const double HeightRatioConfigurationToAll = 0.95;
        const double WidthRatioBoardAndLogToAll = 0.45;
        const double HeightRatioBoardAndLogToAll = 0.95;
        const double RatioBoard = 0.9;
        const int ClientSizeWidth = 1200;
        const int ClientSizeHeight = 900;
        const double RatioHeightToWidth = (double)ClientSizeHeight / ClientSizeWidth;

        private void Form1_Load(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            SuspendLayout();

            //
            //
            // Form1
            //
            //
            ClientSize = new Size(ClientSizeWidth, ClientSizeHeight);
            Text = "GeneralReversi";
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Left = (Screen.PrimaryScreen.Bounds.Width - Width) / 2;
            Top = (Screen.PrimaryScreen.Bounds.Height - Height) / 2;

            //
            // all_TableLayoutPanel
            //
            //
            all_TableLayoutPanel = new TableLayoutPanel();

            all_TableLayoutPanel.Size = ClientSize;
            all_TableLayoutPanel.RowCount = 1;
            all_TableLayoutPanel.ColumnCount = 2;
            all_TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            for (int i = 0; i < all_TableLayoutPanel.RowCount; i++)
            {
                all_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (int i = 0; i < all_TableLayoutPanel.ColumnCount; i++)
            {
                all_TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            all_TableLayoutPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 50);
            all_TableLayoutPanel.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 50);

            //
            // configuration_TableLayoutPanel
            //
            configuration_TableLayoutPanel = new TableLayoutPanel();

            configuration_TableLayoutPanel.Size = new Size((int)(WidthRatioConfigurationToAll * all_TableLayoutPanel.Width), (int)(HeightRatioConfigurationToAll * all_TableLayoutPanel.Height));
            configuration_TableLayoutPanel.RowCount = 30;
            configuration_TableLayoutPanel.ColumnCount = 2;
            configuration_TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            configuration_TableLayoutPanel.Anchor = AnchorStyles.None;

            for (int i = 0; i < configuration_TableLayoutPanel.RowCount; i++)
            {
                configuration_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, configuration_TableLayoutPanel.Height / configuration_TableLayoutPanel.RowCount - 1));
            }

            configuration_TableLayoutPanel.Height = (configuration_TableLayoutPanel.Height / configuration_TableLayoutPanel.RowCount) * configuration_TableLayoutPanel.RowCount;

            for (int i = 0; i < configuration_TableLayoutPanel.ColumnCount; i++)
            {
                configuration_TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            configuration_TableLayoutPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 50);
            configuration_TableLayoutPanel.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 50);

            // start_Label
            //
            start_Label = new Label();

            start_Label.Text = "Start";
            start_Label.TextAlign = ContentAlignment.MiddleCenter;
            start_Label.Anchor = AnchorStyles.None;
            start_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(start_Label);

            // start_CheckBox
            //
            start_CheckBox = new CheckBox();

            start_CheckBox.Text = "Start";
            start_CheckBox.Appearance = Appearance.Button;
            start_CheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            start_CheckBox.TextAlign = ContentAlignment.MiddleCenter;

            start_CheckBox.CheckedChanged += new EventHandler(start_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(start_CheckBox);

            // reset_Label
            //
            reset_Label = new Label();

            reset_Label.Text = "Reset";
            reset_Label.TextAlign = ContentAlignment.MiddleCenter;
            reset_Label.Anchor = AnchorStyles.None;
            reset_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(reset_Label);

            // reset_Button
            //
            reset_Button = new Button();

            reset_Button.Text = "Reset";
            reset_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            reset_Button.TextAlign = ContentAlignment.MiddleCenter;

            reset_Button.Click += new EventHandler(reset_Button_Click);

            configuration_TableLayoutPanel.Controls.Add(reset_Button);

            // size_Label
            //
            size_Label = new Label();

            size_Label.Text = "Board Size";
            size_Label.TextAlign = ContentAlignment.MiddleCenter;
            size_Label.Anchor = AnchorStyles.None;
            size_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(size_Label);

            // size_ComboBox
            //
            size_ComboBox = new ComboBox();

            size_ComboBox.Items.AddRange(Enumerable.Range(2, 9).Select(i => (object)(2 * i)).ToArray());
            size_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            size_ComboBox.SelectedIndex = 2;
            size_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            size_ComboBox.TextChanged += new EventHandler(size_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(size_ComboBox);

            // pattern_Label
            //
            pattern_Label = new Label();

            pattern_Label.Text = "Initial Pattern";
            pattern_Label.TextAlign = ContentAlignment.MiddleCenter;
            pattern_Label.Anchor = AnchorStyles.None;
            pattern_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(pattern_Label);

            // pattern_ComboBox
            //
            pattern_ComboBox = new ComboBox();

            pattern_ComboBox.Items.AddRange(new object[] { Pattern.Cross, Pattern.Parallel });
            pattern_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            pattern_ComboBox.SelectedIndex = 0;
            pattern_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            pattern_ComboBox.SelectionChangeCommitted += new EventHandler(pattern_ComboBox_SelectionChangeCommitted);

            configuration_TableLayoutPanel.Controls.Add(pattern_ComboBox);

            // blackPlayer_Label
            //
            blackPlayer_Label = new Label();

            blackPlayer_Label.Text = "Black Player";
            blackPlayer_Label.TextAlign = ContentAlignment.MiddleCenter;
            blackPlayer_Label.Anchor = AnchorStyles.None;
            blackPlayer_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(blackPlayer_Label);

            // blackPlayer_ComboBox
            //
            blackPlayer_ComboBox = new ComboBox();

            blackPlayer_ComboBox.ValueMember = "Value";
            blackPlayer_ComboBox.DisplayMember = "Display";
            blackPlayer_ComboBox.DataSource = ValueDisplayDataTable(blackPlayer_ComboBox.ValueMember, blackPlayer_ComboBox.DisplayMember, new object[,] { 
            { new HumanPlayer(),                                  "Human" }, 
            { new RandomPlayer(),                                 "Random" }, 
            { new MonteCarloTreeSearchPlayer(),                   "Monte-Carlo" }, 
            { new AlphaBetaTranspositionPlayer(),                 "Alpha-Beta Transposition" },
            { new AlphaBetaMTDfPlayer(),                          "Alpha-Beta MTDf" },
            { new IterativeDeepeningTranspositionPlayer(),        "Iterative-Deepening Transposition" },
            { new IterativeDeepeningMTDfPlayer(),                 "Iterative-Deepening MTDf" },
            { new SolverTranspositionPlayer(),                    "Solver Transposition" },
            { new SolverMTDfPlayer(),                             "Solver MTDf" },
            });
            blackPlayer_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            blackPlayer_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            blackPlayer_ComboBox.SelectionChangeCommitted += new EventHandler(blackPlayer_ComboBox_SelectionChangeCommitted);

            configuration_TableLayoutPanel.Controls.Add(blackPlayer_ComboBox);

            // whitePlayer_Label
            //
            whitePlayer_Label = new Label();

            whitePlayer_Label.Text = "White Player";
            whitePlayer_Label.TextAlign = ContentAlignment.MiddleCenter;
            whitePlayer_Label.Anchor = AnchorStyles.None;
            whitePlayer_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(whitePlayer_Label);

            // whitePlayer_ComboBox
            //
            whitePlayer_ComboBox = new ComboBox();

            whitePlayer_ComboBox.ValueMember = "Value";
            whitePlayer_ComboBox.DisplayMember = "Display";
            whitePlayer_ComboBox.DataSource = ValueDisplayDataTable(whitePlayer_ComboBox.ValueMember, whitePlayer_ComboBox.DisplayMember, new object[,] { 
            { new HumanPlayer(),                                  "Human" }, 
            { new RandomPlayer(),                                 "Random" }, 
            { new MonteCarloTreeSearchPlayer(),                   "Monte-Carlo" }, 
            { new AlphaBetaTranspositionPlayer(),                 "Alpha-Beta Transposition" },
            { new AlphaBetaMTDfPlayer(),                          "Alpha-Beta MTDf" },
            { new IterativeDeepeningTranspositionPlayer(),        "Iterative-Deepening Transposition" },
            { new IterativeDeepeningMTDfPlayer(),                 "Iterative-Deepening MTDf" },
            { new SolverTranspositionPlayer(),                    "Solver Transposition" },
            { new SolverMTDfPlayer(),                             "Solver MTDf" },
            });
            whitePlayer_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            whitePlayer_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            whitePlayer_ComboBox.SelectionChangeCommitted += new EventHandler(whitePlayer_ComboBox_SelectionChangeCommitted);

            configuration_TableLayoutPanel.Controls.Add(whitePlayer_ComboBox);

            // last_Label
            //
            last_Label = new Label();

            last_Label.Text = "Last Position";
            last_Label.TextAlign = ContentAlignment.MiddleCenter;
            last_Label.Anchor = AnchorStyles.None;
            last_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(last_Label);

            // last_CheckBox
            //
            last_CheckBox = new CheckBox();

            last_CheckBox.Checked = true;

            last_CheckBox.CheckedChanged += new EventHandler(last_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(last_CheckBox);

            // movable_Label
            //
            movable_Label = new Label();

            movable_Label.Text = "Movable Positions";
            movable_Label.TextAlign = ContentAlignment.MiddleCenter;
            movable_Label.Anchor = AnchorStyles.None;
            movable_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(movable_Label);

            // movable_CheckBox
            //
            movable_CheckBox = new CheckBox();

            movable_CheckBox.Checked = true;

            movable_CheckBox.CheckedChanged += new EventHandler(movable_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(movable_CheckBox);

            // pass_Label
            //
            pass_Label = new Label();

            pass_Label.Text = "Pass Message";
            pass_Label.TextAlign = ContentAlignment.MiddleCenter;
            pass_Label.Anchor = AnchorStyles.None;
            pass_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(pass_Label);

            // pass_CheckBox
            //
            pass_CheckBox = new CheckBox();

            pass_CheckBox.Checked = true;

            pass_CheckBox.CheckedChanged += new EventHandler(pass_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(pass_CheckBox);

            // delay_Label
            //
            delay_Label = new Label();

            delay_Label.Text = "Delay Time [ms]";
            delay_Label.TextAlign = ContentAlignment.MiddleCenter;
            delay_Label.Anchor = AnchorStyles.None;
            delay_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(delay_Label);

            // delay_ComboBox
            //
            delay_ComboBox = new ComboBox();

            delay_ComboBox.Items.AddRange(Enumerable.Range(0, 10).Select(i => (object)(100 * i)).ToArray());
            delay_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            delay_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            delay_ComboBox.TextChanged += new EventHandler(delay_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(delay_ComboBox);

            // history_Label
            //
            history_Label = new Label();

            history_Label.Text = "History";
            history_Label.TextAlign = ContentAlignment.MiddleCenter;
            history_Label.Anchor = AnchorStyles.None;
            history_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(history_Label);

            // history_ComboBox
            //
            history_ComboBox = new ComboBox();

            history_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            history_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            history_ComboBox.SelectedIndexChanged += new EventHandler(history_ComboBox_SelectedIndexChanged);

            configuration_TableLayoutPanel.Controls.Add(history_ComboBox);

            Controls.Add(configuration_TableLayoutPanel);

            // seed_Label
            //
            seed_Label = new Label();

            seed_Label.Text = "Random Seed";
            seed_Label.TextAlign = ContentAlignment.MiddleCenter;
            seed_Label.Anchor = AnchorStyles.None;
            seed_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(seed_Label);

            // seed_ComboBox
            //
            seed_ComboBox = new ComboBox();

            seed_ComboBox.Items.AddRange(Enumerable.Range(0, 10).Select(i => (object)i).ToArray());
            seed_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            seed_ComboBox.SelectedIndex = 0;
            seed_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            seed_ComboBox.TextChanged += new EventHandler(seed_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(seed_ComboBox);

            // iteration_Label
            //
            iteration_Label = new Label();

            iteration_Label.Text = "Monte-Carlo Iteration";
            iteration_Label.TextAlign = ContentAlignment.MiddleCenter;
            iteration_Label.Anchor = AnchorStyles.None;
            iteration_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(iteration_Label);

            // iteration_ComboBox
            //
            iteration_ComboBox = new ComboBox();

            iteration_ComboBox.Items.AddRange(new object[] { 100, 1000, 10000 });
            iteration_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            iteration_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            iteration_ComboBox.TextChanged += new EventHandler(iteration_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(iteration_ComboBox);

            // evaluationFunction_Label
            //
            evaluationFunction_Label = new Label();

            evaluationFunction_Label.Text = "Evaluation Function";
            evaluationFunction_Label.TextAlign = ContentAlignment.MiddleCenter;
            evaluationFunction_Label.Anchor = AnchorStyles.None;
            evaluationFunction_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(evaluationFunction_Label);

            // evaluationFunction_ComboBox
            //
            evaluationFunction_ComboBox = new ComboBox();

            evaluationFunction_ComboBox.ValueMember = "Value";
            evaluationFunction_ComboBox.DisplayMember = "Display";
            evaluationFunction_ComboBox.DataSource = ValueDisplayDataTable(evaluationFunction_ComboBox.ValueMember, evaluationFunction_ComboBox.DisplayMember, new object[,] { 
            { new EvaluationFunction(EvaluationFunctions.Score), "Score" }, 
            { new EvaluationFunction(EvaluationFunctions.Movable), "Movable" }, 
            { new EvaluationFunction(EvaluationFunctions.Corner), "Corner" }, 
            { new EvaluationFunction(EvaluationFunctions.Stable), "Stable" },
            });
            evaluationFunction_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            evaluationFunction_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            evaluationFunction_ComboBox.SelectionChangeCommitted += new EventHandler(evaluationFunction_ComboBox_SelectionChangeCommitted);
            
            configuration_TableLayoutPanel.Controls.Add(evaluationFunction_ComboBox);

            // alphaBetaDepth_Label
            //
            alphaBetaDepth_Label = new Label();

            alphaBetaDepth_Label.Text = "Alpha-Beta Depth";
            alphaBetaDepth_Label.TextAlign = ContentAlignment.MiddleCenter;
            alphaBetaDepth_Label.Anchor = AnchorStyles.None;
            alphaBetaDepth_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(alphaBetaDepth_Label);

            // alphaBetaDepth_ComboBox
            //
            alphaBetaDepth_ComboBox = new ComboBox();

            alphaBetaDepth_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            alphaBetaDepth_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            alphaBetaDepth_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            alphaBetaDepth_ComboBox.TextChanged += new EventHandler(alphaBetaDepth_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(alphaBetaDepth_ComboBox);

            // alphaBetaOrderThreshold_Label
            //
            alphaBetaOrderThreshold_Label = new Label();

            alphaBetaOrderThreshold_Label.Text = "Alpha-Beta Order Threshold";
            alphaBetaOrderThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            alphaBetaOrderThreshold_Label.Anchor = AnchorStyles.None;
            alphaBetaOrderThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(alphaBetaOrderThreshold_Label);

            // alphaBetaOrderThreshold_ComboBox
            //
            alphaBetaOrderThreshold_ComboBox = new ComboBox();

            alphaBetaOrderThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            alphaBetaOrderThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            alphaBetaOrderThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            alphaBetaOrderThreshold_ComboBox.TextChanged += new EventHandler(alphaBetaOrderThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(alphaBetaOrderThreshold_ComboBox);

            // alphaBetaTranspositionThreshold_Label
            //
            alphaBetaTranspositionThreshold_Label = new Label();

            alphaBetaTranspositionThreshold_Label.Text = "Alpha-Beta Transposition Threshold";
            alphaBetaTranspositionThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            alphaBetaTranspositionThreshold_Label.Anchor = AnchorStyles.None;
            alphaBetaTranspositionThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(alphaBetaTranspositionThreshold_Label);

            // alphaBetaTranspositionThreshold_ComboBox
            //
            alphaBetaTranspositionThreshold_ComboBox = new ComboBox();

            alphaBetaTranspositionThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            alphaBetaTranspositionThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            alphaBetaTranspositionThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            alphaBetaTranspositionThreshold_ComboBox.TextChanged += new EventHandler(alphaBetaTranspositionThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(alphaBetaTranspositionThreshold_ComboBox);

            // alphaBetaRandom_Label
            //
            alphaBetaRandom_Label = new Label();

            alphaBetaRandom_Label.Text = "Alpha-Beta Random";
            alphaBetaRandom_Label.TextAlign = ContentAlignment.MiddleCenter;
            alphaBetaRandom_Label.Anchor = AnchorStyles.None;
            alphaBetaRandom_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(alphaBetaRandom_Label);

            // alphaBetaRandom_CheckBox
            //
            alphaBetaRandom_CheckBox = new CheckBox();

            alphaBetaRandom_CheckBox.Checked = false;

            alphaBetaRandom_CheckBox.CheckedChanged += new EventHandler(alphaBetaRandom_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(alphaBetaRandom_CheckBox);

            // iterativeDeepeningDepth_Label
            //
            iterativeDeepeningDepth_Label = new Label();

            iterativeDeepeningDepth_Label.Text = "Iterative-Deepening Depth";
            iterativeDeepeningDepth_Label.TextAlign = ContentAlignment.MiddleCenter;
            iterativeDeepeningDepth_Label.Anchor = AnchorStyles.None;
            iterativeDeepeningDepth_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningDepth_Label);

            // iterativeDeepeningDepth_ComboBox
            //
            iterativeDeepeningDepth_ComboBox = new ComboBox();

            iterativeDeepeningDepth_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            iterativeDeepeningDepth_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            iterativeDeepeningDepth_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            iterativeDeepeningDepth_ComboBox.TextChanged += new EventHandler(iterativeDeepeningDepth_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningDepth_ComboBox);

            // iterativeDeepeningOrderThreshold_Label
            //
            iterativeDeepeningOrderThreshold_Label = new Label();

            iterativeDeepeningOrderThreshold_Label.Text = "Iterative-Deepening Order Threshold";
            iterativeDeepeningOrderThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            iterativeDeepeningOrderThreshold_Label.Anchor = AnchorStyles.None;
            iterativeDeepeningOrderThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningOrderThreshold_Label);

            // iterativeDeepeningOrderThreshold_ComboBox
            //
            iterativeDeepeningOrderThreshold_ComboBox = new ComboBox();

            iterativeDeepeningOrderThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            iterativeDeepeningOrderThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            iterativeDeepeningOrderThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            iterativeDeepeningOrderThreshold_ComboBox.TextChanged += new EventHandler(iterativeDeepeningOrderThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningOrderThreshold_ComboBox);

            // iterativeDeepeningTranspositionThreshold_Label
            //
            iterativeDeepeningTranspositionThreshold_Label = new Label();

            iterativeDeepeningTranspositionThreshold_Label.Text = "Iterative-Deepening Transposition Threshold";
            iterativeDeepeningTranspositionThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            iterativeDeepeningTranspositionThreshold_Label.Anchor = AnchorStyles.None;
            iterativeDeepeningTranspositionThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningTranspositionThreshold_Label);

            // iterativeDeepeningTranspositionThreshold_ComboBox
            //
            iterativeDeepeningTranspositionThreshold_ComboBox = new ComboBox();

            iterativeDeepeningTranspositionThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 10).Select(i => (object)i).ToArray());
            iterativeDeepeningTranspositionThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            iterativeDeepeningTranspositionThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            iterativeDeepeningTranspositionThreshold_ComboBox.TextChanged += new EventHandler(iterativeDeepeningTranspositionThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningTranspositionThreshold_ComboBox);

            // iterativeDeepeningRandom_Label
            //
            iterativeDeepeningRandom_Label = new Label();

            iterativeDeepeningRandom_Label.Text = "Iterative-Deepening Random";
            iterativeDeepeningRandom_Label.TextAlign = ContentAlignment.MiddleCenter;
            iterativeDeepeningRandom_Label.Anchor = AnchorStyles.None;
            iterativeDeepeningRandom_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningRandom_Label);

            // iterativeDeepeningRandom_CheckBox
            //
            iterativeDeepeningRandom_CheckBox = new CheckBox();

            iterativeDeepeningRandom_CheckBox.Checked = false;

            iterativeDeepeningRandom_CheckBox.CheckedChanged += new EventHandler(iterativeDeepeningRandom_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(iterativeDeepeningRandom_CheckBox);

            // basePlayer_Label
            //
            basePlayer_Label = new Label();

            basePlayer_Label.Text = "Solver Base Player";
            basePlayer_Label.TextAlign = ContentAlignment.MiddleCenter;
            basePlayer_Label.Anchor = AnchorStyles.None;
            basePlayer_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(basePlayer_Label);

            // basePlayer_ComboBox
            //
            basePlayer_ComboBox = new ComboBox();

            basePlayer_ComboBox.ValueMember = "Value";
            basePlayer_ComboBox.DisplayMember = "Display";
            basePlayer_ComboBox.DataSource = ValueDisplayDataTable(basePlayer_ComboBox.ValueMember, basePlayer_ComboBox.DisplayMember, new object[,] { 
            { new RandomPlayer(),                                 "Random" },
            { new MonteCarloTreeSearchPlayer(),                   "Monte-Carlo" },
            { new AlphaBetaTranspositionPlayer(),                 "Alpha-Beta Transposition" },
            { new AlphaBetaMTDfPlayer(),                          "Alpha-Beta MTDf" },
            { new IterativeDeepeningTranspositionPlayer(),        "Iterative-Deepening Transposition" },
            { new IterativeDeepeningMTDfPlayer(),                 "Iterative-Deepening MTDf" },
            });
            basePlayer_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            basePlayer_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            basePlayer_ComboBox.SelectionChangeCommitted += new EventHandler(basePlayer_ComboBox_SelectionChangeCommitted);

            configuration_TableLayoutPanel.Controls.Add(basePlayer_ComboBox);

            // solverDepth_Label
            //
            solverDepth_Label = new Label();

            solverDepth_Label.Text = "Solver Depth";
            solverDepth_Label.TextAlign = ContentAlignment.MiddleCenter;
            solverDepth_Label.Anchor = AnchorStyles.None;
            solverDepth_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(solverDepth_Label);

            // solverDepth_ComboBox
            //
            solverDepth_ComboBox = new ComboBox();

            solverDepth_ComboBox.Items.AddRange(Enumerable.Range(1, 20).Select(i => (object)i).ToArray());
            solverDepth_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            solverDepth_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            solverDepth_ComboBox.TextChanged += new EventHandler(solverDepth_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(solverDepth_ComboBox);

            // solverOrderThreshold_Label
            //
            solverOrderThreshold_Label = new Label();

            solverOrderThreshold_Label.Text = "Solver Order Threshold";
            solverOrderThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            solverOrderThreshold_Label.Anchor = AnchorStyles.None;
            solverOrderThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(solverOrderThreshold_Label);

            // solverOrderThreshold_ComboBox
            //
            solverOrderThreshold_ComboBox = new ComboBox();

            solverOrderThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 20).Select(i => (object)i).ToArray());
            solverOrderThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            solverOrderThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            solverOrderThreshold_ComboBox.TextChanged += new EventHandler(solverOrderThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(solverOrderThreshold_ComboBox);

            // solverTranspositionThreshold_Label
            //
            solverTranspositionThreshold_Label = new Label();

            solverTranspositionThreshold_Label.Text = "Solver Transposition Threshold";
            solverTranspositionThreshold_Label.TextAlign = ContentAlignment.MiddleCenter;
            solverTranspositionThreshold_Label.Anchor = AnchorStyles.None;
            solverTranspositionThreshold_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(solverTranspositionThreshold_Label);

            // solverTranspositionThreshold_ComboBox
            //
            solverTranspositionThreshold_ComboBox = new ComboBox();

            solverTranspositionThreshold_ComboBox.Items.AddRange(Enumerable.Range(1, 20).Select(i => (object)i).ToArray());
            solverTranspositionThreshold_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            solverTranspositionThreshold_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            solverTranspositionThreshold_ComboBox.TextChanged += new EventHandler(solverTranspositionThreshold_ComboBox_TextChanged);

            configuration_TableLayoutPanel.Controls.Add(solverTranspositionThreshold_ComboBox);

            // solverRandom_Label
            //
            solverRandom_Label = new Label();

            solverRandom_Label.Text = "Solver Random";
            solverRandom_Label.TextAlign = ContentAlignment.MiddleCenter;
            solverRandom_Label.Anchor = AnchorStyles.None;
            solverRandom_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(solverRandom_Label);

            // solverRandom_CheckBox
            //
            solverRandom_CheckBox = new CheckBox();

            solverRandom_CheckBox.Checked = false;

            solverRandom_CheckBox.CheckedChanged += new EventHandler(solverRandom_CheckBox_CheckedChanged);

            configuration_TableLayoutPanel.Controls.Add(solverRandom_CheckBox);

            all_TableLayoutPanel.Controls.Add(configuration_TableLayoutPanel);

            // clear_Label
            //
            clear_Label = new Label();

            clear_Label.Text = "Clear Memory";
            clear_Label.TextAlign = ContentAlignment.MiddleCenter;
            clear_Label.Anchor = AnchorStyles.None;
            clear_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(clear_Label);

            // clear_Button
            //
            clear_Button = new Button();

            clear_Button.Text = "Clear Memory";
            clear_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clear_Button.TextAlign = ContentAlignment.MiddleCenter;

            clear_Button.Click += new EventHandler(clear_Button_Click);

            configuration_TableLayoutPanel.Controls.Add(clear_Button);

            // input_Label
            //
            input_Label = new Label();

            input_Label.Text = "Input Moves";
            input_Label.TextAlign = ContentAlignment.MiddleCenter;
            input_Label.Anchor = AnchorStyles.None;
            input_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(input_Label);

            // input_Button
            //
            input_Button = new Button();

            input_Button.Text = "Input Moves";
            input_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            input_Button.TextAlign = ContentAlignment.MiddleCenter;

            input_Button.Click += new EventHandler(input_Button_Click);

            configuration_TableLayoutPanel.Controls.Add(input_Button);

            // output_Label
            //
            output_Label = new Label();

            output_Label.Text = "Output Moves";
            output_Label.TextAlign = ContentAlignment.MiddleCenter;
            output_Label.Anchor = AnchorStyles.None;
            output_Label.AutoSize = true;

            configuration_TableLayoutPanel.Controls.Add(output_Label);

            // output_Button
            //
            output_Button = new Button();

            output_Button.Text = "Output Moves";
            output_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            output_Button.TextAlign = ContentAlignment.MiddleCenter;

            output_Button.Click += new EventHandler(output_Button_Click);

            configuration_TableLayoutPanel.Controls.Add(output_Button);

            // about_Label
            //
            about_Label = new Label();

            about_Label.Text = "About GeneralReversi";
            about_Label.TextAlign = ContentAlignment.MiddleCenter;
            about_Label.Anchor = AnchorStyles.None;
            about_Label.AutoSize = true;

            //configuration_TableLayoutPanel.Controls.Add(about_Label);

            // about_Button
            //
            about_Button = new Button();

            about_Button.Text = "About GeneralReversi";
            about_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            about_Button.TextAlign = ContentAlignment.MiddleCenter;

            about_Button.Click += new EventHandler(about_Button_Click);

            //configuration_TableLayoutPanel.Controls.Add(about_Button);

            //
            // boardAndLog_TabControl
            //
            boardAndLog_TabControl = new TabControl();

            boardAndLog_TabControl.Size = new Size((int)(WidthRatioBoardAndLogToAll * all_TableLayoutPanel.Width), (int)(HeightRatioBoardAndLogToAll * all_TableLayoutPanel.Height));
            boardAndLog_TabControl.Anchor = AnchorStyles.None;

            board_TabPage = new TabPage()
            {
                Text = "Board"
            };

            boardAndLog_TabControl.Controls.Add(board_TabPage);

            log_TabPage = new TabPage()
            {
                Text = "Log"
            };

            boardAndLog_TabControl.Controls.Add(log_TabPage);

            all_TableLayoutPanel.Controls.Add(boardAndLog_TabControl);

            Controls.Add(all_TableLayoutPanel);

            // board_TableLayoutPanel
            //
            board_TableLayoutPanel = new TableLayoutPanel();

            board_TableLayoutPanel.Size = board_TabPage.Size;
            board_TableLayoutPanel.RowCount = 5;
            board_TableLayoutPanel.ColumnCount = 1;
            board_TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            board_TableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // board_PictureBox
            board_PictureBox = new PictureBox();

            side = (int)(RatioBoard * board_TableLayoutPanel.Width);
            board_PictureBox.Size = new Size(side, side);
            board_PictureBox.Anchor = AnchorStyles.None;

            board_PictureBox.Click += new EventHandler(board_PictureBox_Click);

            board_TableLayoutPanel.Controls.Add(board_PictureBox);

            // information_PictureBox
            information_PictureBox = new PictureBox();

            information_PictureBox.Size = new Size(side, side / 4);
            information_PictureBox.Anchor = AnchorStyles.None;

            board_TableLayoutPanel.Controls.Add(information_PictureBox);

            // phase_ExtendedLabel
            phase_ExtendedLabel = new ExtendedLabel();

            phase_ExtendedLabel.Size = new Size(side, side / 8);
            phase_ExtendedLabel.BackColor = Color.Yellow;
            phase_ExtendedLabel.BorderColor = Color.Orange;
            phase_ExtendedLabel.TextAlign = ContentAlignment.MiddleCenter;
            phase_ExtendedLabel.Font = new Font("Century", side / 14);
            phase_ExtendedLabel.Anchor = AnchorStyles.None;

            board_TableLayoutPanel.Controls.Add(phase_ExtendedLabel);

            // text_ExtendedLabel
            text_ExtendedLabel = new ExtendedLabel();

            text_ExtendedLabel.Size = new Size(side, side / 8);
            text_ExtendedLabel.BackColor = Color.LightPink;
            text_ExtendedLabel.BorderColor = Color.DeepPink;
            text_ExtendedLabel.TextAlign = ContentAlignment.MiddleCenter;
            text_ExtendedLabel.Font = new Font("Century", side / 14);
            text_ExtendedLabel.Anchor = AnchorStyles.None;

            board_TableLayoutPanel.Controls.Add(text_ExtendedLabel);

            // _ProgressBar
            _ProgressBar = new ProgressBar();

            _ProgressBar.Size = new Size(side, side / 8);
            _ProgressBar.Anchor = AnchorStyles.None;

            board_TableLayoutPanel.Controls.Add(_ProgressBar);

            board_TabPage.Controls.Add(board_TableLayoutPanel);

            // log_TextBox
            //
            log_TextBox = new TextBox();

            log_TextBox.Size = log_TabPage.Size;
            log_TextBox.BackColor = Color.GreenYellow;
            log_TextBox.ScrollBars = ScrollBars.Both;
            log_TextBox.Multiline = true;
            log_TextBox.WordWrap = false;
            log_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            log_TextBox.ReadOnly = true;

            log_TabPage.Controls.Add(log_TextBox);

            if (log)
            {
                boardAndLog_TabControl.SelectedTab = log_TabPage;
            }

            //
            // before InitializeReversi()
            //
            blackPlayer_ComboBox.SelectedIndex = 1;
            whitePlayer_ComboBox.SelectedIndex = 1;
            delay_ComboBox.SelectedIndex = 0;
            iteration_ComboBox.SelectedIndex = 0;
            evaluationFunction_ComboBox.SelectedIndex = 3;
            alphaBetaDepth_ComboBox.SelectedIndex = 0;
            alphaBetaOrderThreshold_ComboBox.SelectedIndex = 0;
            alphaBetaTranspositionThreshold_ComboBox.SelectedIndex = 0;
            iterativeDeepeningDepth_ComboBox.SelectedIndex = 0;
            iterativeDeepeningOrderThreshold_ComboBox.SelectedIndex = 0;
            iterativeDeepeningTranspositionThreshold_ComboBox.SelectedIndex = 0;
            basePlayer_ComboBox.SelectedIndex = 0;
            solverDepth_ComboBox.SelectedIndex = 0;
            solverOrderThreshold_ComboBox.SelectedIndex = 0;
            solverTranspositionThreshold_ComboBox.SelectedIndex = 0;
            Resize += new EventHandler(Form1_Resize);
            FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            //
            // InitializeReversi()
            //
            InitializeReversi();

            ResumeLayout(false);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            all_TableLayoutPanel.Visible = false;

            int width;
            int height;
            
            if ((double)ClientSize.Height / ClientSize.Width > RatioHeightToWidth)
            {
                width = ClientSize.Width;
                height = (int)(RatioHeightToWidth * width);
                all_TableLayoutPanel.Location = new Point(0, (ClientSize.Height - height) / 2);
            }
            else
            {
                height = ClientSize.Height;
                width = (int)(height / RatioHeightToWidth);
                all_TableLayoutPanel.Location = new Point((ClientSize.Width - width) / 2, 0);
            }

            all_TableLayoutPanel.Size = new Size(width, height);

            configuration_TableLayoutPanel.Size = new Size((int)(WidthRatioConfigurationToAll * all_TableLayoutPanel.Width), (int)(HeightRatioConfigurationToAll * all_TableLayoutPanel.Height));

            configuration_TableLayoutPanel.RowStyles.Clear();

            for (int i = 0; i < configuration_TableLayoutPanel.RowCount; i++)
            {
                configuration_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, configuration_TableLayoutPanel.Height / configuration_TableLayoutPanel.RowCount - 1));
            }

            configuration_TableLayoutPanel.Height = (configuration_TableLayoutPanel.Height / configuration_TableLayoutPanel.RowCount) * configuration_TableLayoutPanel.RowCount;

            boardAndLog_TabControl.Size = new Size((int)(WidthRatioBoardAndLogToAll * all_TableLayoutPanel.Width), (int)(HeightRatioBoardAndLogToAll * all_TableLayoutPanel.Height));

            TabPage tmp = boardAndLog_TabControl.SelectedTab;
            boardAndLog_TabControl.SelectedTab = board_TabPage;
            boardAndLog_TabControl.SelectedTab = log_TabPage;
            boardAndLog_TabControl.SelectedTab = tmp;

            board_TableLayoutPanel.Size = board_TabPage.Size;

            side = (int)(RatioBoard * board_TableLayoutPanel.Width);
            board_PictureBox.Size = new Size(side, side);
            board_PictureBox.Image = BoardImage(boardForResize);

            information_PictureBox.Size = new Size(side, side / 4);
            information_PictureBox.Image = InformationImage(boardForResize, turn);

            phase_ExtendedLabel.Size = new Size(side, side / 8);
            phase_ExtendedLabel.Font = new Font("Century", side / 14);

            text_ExtendedLabel.Size = new Size(side, side / 8);
            text_ExtendedLabel.Font = new Font("Century", side / 14);

            _ProgressBar.Size = new Size(side, side / 8);

            log_TextBox.Size = log_TabPage.Size;

            size_ComboBox.SelectionLength = 0;
            delay_ComboBox.SelectionLength = 0;
            seed_ComboBox.SelectionLength = 0;
            iteration_ComboBox.SelectionLength = 0;
            alphaBetaDepth_ComboBox.SelectionLength = 0;
            alphaBetaOrderThreshold_ComboBox.SelectionLength = 0;
            alphaBetaTranspositionThreshold_ComboBox.SelectionLength = 0;
            iterativeDeepeningDepth_ComboBox.SelectionLength = 0;
            iterativeDeepeningOrderThreshold_ComboBox.SelectionLength = 0;
            iterativeDeepeningTranspositionThreshold_ComboBox.SelectionLength = 0;
            solverDepth_ComboBox.SelectionLength = 0;
            solverOrderThreshold_ComboBox.SelectionLength = 0;
            solverTranspositionThreshold_ComboBox.SelectionLength = 0;

            all_TableLayoutPanel.Visible = true;
        }

        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            if (thread != null)
            {
                thread.Abort();
            }

            if (thread2 != null)
            {
                thread2.Abort();
            }
        }
        
        public static DataTable ValueDisplayDataTable(string value, string display, object[,] array)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(value, typeof(object));
            dataTable.Columns.Add(display, typeof(object));

            for (int i = 0; i < array.GetLength(0); i++)
            {
                DataRow dataRow = dataTable.NewRow();

                dataRow[value] = array[i, 0];
                dataRow[display] = array[i, 1];

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private string StartText()
        {
            return "Let's Start Reversi!";
        }

        private string MiddleText()
        {
            return turn + " Moves " + Board.PositionString(linkedPosition.position);
        }

        private string EndText()
        {
            string ret;

            int sign = Math.Sign(board.score);
            switch (sign)
            {
                case 1:
                    ret = State.Black + " Wins!";

                    break;
                case -1:
                    ret = State.White + " Wins!";

                    break;
                default:
                    ret = "Draw!";

                    break;
            }

            return ret;
        }

        private string PassText()
        {
            return turn + " Passes";
        }

        private string ScoreText()
        {
            return "Black " + board.countBlack + "\t" + "White " + board.countWhite;
        }

        private string PhaseText()
        {
            return "Phase " + phase;
        }

        private void InitializeReversi()
        {
            if (thread != null)
            {
                thread.Abort();
            }

            if (thread2 != null)
            {
                thread2.Abort();
            }

            size = int.Parse(size_ComboBox.Text);
            pattern = (Pattern)pattern_ComboBox.SelectedItem;
            phase = 0;
            turn = State.Black;
            board = new Board(size, pattern);
            boardForResize = board.Clone();
            players = new Dictionary<State, Player>();
            players[State.Black] = (Player)blackPlayer_ComboBox.SelectedValue;
            players[State.White] = (Player)whitePlayer_ComboBox.SelectedValue;
            linkedPosition = LinkedPosition.Out;
            linkedPositionByHuman = LinkedPosition.Out;
            seed_ComboBox.Items[0] = Environment.TickCount;
            seed = int.Parse(seed_ComboBox.Text);
            random = new Random(seed);
            start_CheckBox.Checked = false;
            history_ComboBox.Items.Clear();
            history_ComboBox.Items.Add(phase);
            iteration = int.Parse(iteration_ComboBox.Text);
            alphaBetaDepth = int.Parse(alphaBetaDepth_ComboBox.Text);
            evaluationFunction = (EvaluationFunction)evaluationFunction_ComboBox.SelectedValue;
            iterativeDeepeningDepth = int.Parse(iterativeDeepeningDepth_ComboBox.Text);
            basePlayer = (Player)basePlayer_ComboBox.SelectedValue;
            movablePositions = board.MovablePositions(turn);

            board_PictureBox.Image = BoardImage(board);
            information_PictureBox.Image = InformationImage(board, turn);
            phase_ExtendedLabel.Text = PhaseText();
            text_ExtendedLabel.Text = StartText();
            _ProgressBar.Value = 0;
            _ProgressBar.Style = ProgressBarStyle.Blocks;
            log_TextBox.Text = StartText() + Environment.NewLine + Environment.NewLine;

            start_CheckBox.Enabled = true;
        }

        private void StartReversi(List<Position> positions = null)
        {
            for (int p = history_ComboBox.Items.Count - 1; p > phase; p--)
            {
                history_ComboBox.Items.RemoveAt(p);
            }

            int index = 0;
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            stopwatch.Start();

            Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("Start Date-Time = " + DateTime.Now + Environment.NewLine + Environment.NewLine);
            }));
            
            while (start_CheckBox.Checked || positions != null)
            {
                State notTurn = turn.Not();

                if (movablePositions.Length != 0)
                {
                    if (positions == null)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            log_TextBox.AppendText("movablePositions = " + movablePositions.Select(lp => Board.PositionString(lp.position)).ToStringExtension() + Environment.NewLine);
                        }));

                        stopwatch2.Restart();
                        linkedPosition = players[turn].Choose(board, turn);
                        stopwatch2.Stop();

                        Invoke((MethodInvoker)(() =>
                        {
                            Form1.log_TextBox.AppendText("Time [ms] = " + stopwatch2.ElapsedMilliseconds + Environment.NewLine);
                        }));
                    }
                    else
                    {
                        if (index == positions.Count)
                        {
                            break;
                        }

                        Invoke((MethodInvoker)(() =>
                        {
                            log_TextBox.AppendText("movablePositions = " + movablePositions.Select(lp => Board.PositionString(lp.position)).ToStringExtension() + Environment.NewLine);
                        }));

                        Position position = positions[index++];
                        linkedPosition = Array.Find(movablePositions, lp => lp.position == position);

                        if (linkedPosition == null)
                        {
                            Form2 message = new Form2("Message", true, "Invalid Move: " + Board.PositionString(position), true);

                            Invoke((MethodInvoker)(() =>
                            {
                                message.ShowDialog(this);
                            }));

                            message.Dispose();

                            return;
                        }
                    }

                    if (!start_CheckBox.Checked && positions == null)
                    {
                        break;
                    }

                    linkedPosition = Array.Find(movablePositions, lp => lp.position == linkedPosition.position);
                    board.Move(turn, linkedPosition);

                    Invoke((MethodInvoker)(() =>
                    {
                        text_ExtendedLabel.Text = MiddleText();
                        log_TextBox.AppendText(MiddleText() + Environment.NewLine);
                    }));
                }
                else
                {
                    if (!board.CanMove(notTurn))
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            text_ExtendedLabel.Text = EndText();
                            log_TextBox.AppendText(EndText() + Environment.NewLine + Environment.NewLine);
                            start_CheckBox.Checked = false;
                        }));

                        break;
                    }

                    Invoke((MethodInvoker)(() =>
                    {
                        text_ExtendedLabel.Text = PassText();
                        log_TextBox.AppendText(PassText() + Environment.NewLine);
                    }));

                    if (pass_CheckBox.Checked)
                    {
                        Form2 message = new Form2("Message", true, PassText(), true);

                        Invoke((MethodInvoker)(() =>
                        {
                            message.ShowDialog(this);
                        }));

                        message.Dispose();
                    }
                }

                turn = notTurn;
                phase++;

                movablePositions = board.MovablePositions(turn);
                boardForResize = board.Clone();

                Invoke((MethodInvoker)(() =>
                {
                    phase_ExtendedLabel.Text = PhaseText();
                    log_TextBox.AppendText(ScoreText() + Environment.NewLine);
                    log_TextBox.AppendText(PhaseText() + Environment.NewLine + Environment.NewLine);
                    history_ComboBox.Items.Add(phase);
                    board_PictureBox.Image = BoardImage(board);
                    information_PictureBox.Image = InformationImage(board, turn);
                }));
                
                Thread.Sleep(delay);
            }

            stopwatch.Stop();

            Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("Stop Date-Time = " + DateTime.Now + Environment.NewLine);
                Form1.log_TextBox.AppendText("Time [ms] = " + stopwatch.ElapsedMilliseconds + Environment.NewLine + Environment.NewLine);
            }));
        }

        private void StartReversi()
        {
            StartReversi(null);
        }

        private Bitmap BoardImage(Board board)
        {
            Bitmap image = new Bitmap(board_PictureBox.Width, board_PictureBox.Height);
            Graphics graphics = Graphics.FromImage(image);
            int blockSide = side / Board.size;
            int boardSide = Board.size * blockSide;
            boardOffset = (side - boardSide) / 2;
            int compensation = (side - boardSide) % 2 == 0 ? 1 : 0;
            Size blockSize = new Size(blockSide, blockSide);
            Point[] points;

            graphics.FillRectangle(Brushes.SteelBlue, graphics.VisibleClipBounds);
            points = new Point[] { new Point(side - 1, 0), new Point(0, 0), new Point(0, side - 1) };
            graphics.DrawLines(Pens.Aqua, points);
            points = new Point[] { new Point(side - 1, 0), new Point(side - 1, side - 1), new Point(0, side - 1) };
            graphics.DrawLines(Pens.DarkSlateBlue, points);
            points = new Point[] { new Point(side - boardOffset - 1, boardOffset - 1), new Point(boardOffset - 1, boardOffset - 1), new Point(boardOffset - 1, side - boardOffset - 1) };
            graphics.DrawLines(Pens.DarkSlateBlue, points);
            points = new Point[] { new Point(side - boardOffset - 1 + compensation, boardOffset - 1), new Point(side - boardOffset - 1 + compensation, side - boardOffset - 1 + compensation), new Point(boardOffset - 1, side - boardOffset - 1 + compensation) };
            graphics.DrawLines(Pens.Aqua, points);
            graphics.FillRectangle(Brushes.Green, boardOffset, boardOffset, boardSide, boardSide);

            int opacity = (int)(0.8 * 255);
            Color black = Color.FromArgb(opacity, 0, 0, 0);
            Color white = Color.FromArgb(opacity, 255, 255, 255);
            SolidBrush solidBrushBlack = new SolidBrush(black);
            SolidBrush solidBrushWhite = new SolidBrush(white);

            for (int i = 0; i < Board.size; i++)
            {
                for (int j = 0; j < Board.size; j++)
                {
                    Point blockPoint = new Point(boardOffset + j * blockSide, boardOffset + i * blockSide);
                    Rectangle r = new Rectangle(blockPoint, blockSize);

                    double ratio = 0.7;
                    Rectangle r0 = new Rectangle(new Point(boardOffset + (int)((j + (1 - ratio) / 2) * blockSide), boardOffset + (int)((i + (1 - ratio) / 2) * blockSide)), new Size((int)(ratio * blockSide), (int)(ratio * blockSide)));

                    Position blockPosition = Board.Position(i, j);

                    if(last_CheckBox.Checked && blockPosition == linkedPosition.position)
                    {
                        graphics.FillRectangle(Brushes.Pink, r);
                    }

                    switch (board.array[blockPosition])
                    {
                        case State.Empty:
                            if(movable_CheckBox.Checked && Array.Find(movablePositions, lp => lp.position == blockPosition) != null)
                            {
                                graphics.FillRectangle(Brushes.LightBlue, r);
                            }

                            break;
                        case State.Black:
                            graphics.FillEllipse(solidBrushBlack, r0);
                            graphics.DrawEllipse(Pens.Black, r0);

                            break;
                        case State.White:
                            graphics.FillEllipse(solidBrushWhite, r0);
                            graphics.DrawEllipse(Pens.White, r0);

                            break;
                    }

                    points = new Point[] { new Point(boardOffset + (j + 1) * blockSide - 1, boardOffset + i * blockSide), new Point(boardOffset + j * blockSide, boardOffset + i * blockSide), new Point(boardOffset + j * blockSide, boardOffset + (i + 1) * blockSide - 1) };
                    graphics.DrawLines(Pens.LightGreen, points);
                    points = new Point[] { new Point(boardOffset + (j + 1) * blockSide - 1, boardOffset + i * blockSide), new Point(boardOffset + (j + 1) * blockSide - 1, boardOffset + (i + 1) * blockSide - 1), new Point(boardOffset + j * blockSide, boardOffset + (i + 1) * blockSide - 1) };
                    graphics.DrawLines(Pens.DarkGreen, points);
                }
            }

            graphics.Dispose();

            return image;
        }

        private Bitmap InformationImage(Board board, State turn)
        {
            Bitmap image = new Bitmap(information_PictureBox.Width, information_PictureBox.Height);
            Graphics graphics = Graphics.FromImage(image);
            int s = side / 8;
            int t = s / 2;
            double ratio = 0.7;

            graphics.FillRectangle(Brushes.DeepSkyBlue, graphics.VisibleClipBounds);
            graphics.DrawRectangle(Pens.Blue, new Rectangle(0, 0, information_PictureBox.Width - 1, information_PictureBox.Height - 1));

            int opacity = (int)(0.8 * 255);
            Color black = Color.FromArgb(opacity, 0, 0, 0);
            Color white = Color.FromArgb(opacity, 255, 255, 255);
            SolidBrush solidBrushBlack = new SolidBrush(black);
            SolidBrush solidBrushWhite = new SolidBrush(white);

            graphics.FillEllipse(solidBrushBlack, new Rectangle(new Point((int)((1 + (1 - ratio) / 2) * s), (int)((0 + (1 - ratio) / 2) * s)), new Size((int)(ratio * s), (int)(ratio * s))));
            graphics.DrawEllipse(Pens.Black, new Rectangle(new Point((int)((1 + (1 - ratio) / 2) * s), (int)((0 + (1 - ratio) / 2) * s)), new Size((int)(ratio * s), (int)(ratio * s))));

            graphics.FillEllipse(solidBrushWhite, new Rectangle(new Point((int)((5 + (1 - ratio) / 2) * s), (int)((0 + (1 - ratio) / 2) * s)), new Size((int)(ratio * s), (int)(ratio * s))));
            graphics.DrawEllipse(Pens.White, new Rectangle(new Point((int)((5 + (1 - ratio) / 2) * s), (int)((0 + (1 - ratio) / 2) * s)), new Size((int)(ratio * s), (int)(ratio * s))));

            float emSize = side / 14;
            graphics.DrawString(board.countBlack.ToString(), new Font("Century", emSize), Brushes.Black, 2 * s, 0);
            graphics.DrawString(board.countWhite.ToString(), new Font("Century", emSize), Brushes.Black, 6 * s, 0);

            if (turn == State.Black)
            {
                graphics.FillRectangle(Brushes.HotPink, new Rectangle(new Point((int)((1 + (1 - ratio) / 2) * s), (int)((3 + (1 - ratio) / 2) * t)), new Size((int)(ratio * s), (int)(ratio * t))));
                graphics.DrawRectangle(Pens.Red, new Rectangle(new Point((int)((1 + (1 - ratio) / 2) * s), (int)((3 + (1 - ratio) / 2) * t)), new Size((int)(ratio * s), (int)(ratio * t))));
            }
            else
            {
                graphics.FillRectangle(Brushes.HotPink, new Rectangle(new Point((int)((5 + (1 - ratio) / 2) * s), (int)((3 + (1 - ratio) / 2) * t)), new Size((int)(ratio * s), (int)(ratio * t))));
                graphics.DrawRectangle(Pens.Red, new Rectangle(new Point((int)((5 + (1 - ratio) / 2) * s), (int)((3 + (1 - ratio) / 2) * t)), new Size((int)(ratio * s), (int)(ratio * t))));
            }

            graphics.Dispose();

            return image;
        }

        private void start_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (start_CheckBox.Checked)
            {
                start_CheckBox.Text = "Stop";

                thread = new Thread(StartReversi);
                thread.Start();
            }
            else
            {
                start_CheckBox.Text = "Start";

                lock (board)
                {
                    Monitor.Pulse(board);
                }
            }
        }

        private void reset_Button_Click(object sender, EventArgs e)
        {
            InitializeReversi();
        }
        
        private void size_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                size = int.Parse(size_ComboBox.Text);

                if (size < 2)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                size = (int)size_ComboBox.Items[2];
                size_ComboBox.Text = size_ComboBox.Items[2].ToString();
            }

            InitializeReversi();
        }

        private void pattern_ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            InitializeReversi();
        }

        private void blackPlayer_ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            players[State.Black] = (Player)blackPlayer_ComboBox.SelectedValue;
        }

        private void whitePlayer_ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            players[State.White] = (Player)whitePlayer_ComboBox.SelectedValue;
        }

        private void last_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            board_PictureBox.Image = BoardImage(board);
        }

        private void movable_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            board_PictureBox.Image = BoardImage(board);
        }

        private void pass_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void delay_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                delay = int.Parse(delay_ComboBox.Text);

                if (delay < 0)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                delay = (int)delay_ComboBox.Items[0];
                delay_ComboBox.Text = delay_ComboBox.Items[0].ToString();
            }
        }

        private void history_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (history_ComboBox.SelectedIndex == -1)
            {
                return;
            }

            start_CheckBox.Checked = false;

            while (thread != null && thread.ThreadState != System.Threading.ThreadState.WaitSleepJoin && thread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                Thread.Sleep(100);
            }

            int phase = history_ComboBox.SelectedIndex;

            if (this.phase > phase)
            {
                while (this.phase > phase)
                {
                    turn = turn.Not();

                    this.phase--;
                    MoveInformation moveInformation = board.moveInformations[board.index - 1];

                    if (moveInformation.turn != turn)
                    {
                        continue;
                    }

                    board.Unmove();
                }
            }
            else
            {
                while (this.phase < phase)
                {
                    this.phase++;
                    MoveInformation moveInformation = board.moveInformations[board.index];

                    if (moveInformation.turn != turn)
                    {
                        turn = turn.Not();

                        continue;
                    }

                    board.Move(turn, moveInformation.linkedPosition);

                    turn = turn.Not();
                }
            }

            linkedPosition = board.index == 0 ? LinkedPosition.Out : board.moveInformations[board.index - 1].linkedPosition;
            movablePositions = board.MovablePositions(turn);

            phase_ExtendedLabel.Text = PhaseText();
            board_PictureBox.Image = BoardImage(board);
            information_PictureBox.Image = InformationImage(board, turn);
        }

        private void seed_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                seed = int.Parse(seed_ComboBox.Text);
            }
            catch (FormatException)
            {
                seed = (int)seed_ComboBox.Items[0];
                seed_ComboBox.Text = seed_ComboBox.Items[0].ToString();
            }
            random = new Random(seed);
        }

        private void iteration_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                iteration = int.Parse(iteration_ComboBox.Text);

                if (iteration < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                iteration = (int)iteration_ComboBox.Items[0];
                iteration_ComboBox.Text = iteration_ComboBox.Items[0].ToString();
            }
        }

        private void evaluationFunction_ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            evaluationFunction = (EvaluationFunction)evaluationFunction_ComboBox.SelectedValue;
        }

        private void alphaBetaDepth_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alphaBetaDepth = int.Parse(alphaBetaDepth_ComboBox.Text);

                if (alphaBetaDepth < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                alphaBetaDepth = (int)alphaBetaDepth_ComboBox.Items[0];
                alphaBetaDepth_ComboBox.Text = alphaBetaDepth_ComboBox.Items[0].ToString();
            }
        }

        private void alphaBetaOrderThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alphaBetaOrderThreshold = int.Parse(alphaBetaOrderThreshold_ComboBox.Text);

                if (alphaBetaOrderThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                alphaBetaOrderThreshold = (int)alphaBetaOrderThreshold_ComboBox.Items[0];
                alphaBetaOrderThreshold_ComboBox.Text = alphaBetaOrderThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void alphaBetaTranspositionThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                alphaBetaTranspositionThreshold = int.Parse(alphaBetaTranspositionThreshold_ComboBox.Text);

                if (alphaBetaTranspositionThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                alphaBetaTranspositionThreshold = (int)alphaBetaTranspositionThreshold_ComboBox.Items[0];
                alphaBetaTranspositionThreshold_ComboBox.Text = alphaBetaTranspositionThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void alphaBetaRandom_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void iterativeDeepeningDepth_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                iterativeDeepeningDepth = int.Parse(iterativeDeepeningDepth_ComboBox.Text);

                if (iterativeDeepeningDepth < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                iterativeDeepeningDepth = (int)iterativeDeepeningDepth_ComboBox.Items[0];
                iterativeDeepeningDepth_ComboBox.Text = iterativeDeepeningDepth_ComboBox.Items[0].ToString();
            }
        }

        private void iterativeDeepeningOrderThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                iterativeDeepeningOrderThreshold = int.Parse(iterativeDeepeningOrderThreshold_ComboBox.Text);

                if (iterativeDeepeningOrderThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                iterativeDeepeningOrderThreshold = (int)iterativeDeepeningOrderThreshold_ComboBox.Items[0];
                iterativeDeepeningOrderThreshold_ComboBox.Text = iterativeDeepeningOrderThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void iterativeDeepeningTranspositionThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                iterativeDeepeningTranspositionThreshold = int.Parse(iterativeDeepeningTranspositionThreshold_ComboBox.Text);

                if (iterativeDeepeningTranspositionThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                iterativeDeepeningTranspositionThreshold = (int)iterativeDeepeningTranspositionThreshold_ComboBox.Items[0];
                iterativeDeepeningTranspositionThreshold_ComboBox.Text = iterativeDeepeningTranspositionThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void iterativeDeepeningRandom_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void basePlayer_ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            basePlayer = (Player)basePlayer_ComboBox.SelectedValue;
        }

        private void solverDepth_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                solverDepth = int.Parse(solverDepth_ComboBox.Text);

                if (solverDepth < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                solverDepth = (int)solverDepth_ComboBox.Items[0];
                solverDepth_ComboBox.Text = solverDepth_ComboBox.Items[0].ToString();
            }
        }

        private void solverOrderThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                solverOrderThreshold = int.Parse(solverOrderThreshold_ComboBox.Text);

                if (solverOrderThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                solverOrderThreshold = (int)solverOrderThreshold_ComboBox.Items[0];
                solverOrderThreshold_ComboBox.Text = solverOrderThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void solverTranspositionThreshold_ComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                solverTranspositionThreshold = int.Parse(solverTranspositionThreshold_ComboBox.Text);

                if (solverTranspositionThreshold < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                solverTranspositionThreshold = (int)solverTranspositionThreshold_ComboBox.Items[0];
                solverTranspositionThreshold_ComboBox.Text = solverTranspositionThreshold_ComboBox.Items[0].ToString();
            }
        }

        private void solverRandom_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void clear_Button_Click(object sender, EventArgs e)
        {
            foreach (Player player in players.Values.ToArray().Concat(new Player[] { Form1.basePlayer }))
            {
                if (player is GeneralReversi.AlphaBetaTranspositionPlayer)
                {
                    GeneralReversi.AlphaBetaTranspositionPlayer ancestorPlayer = (GeneralReversi.AlphaBetaTranspositionPlayer)player;
                    int count = ancestorPlayer.transposition.Count;
                    string text = "Clear " + count + " elements for " + player;

                    ancestorPlayer.transposition.Clear();

                    Form2 message = new Form2("Message", true, text, true);

                    message.ShowDialog(this);

                    message.Dispose();
                }
            }
        }

        private void input_Button_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2("Input Moves", false, null, false);
            string text;
            List<Position> positions;

            if (form2.ShowDialog(this) == DialogResult.OK)
            {
                text = form2.textBox_TextBox.Text;

                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(text, @"\<\s*(?<size>\d+)\s*,\s*(?<pattern>\w+)\s*\>");

                if (match.Success)
                {
                    size_ComboBox.Text = match.Groups["size"].Value;

                    switch (match.Groups["pattern"].Value)
                    {
                        case "Cross":
                            pattern = Pattern.Cross;

                            break;
                        case "Parallel":
                            pattern = Pattern.Parallel;

                            break;
                        default:
                            Form2 message = new Form2("Message", true, "Invalid Initial Pattern", true);

                            message.ShowDialog(this);

                            message.Dispose();

                            return;
                    }

                    pattern_ComboBox.SelectedItem = pattern;
                }
                else
                {
                    Form2 message = new Form2("Message", true, "Invalid Board Size", true);

                    message.ShowDialog(this);

                    message.Dispose();

                    return;
                }

                System.Text.RegularExpressions.MatchCollection matchCollection = System.Text.RegularExpressions.Regex.Matches(text, @"\(\s*(?<x>\d+)\s*,\s*(?<y>\d+)\s*\)");
                positions = new List<Position>(matchCollection.Count);
                
                foreach (System.Text.RegularExpressions.Match m in matchCollection)
                {
                    Position position = Board.Position(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value));
                    positions.Add(position);
                }

                InitializeReversi();

                thread2 = new Thread(() =>
                    {
                        Form1.log_TextBox.Invoke((MethodInvoker)(() =>
                        {
                            start_CheckBox.Enabled = false;
                        }));

                        StartReversi(positions);

                        Form1.log_TextBox.Invoke((MethodInvoker)(() =>
                        {
                            start_CheckBox.Enabled = true;
                        }));
                    });

                thread2.Start();
            }

            form2.Dispose();
        }

        private void output_Button_Click(object sender, EventArgs e)
        {
            string text = "<" + size + ", " + pattern + ">" + Environment.NewLine;
            State turn = State.Black;
            Dictionary<State, string> separators = new Dictionary<State, string>()
            {
                {State.Black, "\t"},
                {State.White, Environment.NewLine},
            };

            for (int i = 0; i < board.index; i++)
            {
                if (board.moveInformations[i].turn != turn)
                {
                    text += "Pass" + separators[turn];
                    turn = turn.Not();
                }

                text += Board.PositionString(board.moveInformations[i].linkedPosition.position) + separators[turn];
                turn = turn.Not();
            }

            Form2 form2 = new Form2("Output Moves", true, text, true);
            
            form2.ShowDialog(this);

            form2.Dispose();
        }

        private void about_Button_Click(object sender, EventArgs e)
        {
        }

        private void board_PictureBox_Click(object sender, EventArgs e)
        {
            if (players[turn] is HumanPlayer && start_CheckBox.Checked)
            {
                int blockSide = board_PictureBox.Width / Board.size;
                Point p = PointToClient(Cursor.Position);

                p.X = (p.X - all_TableLayoutPanel.Location.X - boardAndLog_TabControl.Location.X - board_TabPage.Location.X - board_PictureBox.Location.X - boardOffset) / blockSide;
                p.Y = (p.Y - all_TableLayoutPanel.Location.Y - boardAndLog_TabControl.Location.Y - board_TabPage.Location.Y - board_PictureBox.Location.Y - boardOffset) / blockSide;

                Position position = Board.Position(p.Y, p.X);
                linkedPositionByHuman = Array.Find(movablePositions, linkedPosition => linkedPosition.position == position);

                lock (board)
                {
                    Monitor.Pulse(board);
                }
            }
        }
    }

    public sealed class HumanPlayer : GeneralReversi.HumanPlayer
    {
        public override LinkedPosition Choose(Board board, State turn)
        {
            LinkedPosition ret = LinkedPosition.Out;

            while (Form1.start_CheckBox.Checked)
            {
                if (Form1.movablePositions.Contains(Form1.linkedPositionByHuman))
                {
                    ret = Form1.linkedPositionByHuman;
                    Form1.linkedPositionByHuman = LinkedPosition.Out;
                    
                    break;
                }
                else
                {
                    lock (board)
                    {
                        Monitor.Wait(board);
                    }
                }
            }
            
            return ret;
        }
    }

    public sealed class RandomPlayer : GeneralReversi.RandomPlayer
    {
        public override LinkedPosition Choose(Board board, State turn)
        {
            int index = Form1.random.Next(Form1.movablePositions.Length);

            return Form1.movablePositions[index];
        }
    }

    public sealed class MonteCarloTreeSearchPlayer : GeneralReversi.MonteCarloTreeSearchPlayer
    {
        public override void Set()
        {
            movablePositions = Form1.movablePositions;
            iteration = Form1.iteration;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = iteration;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void SetProgress(int value)
        {
            Form1._ProgressBar.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Value = value;
            }));
        }

        public override void WriteLog(MonteCarloTreeNode root)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                string search = "{ ";
                string reward = "{ ";
                string ratio = "{ ";
                string ucb1 = "{ ";

                foreach (MonteCarloTreeNode child in root.children)
                {
                    search += child.search + " ";
                    reward += child.reward + " ";
                    ratio += child.reward / child.search + " ";
                    ucb1 += child.ucb1 + " ";
                }

                search += "}";
                reward += "}";
                ratio += "}";
                ucb1 += "}";

                Form1.log_TextBox.AppendText("search = " + search + Environment.NewLine);
                Form1.log_TextBox.AppendText("reward = " + reward + Environment.NewLine);
                Form1.log_TextBox.AppendText("ratio = " + ratio + Environment.NewLine);
                Form1.log_TextBox.AppendText("ucb1 = " + ucb1 + Environment.NewLine);
            }));
        }
    }

    public sealed class AlphaBetaPlayer : GeneralReversi.AlphaBetaPlayer
    {
        public override void Set()
        {
            evaluationFunction = Form1.evaluationFunction;
            depth = Form1.alphaBetaDepth;
            orderThreshold = Form1.alphaBetaOrderThreshold;
            doRandom = Form1.alphaBetaRandom_CheckBox.Checked;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = Form1.movablePositions.Length;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void WriteLog()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("evaluation = " + countEvaluation + Environment.NewLine);
            }));
        }

        public override void WriteLogAndSetProgress(int order, int value)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("value[" + order + "] = " + value + Environment.NewLine);
                Form1._ProgressBar.Value = ++ctr;
            }));
        }
    }

    public sealed class AlphaBetaTranspositionPlayer : GeneralReversi.AlphaBetaTranspositionPlayer
    {
        public override void Set()
        {
            evaluationFunction = Form1.evaluationFunction;
            depth = Form1.alphaBetaDepth;
            orderThreshold = Form1.alphaBetaOrderThreshold;
            transpositionThreshold = Form1.alphaBetaTranspositionThreshold;
            doRandom = Form1.alphaBetaRandom_CheckBox.Checked;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = Form1.movablePositions.Length;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void WriteLog()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("evaluation = " + countEvaluation + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionUse = " + countTranspositionUse + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionCount = " + transposition.Count + Environment.NewLine);
            }));
        }

        public override void WriteLogAndSetProgress(int order, int value)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("value[" + order + "] = " + value + Environment.NewLine);
                Form1._ProgressBar.Value = ++ctr;
            }));
        }
    }

    public sealed class AlphaBetaMTDfPlayer : GeneralReversi.AlphaBetaMTDfPlayer
    {
        public override void Set()
        {
            evaluationFunction = Form1.evaluationFunction;
            depth = Form1.alphaBetaDepth;
            orderThreshold = Form1.alphaBetaOrderThreshold;
            transpositionThreshold = Form1.alphaBetaTranspositionThreshold;
            doRandom = Form1.alphaBetaRandom_CheckBox.Checked;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Style = ProgressBarStyle.Marquee;
            }));
        }

        public override void WriteLog()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("evaluation = " + countEvaluation + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionUse = " + countTranspositionUse + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionCount = " + transposition.Count + Environment.NewLine);
                Form1._ProgressBar.Style = ProgressBarStyle.Blocks;
            }));
        }

        public override void WriteLogAndSetProgress(int countLoop, int lowerBound, int upperBound, Position position)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("loop = " + countLoop + "\t" + "lowerBound = " + lowerBound + "\t" + "upperBound = " + upperBound + "\t" + "position = " + Board.PositionString(position) + Environment.NewLine);
            }));
        }
    }

    public sealed class IterativeDeepeningTranspositionPlayer : GeneralReversi.IterativeDeepeningTranspositionPlayer
    {
        public override void Set()
        {
            evaluationFunction = Form1.evaluationFunction;
            depth = Form1.iterativeDeepeningDepth;
            orderThreshold = Form1.iterativeDeepeningOrderThreshold;
            transpositionThreshold = Form1.iterativeDeepeningTranspositionThreshold;
            doRandom = Form1.iterativeDeepeningRandom_CheckBox.Checked;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = Form1.iterativeDeepeningDepth;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void WriteLogAndSetProgress(int depth, ulong countEvaluation, int countTranspositionUse, int transpositionCount, int value, Position position)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("depth = " + depth + "\t" + "evaluation = " + countEvaluation + "\t" + "transpositionUse = " + countTranspositionUse + "\t" + "transpositionCount = " + transpositionCount + "\t" + "value = " + value + "\t" + "position = " + Board.PositionString(position) + Environment.NewLine);

                Form1._ProgressBar.Value = ++ctr;
            }));
        }
    }

    public sealed class IterativeDeepeningMTDfPlayer : GeneralReversi.IterativeDeepeningMTDfPlayer
    {
        public override void Set()
        {
            evaluationFunction = Form1.evaluationFunction;
            depth = Form1.iterativeDeepeningDepth;
            orderThreshold = Form1.iterativeDeepeningOrderThreshold;
            transpositionThreshold = Form1.iterativeDeepeningTranspositionThreshold;
            doRandom = Form1.iterativeDeepeningRandom_CheckBox.Checked;
            random = Form1.random;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = Form1.iterativeDeepeningDepth;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void WriteLogAndSetProgress(int depth, ulong countEvaluation, int countTranspositionUse, int transpositionCount, int value, Position position)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("depth = " + depth + "\t" + "evaluation = " + countEvaluation + "\t" + "transpositionUse = " + countTranspositionUse + "\t" + "transpositionCount = " + transpositionCount + "\t" + "value = " + value + "\t" + "position = " + Board.PositionString(position) + Environment.NewLine);

                Form1._ProgressBar.Value = ++ctr;
            }));
        }
    }

    public sealed class SolverTranspositionPlayer : GeneralReversi.SolverTranspositionPlayer
    {
        public override void Set()
        {
            depth = Form1.solverDepth;
            orderThreshold = Form1.solverOrderThreshold;
            transpositionThreshold = Form1.solverTranspositionThreshold;
            doRandom = Form1.solverRandom_CheckBox.Checked;
            random = Form1.random;
            basePlayer = Form1.basePlayer;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Maximum = Form1.movablePositions.Length;
                Form1._ProgressBar.Value = 0;
            }));
        }

        public override void WriteLog()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("evaluation = " + countEvaluation + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionUse = " + countTranspositionUse + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionCount = " + transposition.Count + Environment.NewLine);
            }));
        }

        public override void WriteLogAndSetProgress(int order, int value)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("value[" + order + "] = " + value + Environment.NewLine);
                Form1._ProgressBar.Value = ++ctr;
            }));
        }
    }

    public sealed class SolverMTDfPlayer : GeneralReversi.SolverMTDfPlayer
    {
        public override void Set()
        {
            depth = Form1.solverDepth;
            orderThreshold = Form1.solverOrderThreshold;
            transpositionThreshold = Form1.solverTranspositionThreshold;
            doRandom = Form1.solverRandom_CheckBox.Checked;
            random = Form1.random;
            basePlayer = Form1.basePlayer;
        }

        public override void InitializeProgress()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1._ProgressBar.Style = ProgressBarStyle.Marquee;
            }));
        }

        public override void WriteLog()
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("evaluation = " + countEvaluation + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionUse = " + countTranspositionUse + Environment.NewLine);
                Form1.log_TextBox.AppendText("transpositionCount = " + transposition.Count + Environment.NewLine);
                Form1._ProgressBar.Style = ProgressBarStyle.Blocks;
            }));
        }

        public override void WriteLogAndSetProgress(int countLoop, int lowerBound, int upperBound, Position position)
        {
            Form1.log_TextBox.Invoke((MethodInvoker)(() =>
            {
                Form1.log_TextBox.AppendText("loop = " + countLoop + "\t" + "lowerBound = " + lowerBound + "\t" + "upperBound = " + upperBound + "\t" + "position = " + Board.PositionString(position) + Environment.NewLine);
            }));
        }
    }
}
