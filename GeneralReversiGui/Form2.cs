using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeneralReversiGui
{
    public partial class Form2 : Form
    {
        public Form2(string title, bool okOnly, string text, bool readOnly)
        {
            InitializeComponent();
            InitializeComponent2(title, okOnly);
            textBox_TextBox.Text = text;
            textBox_TextBox.ReadOnly = readOnly;

            if (title == "Message")
            {
                textBox_TextBox.Select(0, 0);
                ok_Button.Select();
            }
            else
            {
                textBox_TextBox.ScrollBars = ScrollBars.Vertical;
            }
        }

        public TextBox textBox_TextBox;
        private Button ok_Button;
        private Button cancel_Button;

        private TableLayoutPanel all_TableLayoutPanel;
        private TableLayoutPanel buttons_TableLayoutPanel;

        private void InitializeComponent2(string title, bool okOnly)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            SuspendLayout();

            //
            // Form2
            //
            ClientSize = new Size(400, 200);
            Text = title;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            StartPosition = FormStartPosition.CenterParent;

            // all_TableLayoutPanel
            //
            all_TableLayoutPanel = new TableLayoutPanel();

            all_TableLayoutPanel.Size = ClientSize;
            all_TableLayoutPanel.RowCount = 2;
            all_TableLayoutPanel.ColumnCount = 1;
            all_TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            for (int i = 0; i < all_TableLayoutPanel.RowCount; i++)
            {
                all_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            all_TableLayoutPanel.RowStyles[0] = new RowStyle(SizeType.Percent, 80);
            all_TableLayoutPanel.RowStyles[1] = new RowStyle(SizeType.Percent, 20);

            for (int i = 0; i < all_TableLayoutPanel.ColumnCount; i++)
            {
                all_TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            // moves_TextBox
            textBox_TextBox = new TextBox();

            textBox_TextBox.Multiline = true;
            textBox_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox_TextBox.MaxLength = 0;

            textBox_TextBox.TextChanged += new EventHandler(moves_TextBox_TextChanged);

            all_TableLayoutPanel.Controls.Add(textBox_TextBox);

            // buttons_TableLayoutPanel
            //
            buttons_TableLayoutPanel = new TableLayoutPanel();

            buttons_TableLayoutPanel.RowCount = 1;
            buttons_TableLayoutPanel.ColumnCount = 2;
            buttons_TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            buttons_TableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            for (int i = 0; i < buttons_TableLayoutPanel.RowCount; i++)
            {
                buttons_TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (int i = 0; i < buttons_TableLayoutPanel.ColumnCount; i++)
            {
                buttons_TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            float okPercent = okOnly ? 100 : 50;

            buttons_TableLayoutPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, okPercent);
            buttons_TableLayoutPanel.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100 - okPercent);

            // ok_Button
            ok_Button = new Button();

            ok_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            ok_Button.Text = "OK";
            ok_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ok_Button.TextAlign = ContentAlignment.MiddleCenter;

            ok_Button.Click += new EventHandler(ok_Button_Click);

            buttons_TableLayoutPanel.Controls.Add(ok_Button);

            AcceptButton = ok_Button;

            // cancel_Button
            cancel_Button = new Button();

            cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancel_Button.Text = "Cancel";
            cancel_Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cancel_Button.TextAlign = ContentAlignment.MiddleCenter;

            cancel_Button.Click += new EventHandler(cancel_Button_Click);

            buttons_TableLayoutPanel.Controls.Add(cancel_Button);

            CancelButton = cancel_Button;

            all_TableLayoutPanel.Controls.Add(buttons_TableLayoutPanel);

            Controls.Add(all_TableLayoutPanel);

            Resize += new EventHandler(Form2_Resize);

            ResumeLayout(false);
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            all_TableLayoutPanel.Size = ClientSize;
        }

        private void moves_TextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void ok_Button_Click(object sender, EventArgs e)
        {
        }

        private void cancel_Button_Click(object sender, EventArgs e)
        {
        }
    }
}
