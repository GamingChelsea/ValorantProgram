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
        private readonly RiotApiHelper _riotApiHelper;

        public MainWindow()
        {
            InitializeComponent();

            string apiKey = Environment.GetEnvironmentVariable("VALORANT_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API-Schlüssel nicht gefunden. Bitte Umgebungsvariable 'VALORANT_API_KEY' setzen.", "Fehler", MessageBoxButton.OK);
                return;
            }

            _riotApiHelper = new RiotApiHelper(new HttpClient(), apiKey);
        }

        private async void BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputName = PlayerNameInput.Text;
                string puuid = await _riotApiHelper.GetPuuidFromName(inputName);
                StatsBox.Text = $"Puuid: {puuid}";
            }
            catch (Exception ex)
            {
                StatsBox.Text = $"Fehler: {ex.Message}";
            }
        }

    }

}