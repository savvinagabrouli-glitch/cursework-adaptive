using Cursework.Application.Interfaces;
using Cursework.Application.Security;
using Cursework.Application.Services.Api;
using Cursework.Domains.Models;
using Cursework.Wpf.Options;
using Cursework.Wpf.Services.HallLayout;
using Cursework.Wpf.Services.QR_Code;
using Cursework.Wpf.Services.Realtime;
using Cursework.Wpf.ViewModels.Admin;
using Cursework.Wpf.ViewModels.Waiter;
using Cursework.Wpf.Views;
using Cursework.Wpf.Views.Auth;
using Cursework.Wpf.Views.Waiter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace Cursework.Wpf;

public partial class App : System.Windows.Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var baseDir = AppContext.BaseDirectory;

        var sharedCfgPath = Path.GetFullPath(Path.Combine(baseDir, "..", "config", "appsettingsMain.json"));

        var cfg = new ConfigurationBuilder()
            .SetBasePath(baseDir)
            .AddJsonFile(sharedCfgPath, optional: true, reloadOnChange: true) 
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var profile = cfg["ActiveProfile"] ?? "Local";

        var cs = cfg[$"Profiles:{profile}:ConnectionStrings:Default"]
                 ?? cfg.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("ConnectionStrings:Default пуст. Проверь appsettingsMain.json (Profiles + ActiveProfile) или appsettings.json");

        var apiBaseUrl = cfg[$"Profiles:{profile}:ApiBaseUrl"]
                         ?? cfg["ApiBaseUrl"]
                         ?? "http://localhost:5201";

        var sc = new ServiceCollection();

        var guestSection = cfg.GetSection($"Profiles:{profile}:GuestSite");
        if (!guestSection.Exists())
            guestSection = cfg.GetSection("GuestSite");

        sc.Configure<GuestSiteOptions>(guestSection);


        sc.AddSingleton<IQrCodeService, QrCodeService>();

        sc.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        sc.AddHttpClient("Backend", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        sc.AddScoped<IStaffService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiStaffService(http);
        });

        sc.AddScoped<ITableService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiTableService(http);
        });

        sc.AddScoped<IMenuService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiMenuService(http);
        });

        sc.AddScoped<IOrderService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiOrderService(http);
        });

        sc.AddScoped<IOrderDetailsService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiOrderDetailsService(http);
        });

        sc.AddScoped<ICallWaiterService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend");
            return new ApiCallWaiterService(http);
        });

        sc.AddSingleton<IRealtimeService, SignalRRealtimeService>();

        sc.AddSingleton<AdminWindow>();
        sc.AddSingleton<WaiterWindow>();

        sc.AddTransient<StaffTabViewModel>();
        sc.AddTransient<WaiterWindowViewModel>();
        sc.AddTransient<HallTablesTabViewModel>();
        sc.AddTransient<MenuTabViewModel>();
        sc.AddTransient<OrdersTabViewModel>();
        sc.AddTransient<CallWaiterTabViewModel>();
        sc.AddSingleton<IHallLayoutStorageService, HallLayoutStorageService>();

        sc.AddTransient<LoginForm>();

        Services = sc.BuildServiceProvider();

        var realtime = Services.GetRequiredService<IRealtimeService>();
        _ = realtime.StartAsync();

        var login = Services.GetRequiredService<LoginForm>();
        var result = login.ShowDialog();

        if (result == true && login.SelectedStaff is Staff staff)
        {
            if (string.Equals(staff.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var admin = Services.GetRequiredService<AdminWindow>();
                admin.Init(staff);
                MainWindow = admin;
                admin.Show();
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                var waiter = Services.GetRequiredService<WaiterWindow>();
                waiter.Init(staff);
                MainWindow = waiter;
                waiter.Show();
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
        }
        else
        {
            Shutdown();
        }
    }
    protected override async void OnExit(ExitEventArgs e)
    {
        if (Services != null)
        {
            var realtime = Services.GetService<IRealtimeService>();
            if (realtime != null)
            {
                try
                {
                    await realtime.StopAsync();
                    await realtime.DisposeAsync();
                }
                catch
                {
                }
            }
        }

        base.OnExit(e);
    }
}
