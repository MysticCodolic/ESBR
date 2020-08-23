using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace EStoreBillReader.Custom
{
    class DateTextBox : TextBox
    {
        public DateTextBox Mask { get; set; }

        public DateTextBox()
        {
            this.TextChanged += new TextChangedEventHandler(DateTextBox_TextChanged);
        }

        void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CaretIndex = this.Text.Length;

            var tbEntry = sender as DateTextBox;

            if (tbEntry != null && tbEntry.Text.Length > 0)
            {
                tbEntry.Text = formatNumber(tbEntry.Text, tbEntry.Mask);
            }
        }

        public static string formatNumber(string MaskedNum, DateTextBox phoneFormat)
        {
            int x;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            if (MaskedNum != null)
            {
                for (int i = 0; i < MaskedNum.Length; i++)
                {
                    if (int.TryParse(MaskedNum.Substring(i, 1), out x))
                    {
                        sb.Append(x.ToString());
                    }
                }
                return FormatForSSN(sb.ToString()).ToString();                                   
            }
            return sb.ToString();
        }
        public static StringBuilder FormatForSSN(String sb)
        {
            StringBuilder sb2 = new StringBuilder();

            if (sb.Length > 0) sb2.Append(sb.Substring(0, 1));
            if (sb.Length > 1) sb2.Append(sb.Substring(1, 1));

            if (sb.Length > 2) sb2.Append("/");

            if (sb.Length > 2) sb2.Append(sb.Substring(2, 1));
            if (sb.Length > 3) sb2.Append(sb.Substring(3, 1));

            if (sb.Length > 4) sb2.Append("/");

            if (sb.Length > 4) sb2.Append(sb.Substring(4, 1));
            if (sb.Length > 5) sb2.Append(sb.Substring(5, 1));
            if (sb.Length > 6) sb2.Append(sb.Substring(6, 1));
            if (sb.Length > 7) sb2.Append(sb.Substring(7, 1));

            return sb2;
        }
   
    }
}
