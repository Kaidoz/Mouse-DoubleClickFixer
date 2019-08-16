using System;
using System.Windows.Forms;

namespace Mouse_DoubleClickFixer
{
    public partial class Form1 : Form
    {
        private bool loaded;

        private readonly UserActivityHook uah = new UserActivityHook();

        public Form1()
        {
            InitializeComponent();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Visible)
            {
                Show();
                notifyIcon1.Visible = false;
            }
        }

        private void Button_hide_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.Visible = true;
        }

        private int count = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            uah.OnMouseActivity += (o, args) =>
            {
                if (args.Clicks == 5) label_count.Text = Convert.ToString(count++);
            };
            checkBoxLeft.Checked = Settings._left;
            checkBoxRight.Checked = Settings._right;
            checkBoxAutoRun.Checked = Settings._autorun;
            loaded = true;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            Settings._left = checkBoxLeft.Checked;
            Settings._right = checkBoxRight.Checked;
            Settings._autorun = checkBoxAutoRun.Checked;
            Settings.Save();
        }
    }
}