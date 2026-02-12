using System;
using System.Windows.Forms;

namespace 期末テスト_2203024許海翔
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            var text = textBoxInput.Text?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                labelResult.Text = "请输入数字";
                return;
            }

            if (int.TryParse(text, out _))
            {
                labelResult.Text = "整数です";
                return;
            }

            labelResult.Text = "整数じゃない";
        }
    }
}
