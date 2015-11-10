using System.Drawing;
using System.Windows.Forms;

namespace ExtendedLabelLibrary
{
    public partial class ExtendedLabel: Label
    {
        public ExtendedLabel()
        {
            InitializeComponent();
        }

        public Color borderColor;

        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Pen p = new Pen(borderColor, 1);
            Rectangle r = new Rectangle(DisplayRectangle.Location, new Size(DisplayRectangle.Width - 1, DisplayRectangle.Height - 1));
            e.Graphics.DrawRectangle(p, r);

            p.Dispose();
        }
    }
}
