using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BancolombiaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly string countriesApiUrl = "https://restcountries.com/v3.1/all";
        Dictionary<string, Tuple<double, double>>? dataFiltered;
        Dictionary<string, double>? relation;

        // GET api/countries/top5densest
        [HttpGet("top5densest")]
        public async Task<ActionResult<IEnumerable<object>>> GetTop5DensestCountriesAsync()
        {
            var countries = await GetCountriesFromApi();
            if (countries == null)
            {
                return BadRequest("Unable to fetch countries data.");
            }

            var top5Densest = CalculateDensity(countries)
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(kvp => new { Country = kvp.Key, Density = kvp.Value })
                .ToList();
            return Ok(top5Densest);
        }

        private async Task<Dictionary<string, Tuple<double, double>>> GetCountriesFromApi()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(countriesApiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonDocument = JsonDocument.Parse(json);
                        dataFiltered = new Dictionary<string, Tuple<double, double>>();
                        
                        foreach (JsonElement countryElement in jsonDocument.RootElement.EnumerateArray())
                        {
                            if (countryElement.TryGetProperty("name", out JsonElement nameElement) && countryElement.TryGetProperty("area", out JsonElement areaElement) && countryElement.TryGetProperty("population", out JsonElement populationElement))
                            {
                                string name = nameElement.GetProperty("common").GetString();
                                double area = areaElement.GetDouble();
                                double population = populationElement.GetDouble();
                                dataFiltered.Add(name, Tuple.Create(area, population));
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
            }
            return dataFiltered;
        }

        private Dictionary<string, double> CalculateDensity(Dictionary<string, Tuple<double, double>> areas)
        {
            relation = new Dictionary<string, double>();
            foreach (var kvp in areas)
            {
                double relacionDivision = kvp.Value.Item2 / kvp.Value.Item1;
                relation.Add(kvp.Key, relacionDivision);
            }
            return relation;
        }

    }
}
