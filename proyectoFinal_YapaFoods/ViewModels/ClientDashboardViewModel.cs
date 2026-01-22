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
    public class ClientDashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<Local> LocalesActivos { get; set; } = new();
        public ICommand VerCatalogoCommand { get; }

        public ClientDashboardViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            VerCatalogoCommand = new Command<Local>(async (l) => await Shell.Current.GoToAsync($"ClientCatalogPage?localId={l.Id}"));
            // Cargar datos al iniciar
            Task.Run(LoadData);
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            var conn = await _dbService.GetConnectionAsync();
            var lista = await conn.Table<Local>().Where(l => l.Estado == "Activo").ToListAsync();

            LocalesActivos.Clear();
            foreach (var l in lista) LocalesActivos.Add(l);
            IsBusy = false;
        }
    }
}