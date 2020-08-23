using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EStoreBillReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Helper.DirPath = Helper.SetProperty.DirPath;
            base.OnStartup(e);
        }
    }
}
