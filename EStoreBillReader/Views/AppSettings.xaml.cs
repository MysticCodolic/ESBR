using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
namespace EStoreBillReader.Views
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettings : UserControl
    {
        public AppSettings()
        {
            InitializeComponent();
            DBLocation.Text = Helper.DirPath;
        }
        private void AssignFileDB(OpenFileDialog dialog)
        {
            //con.DataSource = System.IO.Path.GetDirectoryName(dialog.FileName);
            //Helper.FileName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
            //Properties.Settings.Default.DBtable = Helper.FileName;
            //Properties.Settings.Default.DataConnection = con.ConnectionString;
            //Properties.Settings.Default.Save();
            //DBLocation.Text = con.DataSource + "\\" + Helper.FileName + ".dbf";
            Helper.DirPath= System.IO.Path.GetDirectoryName(dialog.FileName);
            DBLocation.Text = Helper.DirPath;
        }
        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog fd = new OpenFileDialog()
            {
                Title = "Select DBF File",
                Filter = "Directory|*.dir",
                FilterIndex = 1,
                RestoreDirectory = false,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "This Folder"
            };
            if (fd.ShowDialog() ?? false)
            {
                AssignFileDB(fd);
            }
        }
    }
}
