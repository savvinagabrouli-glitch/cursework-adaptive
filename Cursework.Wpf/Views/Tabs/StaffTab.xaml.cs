using Cursework.Wpf.ViewModels.Admin;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Cursework.Wpf.Views.Tabs
{
    public partial class StaffTab : UserControl
    {
        public StaffTab()
        {
            InitializeComponent();
            DataContext = App.Services.GetRequiredService<StaffTabViewModel>();
        }

        private void StaffGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not StaffTabViewModel vm) return;
            if (vm.Selected == null) return;

            var grid = (DataGrid)sender;

            grid.Dispatcher.BeginInvoke(new Action(() =>
            {
                grid.ScrollIntoView(vm.Selected);
                grid.Focus();
            }), DispatcherPriority.Background);
        }
    }
}
