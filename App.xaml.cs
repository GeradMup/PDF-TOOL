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
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCeUx0Qnxbf1x1ZFBMYVhbQHFPIiBoS35Rc0VkW39fd3VRQmddUU11VEFd");
            base.OnStartup(e);
        }
    }
}
