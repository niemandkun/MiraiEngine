using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiraiEditor
{
    public partial class NumberTextBox : TextBox
    {
        public NumberTextBox()
        {
            InitializeComponent();
        }

        public double ValueDouble
        {
            get
            {
                try
                {
                    return Double.Parse(Text);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public int ValueInt
        {
            get { return Int32.Parse(Text); }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && this.Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }

        public void AppendText(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                Text = String.Empty;
            }
            else
            {
                if (value == "." && Text.IndexOf('.') > -1)
                    return;
                Text += value;
            }
        }
    }
}
