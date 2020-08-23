using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EStoreBillReader.Custom
{
    public class ToggleEventArgs : EventArgs
    {
        public ToggleEventArgs(bool check)
        {
            Checked = check;
        }

        public bool Checked { get; }
    }


    /// <summary>
    /// Interaction logic for ToggleButton
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        Thickness LeftSide = new Thickness(-39, 0, 0, 0);
        Thickness RightSide = new Thickness(0, 0, -39, 0);
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125));

        public ToggleButton()
        {
            InitializeComponent();
            Back.Fill = Off;
            Checked = false;
            Dot.Margin = LeftSide;
            Text = "";
            TextSide = Orientation.Right;
        }

        public enum Orientation
        {
            Right, Left
        }

        public bool Checked
        {
            get
            {
                return Back.Fill == On ? true : false;
            }
            set
            {
                if (!value)
                {
                    Back.Fill = Off;
                    Dot.Margin = LeftSide;
                }
                else
                {
                    Back.Fill = On;
                    Dot.Margin = RightSide;
                }
            }
        }
        public string Text { get => Content.Text; set => Content.Text = value; }

        public Orientation TextSide
        {
            get
            {
                return (int)Content.GetValue(Grid.ColumnProperty) == 0 ? Orientation.Left : Orientation.Right;
            }
            set
            {
                if (value == Orientation.Left)
                {
                    Content.SetValue(Grid.ColumnProperty, 0);
                }
                else
                {
                    Content.SetValue(Grid.ColumnProperty, 2);
                }
            }
        }

        public event EventHandler<ToggleEventArgs> IsCheckedChanged;

        protected virtual void OnIsCheckedChanged(ToggleEventArgs e)
        {
            IsCheckedChanged?.Invoke(this, e);
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Checked)
            {
                Back.Fill = On;
                Checked = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Checked = false;
                Dot.Margin = LeftSide;
            }
            OnIsCheckedChanged(new ToggleEventArgs(Checked));
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Checked)
            {
                Back.Fill = On;
                Checked = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Checked = false;
                Dot.Margin = LeftSide;
            }
            OnIsCheckedChanged(new ToggleEventArgs(Checked));
        }
    }
}
