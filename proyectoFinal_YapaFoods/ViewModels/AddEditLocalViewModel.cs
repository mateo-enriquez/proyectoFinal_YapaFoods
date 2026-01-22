using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class AddEditLocalViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        private string _nombre;
        private string _direccionHtml;
        private string _horario;
        private string _contacto;
        private double _latitud;
        private double _longitud;

        public AddEditLocalViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            GuardarCommand = new Command(OnGuardar);
            PegarCoordenadasCommand = new Command(OnPegarCoordenadas);
        }

        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
        public string DireccionHtml { get => _direccionHtml; set => SetProperty(ref _direccionHtml, value); }
        public string Horario { get => _horario; set => SetProperty(ref _horario, value); }
        public string Contacto { get => _contacto; set => SetProperty(ref _contacto, value); }
        public double Latitud { get => _latitud; set => SetProperty(ref _latitud, value); }
        public double Longitud { get => _longitud; set => SetProperty(ref _longitud, value); }

        public ICommand GuardarCommand { get; }
        public ICommand PegarCoordenadasCommand { get; } // <--- NUEVO COMANDO

        private async void OnPegarCoordenadas()
        {
            try
            {
                // 1. Obtener texto del portapapeles
                string texto = await Clipboard.Default.GetTextAsync();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "El portapapeles está vacío", "OK");
                    return;
                }

                // 2. Limpiar espacios y dividir por la coma
                // Google Maps suele dar: "-0.1807, -78.4678"
                var partes = texto.Replace(" ", "").Split(',');

                if (partes.Length >= 2)
                {
                    // Intentar convertir (usando InvariantCulture para que el punto sea decimal)
                    if (double.TryParse(partes[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(partes[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    {
                        Latitud = lat;
                        Longitud = lon;
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Coordenadas detectadas y asignadas.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "No se reconocieron números válidos en el texto copiado.", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El formato debe ser 'Latitud, Longitud' (separado por coma).", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo pegar: {ex.Message}", "OK");
            }
        }

        private async void OnGuardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(DireccionHtml))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Nombre y Dirección son obligatorios", "OK");
                return;
            }

            if (Latitud == 0 || Longitud == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ojo", "Ingresa coordenadas válidas para el mapa.", "OK");
                // Dejamos pasar si quieres, o pones return para bloquear
            }

            var nuevoLocal = new Local
            {
                Nombre = Nombre,
                DireccionHtml = DireccionHtml,
                Horario = Horario,
                Contacto = Contacto,
                Estado = "Pendiente",
                DueñoId = OwnerLocalesViewModel.UsuarioIdActual,
                Latitud = Latitud,
                Longitud = Longitud
            };

            var conn = await _dbService.GetConnectionAsync();
            await conn.InsertAsync(nuevoLocal);

            await Application.Current.MainPage.DisplayAlert("Éxito", "Local enviado a revisión", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}