using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Pobierz kurs wymiany PLN na USD z API NBP
        decimal exchangeRate = await GetExchangeRateAsync();

        // Poproś użytkownika o kwote PLN
        Console.Write("Podaj kwote PLN: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal plnAmount))
        {
            // Przelicz kwotę na USD
            decimal usdAmount = plnAmount / exchangeRate;
                    // Wyświetl wynik
            Console.WriteLine($"Kwota w USD: {usdAmount:F2}");
        }
        else
        {
            Console.WriteLine("Nieprawidłowa kwota.");
        }
    }

    static async Task<decimal> GetExchangeRateAsync()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("http://api.nbp.pl/api/exchangerates/rates/c/usd/today/");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                NbpApiResponse nbpApiResponse = JsonConvert.DeserializeObject<NbpApiResponse>(responseBody);

                if (nbpApiResponse.Rates.Count > 0)
                {
                    return nbpApiResponse.Rates[0].Mid;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas pobierania kursu: {ex.Message}");
            }
        }

        return 0; // Domyślna wartość w przypadku błędu
    }
}

public class NbpApiResponse
{
    public List<NbpExchangeRate> Rates { get; set; }
}

public class NbpExchangeRate
{
    public decimal Mid { get; set; }
}
