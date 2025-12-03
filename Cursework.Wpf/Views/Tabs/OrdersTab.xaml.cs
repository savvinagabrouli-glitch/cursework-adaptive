using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Cursework.Wpf.ViewModels.Admin;
using Microsoft.Extensions.DependencyInjection;

namespace Cursework.Wpf.Views.Tabs
{
    public partial class OrdersTab : UserControl
    {
        public OrdersTab()
        {
            InitializeComponent();
            DataContext = App.Services.GetRequiredService<OrdersTabViewModel>();
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not OrdersTabViewModel vm)
                return;

            if (vm.Selected == null)
                return;

            var grid = (DataGrid)sender;

            grid.Dispatcher.BeginInvoke(new Action(() =>
            {
                grid.ScrollIntoView(vm.Selected);
                grid.Focus();
            }), DispatcherPriority.Background);
        }
    }
}
