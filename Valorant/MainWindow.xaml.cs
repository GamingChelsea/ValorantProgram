using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace Valorant
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;

        public MainWindow()
        {
            InitializeComponent();


            // Set the API key from the environment variable
            _apiKey = Environment.GetEnvironmentVariable("VALORANT_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                MessageBox.Show("API-Schlüssel nicht gefunden. Bitte Umgebungsvariable 'VALORANT_API_KEY' setzen.", "Fehler", MessageBoxButton.OK);
            }
        }

        public async Task<dynamic> GetPuuidFromName()
        {
            string realPlayerName;
            string realPlayerTag;

            string apiUrl = "https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/";

            TextBox playerNameInput = PlayerNameInput;
            string InputName = playerNameInput.Text;

            if (string.IsNullOrWhiteSpace(InputName) || InputName == "Gebe deinen Valorant Namen ein:")
            {
                StatsBox.Text = "Bitte gib einen Spielernamen ein.";
                return null; // Ensure a return value
            }

            // Check if the API key is set
            if (string.IsNullOrEmpty(_apiKey))
            {
                StatsBox.Text = "API-Schlüssel fehlt. Bitte konfigurieren.";
                return null; // Ensure a return value
            }

            // Split the name and tag
            string[] nameParts = InputName.Split('#');

            if (nameParts.Length == 2)
            {
                realPlayerName = nameParts[0];
                realPlayerTag = nameParts[1];
                apiUrl = apiUrl + $"{realPlayerName}/{realPlayerTag}";
            }
            else
            {
                StatsBox.Text = "Ungültiges Format. Bitte gib den Namen im Format 'Name#Tag' ein.";
                return null; // Ensure a return value
            }

            // Get Puuid and make API Call
            try
            {
                // Remove previous headers to avoid duplicates
                if (_httpClient.DefaultRequestHeaders.Contains("X-Riot-Token"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("X-Riot-Token");
                }

                // Set the API key in the header
                _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);

                StatsBox.Text = "Statistiken werden geladen...";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(responseBody);
                    StatsBox.Text = $"Puuid: {json.puuid}";
                    return json.puuid; // Ensure a return value
                }
                else
                {
                    StatsBox.Text = $"Fehler: {response.StatusCode}";
                    return null; // Ensure a return value
                }
            }
            catch (Exception ex)
            {
                StatsBox.Text = $"Fehler: {ex.Message}";
                return null; // Ensure a return value
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetPuuidFromName();
        }

        private async void BTN_Click(object sender, RoutedEventArgs e)
        {
            await GetPuuidFromName();
        }
    }

}