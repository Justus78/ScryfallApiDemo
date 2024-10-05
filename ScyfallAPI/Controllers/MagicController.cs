using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using ScyfallAPI.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace ScyfallAPI.Controllers
{
    public class MagicController : Controller
    {
        private readonly HttpClient _httpClient;

        public MagicController()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(handler);

            // Set the headers to match the browser request
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:129.0) Gecko/20100101 Firefox/129.0");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.5));
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Host = "api.scryfall.com";
        }

        public async Task<IActionResult> Index()
        {
            MagicCard card = null;

            string url = "https://api.scryfall.com/cards/random";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var cardJson = await response.Content.ReadAsStringAsync();
                    var jsonObj = JObject.Parse(cardJson);

                    card = new MagicCard
                    {
                        Name = (string)jsonObj["name"],
                        ManaCost = (string)jsonObj["mana_cost"],
                        TypeLine = (string)jsonObj["type_line"],
                        OracleText = (string)jsonObj["oracle_text"],
                        ImageUrl = (string)jsonObj["image_uris"]["normal"]
                    };
                }
                else
                {
                    // Log or handle the error response
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                // Handle HTTP request exceptions
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonReaderException e)
            {
                // Handle JSON parsing errors
                Console.WriteLine($"JSON parsing error: {e.Message}");
            }

            return View(card);
        }
    }
}
