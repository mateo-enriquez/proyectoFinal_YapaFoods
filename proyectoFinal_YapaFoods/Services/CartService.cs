using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using proyectoFinal_YapaFoods.Models;

namespace proyectoFinal_YapaFoods.Services
{
    public class CartService
    {
        public ObservableCollection<CartItem> Carrito { get; private set; } = new();

        public void AgregarProducto(Producto producto, int cantidad)
        {
            var itemExistente = Carrito.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                Carrito.Add(new CartItem { Producto = producto, Cantidad = cantidad });
            }
        }

        public void VaciarCarrito() => Carrito.Clear();

        public decimal ObtenerTotal() => Carrito.Sum(i => i.Subtotal);
    }
}