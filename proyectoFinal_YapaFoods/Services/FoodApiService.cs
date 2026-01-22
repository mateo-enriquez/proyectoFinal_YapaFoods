using System.Net.Http.Json;
using System.Text.Json;
using proyectoFinal_YapaFoods.Models;

namespace proyectoFinal_YapaFoods.Services
{
    public class FoodApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public FoodApiService()
        {
            _httpClient = new HttpClient();
            // Esto permite que no importe si es Mayúscula o Minúscula
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<MealItem> GetRandomMealAsync()
        {
            // URL de prueba
            var url = "https://www.themealdb.com/api/json/v1/1/random.php";

            HttpResponseMessage response = null;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                // Si falla aquí, es que no hay internet o permiso bloqueado
                throw new Exception($"Error de Conexión: {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error del Servidor: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                var data = JsonSerializer.Deserialize<MealApiResponse>(json, _serializerOptions);

                if (data != null && data.Meals != null && data.Meals.Count > 0)
                {
                    return data.Meals[0];
                }
                else
                {
                    throw new Exception("La API respondió, pero la lista 'meals' llegó vacía.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error leyendo JSON: {ex.Message}. JSON recibido: {json}");
            }
        }
    }
}