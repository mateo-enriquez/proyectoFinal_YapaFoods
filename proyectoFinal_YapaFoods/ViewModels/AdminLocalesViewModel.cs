using System.Collections.ObjectModel;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class AdminLocalesViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        // Listas separadas para las pestañas
        public ObservableCollection<Local> Pendientes { get; set; } = new();
        public ObservableCollection<Local> Activos { get; set; } = new();
        public ObservableCollection<Local> Suspendidos { get; set; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand AprobarCommand { get; }
        public ICommand SuspenderCommand { get; }

        public AdminLocalesViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            RefreshCommand = new Command(async () => await LoadData());
            AprobarCommand = new Command<Local>(OnAprobar);
            SuspenderCommand = new Command<Local>(OnSuspender);
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var connection = await _dbService.GetConnectionAsync();
                var todos = await connection.Table<Local>().ToListAsync();

                Pendientes.Clear();
                Activos.Clear();
                Suspendidos.Clear();

                foreach (var local in todos)
                {
                    if (local.Estado == "Pendiente") Pendientes.Add(local);
                    else if (local.Estado == "Activo") Activos.Add(local);
                    else if (local.Estado == "Suspendido") Suspendidos.Add(local);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnAprobar(Local local)
        {
            if (local == null) return;
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", $"¿Aprobar {local.Nombre}?", "Sí", "No");
            if (confirm)
            {
                local.Estado = "Activo";
                var conn = await _dbService.GetConnectionAsync();
                await conn.UpdateAsync(local);
                await LoadData(); // Recargar listas
            }
        }

        private async void OnSuspender(Local local)
        {
            if (local == null) return;
            string razon = await Application.Current.MainPage.DisplayPromptAsync("Suspender", "Motivo de suspensión:");
            if (!string.IsNullOrWhiteSpace(razon))
            {
                local.Estado = "Suspendido";
                // Aquí podrías guardar la razón en otra tabla o campo si quisieras
                var conn = await _dbService.GetConnectionAsync();
                await conn.UpdateAsync(local);
                await LoadData();
            }
        }
    }
}