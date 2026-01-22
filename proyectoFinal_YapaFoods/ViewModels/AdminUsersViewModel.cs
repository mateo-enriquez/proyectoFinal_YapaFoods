using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class AdminUsersViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<Usuario> ListaUsuarios { get; set; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand CambiarRolCommand { get; }

        public AdminUsersViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            RefreshCommand = new Command(async () => await LoadData());
            CambiarRolCommand = new Command<Usuario>(OnCambiarRol);
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var conn = await _dbService.GetConnectionAsync();
                // Traemos todos los usuarios, ordenados por nombre
                var usuarios = await conn.Table<Usuario>().OrderBy(u => u.Nombre).ToListAsync();

                ListaUsuarios.Clear();
                foreach (var user in usuarios)
                {
                    // Opcional: No mostrar al Admin Maestro (Id 1) para evitar accidentes
                    if (user.RolId != 1)
                    {
                        ListaUsuarios.Add(user);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnCambiarRol(Usuario usuario)
        {
            if (usuario == null) return;

            // Mostramos las opciones disponibles
            string accion = await Application.Current.MainPage.DisplayActionSheet(
                $"Cambiar rol de {usuario.Nombre}",
                "Cancelar",
                null,
                "Hacer Admin",
                "Hacer Dueño",
                "Hacer Cliente");

            int nuevoRolId = -1;

            switch (accion)
            {
                case "Hacer Admin": nuevoRolId = 2; break;
                case "Hacer Dueño": nuevoRolId = 3; break;
                case "Hacer Cliente": nuevoRolId = 4; break;
                default: return; // Si cancela, no hacemos nada
            }

            if (nuevoRolId != -1)
            {
                // Actualizamos en Base de Datos
                usuario.RolId = nuevoRolId;
                var conn = await _dbService.GetConnectionAsync();
                await conn.UpdateAsync(usuario);

                await Application.Current.MainPage.DisplayAlert("Éxito", $"El rol de {usuario.Nombre} ha sido actualizado.", "OK");

                // Recargamos la lista para reflejar cambios visuales si fuera necesario
                await LoadData();
            }
        }
    }
}