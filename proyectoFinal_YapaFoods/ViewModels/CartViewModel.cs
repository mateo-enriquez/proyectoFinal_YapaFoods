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
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly DatabaseService _dbService;

        public ObservableCollection<CartItem> ItemsCarrito { get; set; }
        private decimal _total;

        public CartViewModel(CartService cartService, DatabaseService dbService)
        {
            _cartService = cartService;
            _dbService = dbService;
            ItemsCarrito = _cartService.Carrito; // Enlace directo
            CheckoutCommand = new Command(OnCheckout);
            RecalcularTotal();
        }

        public decimal Total { get => _total; set => SetProperty(ref _total, value); }
        public ICommand CheckoutCommand { get; }

        public void RecalcularTotal() => Total = _cartService.ObtenerTotal();

        private async void OnCheckout()
        {
            if (ItemsCarrito.Count == 0) return;

            IsBusy = true;
            try
            {
                var conn = await _dbService.GetConnectionAsync();

                // 1. Crear Cabecera de Factura
                var nuevaFactura = new Factura
                {
                    UsuarioId = OwnerLocalesViewModel.UsuarioIdActual, // Usamos el ID guardado en Login
                    Fecha = DateTime.Now,
                    Subtotal = Total,
                    IVA = Total * 0.15m,
                    Total = Total * 1.15m
                };
                await conn.InsertAsync(nuevaFactura);

                // 2. Crear Detalles
                foreach (var item in ItemsCarrito)
                {
                    var detalle = new DetalleFactura
                    {
                        FacturaId = nuevaFactura.Id,
                        ProductoId = item.Producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Producto.Precio
                    };
                    await conn.InsertAsync(detalle);
                }

                _cartService.VaciarCarrito();
                RecalcularTotal();

                await Application.Current.MainPage.DisplayAlert("¡Pedido Exitoso!", $"Factura #{nuevaFactura.Id} generada.", "OK");
                await Shell.Current.GoToAsync(".."); // Volver
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}