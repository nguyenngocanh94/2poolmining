using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace Chia2Pool.Views
{
    public partial class DashboardView : MetroWindow
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start("cmd", "/c start "+ e.Uri.AbsoluteUri);

            e.Handled = true;
        }
    }
}