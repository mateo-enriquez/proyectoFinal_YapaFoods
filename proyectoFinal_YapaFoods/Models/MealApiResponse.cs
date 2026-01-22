using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace proyectoFinal_YapaFoods.Models
{
    public class MealApiResponse
    {
        [JsonPropertyName("meals")]
        public List<MealItem> Meals { get; set; }
    }

    public class MealItem
    {
        // La API devuelve "strMeal", nos aseguramos que coincida
        [JsonPropertyName("strMeal")]
        public string NombreComida { get; set; }

        [JsonPropertyName("strMealThumb")]
        public string ImagenUrl { get; set; }
    }
}