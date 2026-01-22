using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class AdminDashboardPage : TabbedPage
    {
        private AdminLocalesViewModel _vmLocales;
        private AdminUsersViewModel _vmUsers;

        // Inyectamos AMBOS ViewModels en el constructor
        public AdminDashboardPage(AdminLocalesViewModel vmLocales, AdminUsersViewModel vmUsers)
        {
            InitializeComponent();

            _vmLocales = vmLocales;
            _vmUsers = vmUsers;

            // 1. El contexto GENERAL de la página es para Locales (Pestañas 1 y 2)
            BindingContext = _vmLocales;

            // 2. El contexto ESPECÍFICO de la pestaña Usuarios es para Usuarios (Pestaña 3)
            TabUsuarios.BindingContext = _vmUsers;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Cargamos los datos de ambos al aparecer la pantalla
            await _vmLocales.LoadData();
            await _vmUsers.LoadData();
        }
    }
}