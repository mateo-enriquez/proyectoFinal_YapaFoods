using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    // Clase auxiliar fuera del ViewModel
    public class LocalCercano
    {
        public Local Local { get; set; }
        public double DistanciaKm { get; set; }
        public string DistanciaTexto => $"{DistanciaKm:F1} km";
    }

    public class NearbyViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<LocalCercano> LocalesOrdenados { get; set; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand IrAlLocalCommand { get; }

        public NearbyViewModel(DatabaseService dbService)
        {
            _dbService = dbService;

            RefreshCommand = new Command(async () => await CalcularCercania());

            IrAlLocalCommand = new Command<LocalCercano>(async (item) =>
                await Shell.Current.GoToAsync($"ClientCatalogPage?localId={item.Local.Id}"));

            // CORRECCIÓN 1: No usamos Task.Run aquí directamente.
            // Llamamos al método y dejamos que él gestione los hilos internamente.
            _ = CalcularCercania();
        }

        private async Task CalcularCercania()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // CORRECCIÓN 2: Los permisos DEBEN pedirse en el Hilo Principal
                PermissionStatus status = PermissionStatus.Unknown;

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }
                });

                if (status != PermissionStatus.Granted)
                {
                    // Alerta en Hilo Principal
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert("Permiso", "Se necesita ubicación.", "OK"));

                    IsBusy = false; return;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                // Obtener ubicación (esto suele ser seguro en background, pero mejor prevenir)
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location == null) location = new Location(-0.1807, -78.4678);

                var conn = await _dbService.GetConnectionAsync();
                var locales = await conn.Table<Local>().Where(l => l.Estado == "Activo").ToListAsync();

                var listaTemporal = new List<LocalCercano>();

                foreach (var l in locales)
                {
                    if (l.Latitud == 0 && l.Longitud == 0) continue;

                    var locLocal = new Location(l.Latitud, l.Longitud);
                    double distancia = location.CalculateDistance(locLocal, DistanceUnits.Kilometers);

                    listaTemporal.Add(new LocalCercano { Local = l, DistanciaKm = distancia });
                }

                var ordenados = listaTemporal.OrderBy(x => x.DistanciaKm).ToList();

                // CORRECCIÓN 3: Actualizar la lista observable DEBE ser en el Hilo Principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LocalesOrdenados.Clear();
                    foreach (var item in ordenados) LocalesOrdenados.Add(item);
                });
            }
            catch (Exception ex)
            {
                // Alerta de error en Hilo Principal
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert("Error GPS", ex.Message, "OK"));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}