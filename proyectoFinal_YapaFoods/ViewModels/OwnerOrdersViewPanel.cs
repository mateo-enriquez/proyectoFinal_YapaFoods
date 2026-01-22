using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    // Clase auxiliar para mostrar en la lista
    public class VentaItem
    {
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string ClienteNombre { get; set; }
        public string LocalNombre { get; set; }

        // Propiedad visual para agrupar o mostrar
        public string Resumen => $"{Cantidad}x {ProductoNombre} (${TotalVenta:F2})";
    }

    public class OwnerOrdersViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<VentaItem> MisVentas { get; set; } = new();
        public ICommand RefreshCommand { get; }

        public OwnerOrdersViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            RefreshCommand = new Command(async () => await LoadData());
            Task.Run(LoadData);
        }

        private async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var conn = await _dbService.GetConnectionAsync();
                int myId = OwnerLocalesViewModel.UsuarioIdActual;

                // 1. Obtener mis locales
                var misLocales = await conn.Table<Local>().Where(l => l.DueñoId == myId).ToListAsync();
                var misLocalesIds = misLocales.Select(l => l.Id).ToList();

                if (!misLocalesIds.Any())
                {
                    IsBusy = false; return;
                }

                // 2. Obtener mis productos
                // (SQLite-net no soporta 'Contains' complejo, así que traemos productos y filtramos en memoria o hacemos loop)
                // Forma optimizada para SQLite-net: Traer todos y filtrar en C# si no son miles
                var todosProductos = await conn.Table<Producto>().ToListAsync();
                var misProductos = todosProductos.Where(p => misLocalesIds.Contains(p.LocalId)).ToList();
                var misProductosIds = misProductos.Select(p => p.Id).ToList();

                if (!misProductosIds.Any())
                {
                    IsBusy = false; return;
                }

                // 3. Obtener detalles de facturas donde estén mis productos
                var todosDetalles = await conn.Table<DetalleFactura>().ToListAsync();
                var misVentasDetalle = todosDetalles.Where(d => misProductosIds.Contains(d.ProductoId)).ToList();

                // 4. Armar la lista final cruzando datos (Factura y Usuario)
                var listaVisual = new List<VentaItem>();

                foreach (var detalle in misVentasDetalle)
                {
                    var producto = misProductos.FirstOrDefault(p => p.Id == detalle.ProductoId);
                    var local = misLocales.FirstOrDefault(l => l.Id == producto.LocalId);
                    var factura = await conn.FindWithQueryAsync<Factura>("SELECT * FROM Factura WHERE Id = ?", detalle.FacturaId);
                    var cliente = await conn.FindWithQueryAsync<Usuario>("SELECT * FROM Usuario WHERE Id = ?", factura.UsuarioId);

                    listaVisual.Add(new VentaItem
                    {
                        ProductoNombre = producto.Nombre,
                        Cantidad = detalle.Cantidad,
                        TotalVenta = detalle.PrecioUnitario * detalle.Cantidad,
                        Fecha = factura.Fecha,
                        ClienteNombre = cliente != null ? cliente.Nombre : "Desconocido",
                        LocalNombre = local.Nombre
                    });
                }

                // 5. Ordenar por fecha (más reciente arriba)
                var ordenado = listaVisual.OrderByDescending(x => x.Fecha).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MisVentas.Clear();
                    foreach (var v in ordenado) MisVentas.Add(v);
                });

            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}