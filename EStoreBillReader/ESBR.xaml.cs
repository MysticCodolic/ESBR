using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EStoreBillReader
{

    /// <summary>
    /// Interaction logic for ESBR
    /// </summary>
    public partial class ESBR : Window
    {

        public ESBR()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Win10_Blur.EnableBlur(this);
            Defaults();
            if (!Helper.CheckPathExists(Helper.DirPath))
            {
                NewProfile();
            }
            Helper.checkDatabasePath(Helper.MonthNow);
        }

        private void NewProfile()
        {
            MessageBoxResult YesNo = MessageBox.Show("Could not find default directory at: " + Helper.DirPath +
                   "\n Would you like to create.", "Important", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (YesNo == MessageBoxResult.Yes)
            {
                Helper.CreatePath(Helper.DirPath);
            }
            else
            {
                OpenFileDialog fd = new OpenFileDialog()
                {
                    Title = "Select PDF File",
                    Multiselect = true,
                    Filter = "All Files|*",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    ValidateNames = false,
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "This Folder"
                };
                if (fd.ShowDialog() ?? false)
                {
                    MessageBox.Show(Path.GetDirectoryName(fd.FileName).ToString());
                    Helper.DirPath = Path.GetDirectoryName(fd.FileName);
                }
            }
            Helper.CreatePath(Helper.DirPath + "\\" + Helper.MonthNow);
        }
        private void Defaults()
        {

        }
        #region UI Settings


        private void MinMaxBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void CloseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void WinTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

    }
}
