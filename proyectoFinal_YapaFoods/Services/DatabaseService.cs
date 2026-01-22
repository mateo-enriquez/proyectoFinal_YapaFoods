using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using proyectoFinal_YapaFoods.Models;

namespace proyectoFinal_YapaFoods.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "yapafoods_v4.db");
            _database = new SQLiteAsyncConnection(dbPath);

            // Creación de tablas
            await _database.CreateTableAsync<Rol>();
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Local>();
            await _database.CreateTableAsync<Producto>();
            await _database.CreateTableAsync<Factura>();
            await _database.CreateTableAsync<DetalleFactura>();
            await _database.CreateTableAsync<OTP>();

            await SeedDataAsync();
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            await InitializeAsync();
            return _database;
        }
        private async Task SeedDataAsync()
        {
            // Verificar si existen roles
            var count = await _database.Table<Rol>().CountAsync();
            if (count == 0)
            {
                // 1. Crear Roles
                var roles = new List<Rol>
                {
                    new Rol { NombreRol = "AdminMaestro" },
                    new Rol { NombreRol = "Admin" },
                    new Rol { NombreRol = "Dueño" },
                    new Rol { NombreRol = "Cliente" }
                };
                await _database.InsertAllAsync(roles);

                // 2. Crear Admin Maestro (Password debe ser hasheada en producción)
                // Nota: Aquí guardo texto plano solo por el ejemplo inicial, 
                // pero en el registro real debes usar BCrypt.Net.BCrypt.HashPassword("Yapafoods04")
                var adminMaestro = new Usuario
                {
                    Nombre = "Super Admin",
                    Correo = "admin@yapafoods.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Yapafoods04"),
                    RolId = 1, // ID del AdminMaestro
                    IsVerified = true
                };
                await _database.InsertAsync(adminMaestro);

                var dueñoLocalBase= new Usuario
                {
                    Nombre = "Owner Base",
                    Correo = "owner@yapafoods.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Yapafoods04"),
                    RolId = 3, // ID de Owner
                    IsVerified = true
                };
                await _database.InsertAsync(dueñoLocalBase);

                var usuarioBase = new Usuario
                {
                    Nombre = "Usuario Base",
                    Correo = "usuario@yapafoods.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Yapafoods04"),
                    RolId = 4, // ID del AdminMaestro
                    IsVerified = true
                };
                await _database.InsertAsync(usuarioBase);
            }
        }

        // Métodos CRUD genéricos o específicos pueden ir aquí abajo...
        public SQLiteAsyncConnection Connection => _database;
    }
}