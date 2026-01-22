using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<Factura> MisFacturas { get; set; } = new();
        public ICommand RefreshCommand { get; }
        public ICommand VerDetalleCommand { get; }

        public HistoryViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            RefreshCommand = new Command(async () => await LoadData());

            // Opcional: Si quisiéramos ver el detalle de productos de una factura específica
            VerDetalleCommand = new Command<Factura>(async (f) =>
                await Application.Current.MainPage.DisplayAlert("Info", $"Factura #{f.Id}\nTotal: ${f.Total:F2}\nFecha: {f.Fecha}", "OK"));

            Task.Run(LoadData);
        }

        private async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var conn = await _dbService.GetConnectionAsync();

                // Obtenemos el ID del usuario actual (que guardamos en el Login)
                int userId = OwnerLocalesViewModel.UsuarioIdActual;

                // Buscamos sus facturas ordenadas por fecha (la más reciente primero)
                var lista = await conn.Table<Factura>()
                                      .Where(f => f.UsuarioId == userId)
                                      .OrderByDescending(f => f.Fecha)
                                      .ToListAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MisFacturas.Clear();
                    foreach (var f in lista) MisFacturas.Add(f);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}