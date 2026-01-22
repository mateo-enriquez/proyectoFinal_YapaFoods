using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using proyectoFinal_YapaFoods.Services;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.ViewModels;
using proyectoFinal_YapaFoods.Views;

namespace proyectoFinal_YapaFoods
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            //Servicios
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<CorreoService>();
            builder.Services.AddSingleton<CartService>();

            //ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<AdminLocalesViewModel>();
            builder.Services.AddTransient<OwnerLocalesViewModel>();
            builder.Services.AddTransient<AddEditLocalViewModel>();
            builder.Services.AddTransient<VerificationViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<AdminUsersViewModel>();
            builder.Services.AddTransient<ClientDashboardViewModel>();
            builder.Services.AddTransient<ClientCatalogViewModel>();
            builder.Services.AddTransient<CartViewModel>();
            builder.Services.AddTransient<HistoryViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ManageProductsViewModel>();
            builder.Services.AddTransient<AddEditProductViewModel>();
            builder.Services.AddTransient<OwnerOrdersViewModel>();
            builder.Services.AddSingleton<FoodApiService>();

            //Vistas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            //Pages
            builder.Services.AddTransient<AdminDashboardPage>();
            builder.Services.AddTransient<OwnerDashboardPage>();
            builder.Services.AddTransient<AddEditLocalPage>();
            builder.Services.AddTransient<VerificationPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<AdminUsersPage>();
            builder.Services.AddTransient<OwnerOrdersPage>();



            builder.Services.AddTransient<ManageProductsPage>();
            builder.Services.AddTransient<AddEditProductPage>();
            builder.Services.AddTransient<ClientDashboardPage>();
            builder.Services.AddTransient<ClientCatalogPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<NearbyViewModel>();
            builder.Services.AddTransient<NearbyPage>();
            builder.Services.AddTransient<HistoryPage>();
            builder.Services.AddTransient<ProfilePage>();

            return builder.Build();
        }
    }
}