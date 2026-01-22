using proyectoFinal_YapaFoods.Views;

namespace proyectoFinal_YapaFoods
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registramos la ruta para poder navegar con GoToAsync("RegisterPage")
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));

            Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));
            Routing.RegisterRoute("OwnerDashboardPage", typeof(OwnerDashboardPage));
            Routing.RegisterRoute("AddEditLocalPage", typeof(AddEditLocalPage));
            Routing.RegisterRoute("VerificationPage", typeof(VerificationPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("ManageProductsPage", typeof(ManageProductsPage));
            Routing.RegisterRoute("AddEditProductPage", typeof(AddEditProductPage));
            Routing.RegisterRoute("ClientCatalogPage", typeof(ClientCatalogPage));
            Routing.RegisterRoute("CartPage", typeof(CartPage));
            Routing.RegisterRoute("HistoryPage", typeof(HistoryPage));

        }
    }
}