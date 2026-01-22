using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace proyectoFinal_YapaFoods.Models
{
    public class Rol
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string NombreRol { get; set; } // AdminMaestro, Admin, Dueño, Cliente
    }

    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        [Unique]
        public string Correo { get; set; }
        public string Password { get; set; } // Recuerda: Hash con BCrypt
        public int RolId { get; set; }
        public bool IsVerified { get; set; }
    }

    public class Local
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string DireccionHtml { get; set; } // Iframe de Google Maps
        public string Horario { get; set; }
        public string Contacto { get; set; }
        public string Estado { get; set; } // Pendiente, Activo, Suspendido, Inactivo
        public int DueñoId { get; set; } // FK Usuario
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }

    public class Producto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LocalId { get; set; } // FK Local
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Unidades { get; set; }
        public string ImagenSvgPath { get; set; }
    }

    public class Factura
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }

    public class DetalleFactura
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class OTP
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Correo { get; set; }
        public string Codigo { get; set; }
        public DateTime Expiracion { get; set; }
        public DateTime UltimoEnvio { get; set; }
    }
}
