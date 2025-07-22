using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Syncfusion.Licensing;

namespace PDF_Merger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            
            // Register Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("Mzk2MTgzMkAzMzMwMmUzMDJlMzAzYjMzMzAzYmlybk04RFhweE8rKzE5c0NycXEzZFVMS295NXo3QkdhUWRlMjNBbFRDVlk9");
            base.OnStartup(e);
        }
    }
}
