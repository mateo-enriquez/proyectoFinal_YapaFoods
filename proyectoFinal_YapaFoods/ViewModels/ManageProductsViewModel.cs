using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    // Recibimos el ID del local seleccionado desde la navegación
    [QueryProperty(nameof(LocalId), "localId")]
    public class ManageProductsViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private int _localId;

        public ObservableCollection<Producto> Productos { get; set; } = new();

        public ManageProductsViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            LoadCommand = new Command(async () => await LoadData());
            AddProductCommand = new Command(OnAddProduct);
            DeleteCommand = new Command<Producto>(OnDeleteProduct);
        }

        public int LocalId
        {
            get => _localId;
            set
            {
                _localId = value;
                OnPropertyChanged(); // Notificar cambio
                LoadData(); // Cargar datos apenas llegue el ID
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteCommand { get; }

        private async Task LoadData()
        {
            IsBusy = true;
            var conn = await _dbService.GetConnectionAsync();
            var lista = await conn.Table<Producto>().Where(p => p.LocalId == LocalId).ToListAsync();

            Productos.Clear();
            foreach (var p in lista) Productos.Add(p);
            IsBusy = false;
        }

        private async void OnAddProduct()
        {
            // Navegamos a la pantalla de crear, pasando el ID del Local
            await Shell.Current.GoToAsync($"AddEditProductPage?localId={LocalId}");
        }

        private async void OnDeleteProduct(Producto producto)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Eliminar", $"¿Borrar {producto.Nombre}?", "Sí", "No");
            if (confirm)
            {
                var conn = await _dbService.GetConnectionAsync();
                await conn.DeleteAsync(producto);
                await LoadData();
            }
        }
    }
}