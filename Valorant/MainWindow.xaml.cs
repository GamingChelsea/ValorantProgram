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


namespace Valorant
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;

        public MainWindow()
        {
            InitializeComponent();

            string testApiKey = Environment.GetEnvironmentVariable("VALORANT_API_KEY");
            MessageBox.Show($"Gefundener API-Schlüssel: {testApiKey ?? "Nicht gefunden"}");

            _apiKey = Environment.GetEnvironmentVariable("VALORANT_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                MessageBox.Show("API-Schlüssel nicht gefunden. Bitte Umgebungsvariable 'VALORANT_API_KEY' setzen.", "Fehler", MessageBoxButton.OK);
            }
        }

        public async Task GetPuuidFromName()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                StatsBox.Text = "API-Schlüssel fehlt. Bitte konfigurieren.";
                return;
            }

            string realPlayerName = null;
            string realPlayerTag = null;

            string apiUrl = "https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/GamingChelseaTV/EUW";

            TextBox playerNameInput = PlayerNameInput;
            string InputName = playerNameInput.Text;

            if (string.IsNullOrWhiteSpace(InputName) || InputName == "Gebe deinen Valorant Namen ein:")
            {
                StatsBox.Text = "Bitte gib einen Spielernamen ein.";
                return;
            }

            //Spilt the name and tag
            string[] nameParts = InputName.Split('#');

            if (nameParts.Length == 2)
            {
                realPlayerName = nameParts[0];
                realPlayerTag = "#" + nameParts[1];
                StatsBox.Text = "Spielername: " + realPlayerName + "\nTag: " + realPlayerTag;
            }
            else
            {
                StatsBox.Text = "Ungültiges Format. Bitte gib den Namen im Format 'Name#Tag' ein.";
                return;
            }
            //get Puuid and make API Call
            try
            {
                // Entferne vorherige Header, um doppelte Einträge zu vermeiden
                if (_httpClient.DefaultRequestHeaders.Contains("X-Riot-Token"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("X-Riot-Token");
                }

                // Setze den API-Schlüssel in den Header
                _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);

                StatsBox.Text = "Statistiken werden geladen...";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    StatsBox.Text = $"Antwort: {responseBody}";
                }
                else
                {
                    StatsBox.Text = $"Fehler: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                StatsBox.Text = $"Fehler: {ex.Message}";
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