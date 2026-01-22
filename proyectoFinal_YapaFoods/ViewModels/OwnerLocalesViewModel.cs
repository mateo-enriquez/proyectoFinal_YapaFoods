using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class OwnerLocalesViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        public ObservableCollection<Local> MisLocales { get; set; } = new();
        public ICommand LoadCommand { get; }
        public ICommand NuevoLocalCommand { get; }
        public ICommand ManageProductsCommand { get; }
        public ICommand VerPedidosCommand { get; }

        // Necesitamos saber qué usuario está logueado. 
        // Por simplicidad, guardaremos el ID temporalmente en una variable estática o servicio de sesión.
        // Aquí asumiremos que pasas el ID o lo obtienes de una "UserSession".
        public static int UsuarioIdActual;

        public OwnerLocalesViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            LoadCommand = new Command(async () => await LoadData());
            NuevoLocalCommand = new Command(async () => await Shell.Current.GoToAsync("AddEditLocalPage"));
            ManageProductsCommand = new Command<Local>(async (local) => {
                if (local == null) return;
                await Shell.Current.GoToAsync($"ManageProductsPage?localId={local.Id}");
            });
            VerPedidosCommand = new Command(async () => await Shell.Current.GoToAsync("OwnerOrdersPage"));
        }

        public async Task LoadData()
        {
            IsBusy = true;
            var conn = await _dbService.GetConnectionAsync();
            var locales = await conn.Table<Local>()
                                    .Where(l => l.DueñoId == UsuarioIdActual)
                                    .ToListAsync();
            MisLocales.Clear();
            foreach (var l in locales) MisLocales.Add(l);
            IsBusy = false;
        }
    }
}