using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Valorant
{
    public class RiotApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public RiotApiHelper(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<dynamic> GetPuuidFromName(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName))
            {
                throw new ArgumentException("Spielername darf nicht leer sein.", nameof(inputName));
            }

            string[] nameParts = inputName.Split('#');
            if (nameParts.Length != 2)
            {
                throw new FormatException("Ungültiges Format. Bitte gib den Namen im Format 'Name#Tag' ein.");
            }

            string realPlayerName = nameParts[0];
            string realPlayerTag = nameParts[1];
            string apiUrl = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{realPlayerName}/{realPlayerTag}";

            try
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-Riot-Token"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("X-Riot-Token");
                }

                _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(responseBody);
                    return json.puuid;
                }
                else
                {
                    throw new HttpRequestException($"Fehler: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler: {ex}");
                //throw new Exception($"Fehler bei der API-Anfrage: {ex.Message}", ex);
            }
        }
    }
}