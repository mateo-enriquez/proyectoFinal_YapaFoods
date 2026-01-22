using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    [QueryProperty(nameof(LocalId), "localId")]
    public class ClientCatalogViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly CartService _cartService;
        private int _localId;
        private string _searchText;
        private List<Producto> _todosLosProductos = new(); // Lista respaldo para filtrar

        public ObservableCollection<Producto> ProductosFiltrados { get; set; } = new();

        public ClientCatalogViewModel(DatabaseService dbService, CartService cartService)
        {
            _dbService = dbService;
            _cartService = cartService;
            AddToCartCommand = new Command<Producto>(OnAddToCart);
            GoToCartCommand = new Command(async () => await Shell.Current.GoToAsync("CartPage"));
        }

        public int LocalId
        {
            get => _localId;
            set { _localId = value; Task.Run(LoadData); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FiltrarProductos(); // Filtrar cada vez que escribe
            }
        }

        public ICommand AddToCartCommand { get; }
        public ICommand GoToCartCommand { get; }

        private async Task LoadData()
        {
            IsBusy = true;
            var conn = await _dbService.GetConnectionAsync();
            _todosLosProductos = await conn.Table<Producto>().Where(p => p.LocalId == LocalId).ToListAsync();
            FiltrarProductos();
            IsBusy = false;
        }

        private void FiltrarProductos()
        {
            ProductosFiltrados.Clear();
            var query = string.IsNullOrWhiteSpace(SearchText)
                ? _todosLosProductos
                : _todosLosProductos.Where(p => p.Nombre.ToLower().Contains(SearchText.ToLower()));

            foreach (var p in query) ProductosFiltrados.Add(p);
        }

        private async void OnAddToCart(Producto producto)
        {
            _cartService.AgregarProducto(producto, 1);
            await Application.Current.MainPage.DisplayAlert("Carrito", $"{producto.Nombre} agregado.", "OK");
        }
    }
}