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
        private int _localId;

        // Formulario
        private string _nombre;
        private string _descripcion;
        private decimal _precio;
        private int _unidades;
        private string _selectedImage;

        // Catálogo de imágenes precargadas (Simulación)
        // DEBES asegurarte de tener archivos con estos nombres en Resources/Images (formato .png o .svg)
        // O usa "dotnet_bot.png" si no tienes otros para probar.
        public ObservableCollection<string> CatalogoImagenes { get; } = new()
        {
            "burger.png",
            "chicken.png",
            "fries.png",
            "kfc.png",
            "pizza.png",
            "sushi.png",
        };

        public AddEditProductViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            GuardarCommand = new Command(OnGuardar);            
            SetImageCommand = new Command<string>((img) => SelectedImage = img);
            SelectedImage = CatalogoImagenes[0];
        }

        public int LocalId { get => _localId; set => SetProperty(ref _localId, value); }

        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
        public string Descripcion { get => _descripcion; set => SetProperty(ref _descripcion, value); }
        public decimal Precio { get => _precio; set => SetProperty(ref _precio, value); }
        public int Unidades { get => _unidades; set => SetProperty(ref _unidades, value); }
        public string SelectedImage { get => _selectedImage; set => SetProperty(ref _selectedImage, value); }

        public ICommand GuardarCommand { get; }
        public ICommand SetImageCommand { get; }

        private async void OnGuardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || Precio <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Verifica el nombre y el precio", "OK");
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
            await Shell.Current.GoToAsync(".."); // Volver
        }
    }
}