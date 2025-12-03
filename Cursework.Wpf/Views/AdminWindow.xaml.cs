using Cursework.Domains.Models;
using Cursework.Wpf.ViewModels.Admin;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Cursework.Wpf.Views
{
    public partial class AdminWindow : Window
    {
        public Staff CurrentStaff { get; private set; } = null!;

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += AdminWindow_Loaded;
        }

        public void Init(Staff staff)
        {
            CurrentStaff = staff;
            DataContext = this;
        }

        private async void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AdminWindow_Loaded;

            var vm = App.Services.GetRequiredService<HallTablesTabViewModel>();
            HallTab.DataContext = vm;
            await vm.LoadAsync();
        }

    }
}
