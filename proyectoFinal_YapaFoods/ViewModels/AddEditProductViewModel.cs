using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    [QueryProperty(nameof(LocalId), "localId")]
    public class AddEditProductViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly FoodApiService _apiService; // Servicio de API

        private int _localId;
        private string _nombre;
        private string _descripcion;
        private decimal _precio;
        private int _unidades;
        private string _selectedImage;

        // Catálogo de imágenes (simulado)
        public ObservableCollection<string> CatalogoImagenes { get; } = new()
        {
            "dotnet_bot.png",
            "burger.png",
            "pizza.png",
            "soda.png",
            "salad.png"
        };

        // CONSTRUCTOR
        public AddEditProductViewModel(DatabaseService dbService, FoodApiService apiService)
        {
            _dbService = dbService;
            _apiService = apiService;

            // Inicializar Comandos
            GuardarCommand = new Command(OnGuardar);
            SetImageCommand = new Command<string>((img) => SelectedImage = img);

            // --- CORRECCIÓN: Inicializar el comando de la API ---
            SugerirComidaCommand = new Command(OnSugerirComida);
            // ----------------------------------------------------

            // Seleccionar imagen por defecto
            SelectedImage = CatalogoImagenes[0];
        }

        // PROPIEDADES
        public int LocalId { get => _localId; set => SetProperty(ref _localId, value); }
        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
        public string Descripcion { get => _descripcion; set => SetProperty(ref _descripcion, value); }
        public decimal Precio { get => _precio; set => SetProperty(ref _precio, value); }
        public int Unidades { get => _unidades; set => SetProperty(ref _unidades, value); }
        public string SelectedImage { get => _selectedImage; set => SetProperty(ref _selectedImage, value); }

        // COMANDOS PÚBLICOS
        public ICommand GuardarCommand { get; }
        public ICommand SetImageCommand { get; }
        public ICommand SugerirComidaCommand { get; }

        // MÉTODO: Llamar a la API
        private async void OnSugerirComida()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Llamamos al servicio (que ya tiene su manejo de errores interno)
                var plato = await _apiService.GetRandomMealAsync();

                if (plato != null)
                {
                    // Alerta de éxito
                    await Application.Current.MainPage.DisplayAlert("¡Encontrado!", $"Sugerencia: {plato.NombreComida}", "OK");

                    // Actualizar la interfaz en el Hilo Principal para asegurar que se vean los textos
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Nombre = plato.NombreComida;
                        Descripcion = $"Delicioso plato inspirado en: {plato.NombreComida}";
                    });
                }
            }
            catch (Exception ex)
            {
                // Si algo falla, mostramos el error
                await Application.Current.MainPage.DisplayAlert("Error API", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // MÉTODO: Guardar en Base de Datos
        private async void OnGuardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || Precio <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El precio debe ser mayor a 0 y el nombre es obligatorio.", "OK");
                return;
            }

            var nuevoProducto = new Producto
            {
                LocalId = LocalId,
                Nombre = Nombre,
                Descripcion = Descripcion,
                Precio = Precio,
                Unidades = Unidades,
                ImagenSvgPath = SelectedImage
            };

            var conn = await _dbService.GetConnectionAsync();
            await conn.InsertAsync(nuevoProducto);

            await Application.Current.MainPage.DisplayAlert("Éxito", "Producto publicado", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}