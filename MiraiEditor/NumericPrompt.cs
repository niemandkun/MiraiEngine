using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiraiEditor
{
    public static class NumericPrompt
    {
        public static Tuple<decimal, decimal> ShowDialog(string firstLabel, 
            string secondLabel, string caption)
        {
            var dialog = new Form();
            dialog.Width = 250;
            dialog.Height = 180;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MinimizeBox = false;
            dialog.MaximizeBox = false;
            dialog.Text = caption;
            dialog.StartPosition = FormStartPosition.CenterScreen;

            var table = new TableLayoutPanel() 
            { 
                Top = 20,
                Left = 20,
                Width = dialog.ClientSize.Width - 40,
                Height = 60,
            };

            var upperLabel = new Label() { Text = firstLabel };
            var bottomLabel = new Label() { Text = secondLabel };
            var upperInput = new NumericUpDown() { Minimum = 1, Maximum = Int32.MaxValue, Value = 800 };
            var bottomInput = new NumericUpDown() { Minimum = 1, Maximum = Int32.MaxValue, Value = 600 };

            var confirmationButton = new Button() { Text = "OK", Left = 150, Top = 100 };
            confirmationButton.Click += (sender, e) => { dialog.Close(); };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            table.Controls.Add(upperLabel, 0, 0);
            table.Controls.Add(bottomLabel, 0, 1);
            table.Controls.Add(upperInput, 1, 0);
            table.Controls.Add(bottomInput, 1, 1);

            dialog.Controls.Add(table);
            dialog.Controls.Add(confirmationButton);
            dialog.AcceptButton = confirmationButton;
            dialog.ShowDialog();
            
            return Tuple.Create(upperInput.Value, bottomInput.Value);
        }
    }
}
