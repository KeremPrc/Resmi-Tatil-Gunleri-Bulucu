using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

class Holiday
{
    [JsonPropertyName("date")]
    public string date { get; set; }

    [JsonPropertyName("localName")]
    public string localName { get; set; }

    [JsonPropertyName("name")]
    public string name { get; set; }

    [JsonPropertyName("countryCode")]
    public string countryCode { get; set; }

    // "fixed" JSON alanı C# anahtar sözcüğü olabileceği için burada Fixed adını kullandık
    [JsonPropertyName("fixed")]
    public bool Fixed { get; set; }

    [JsonPropertyName("global")]
    public bool Global { get; set; }
}

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static Dictionary<int, List<Holiday>> holidayCache = new Dictionary<int, List<Holiday>>();

    static async Task Main(string[] args)
    {
        // Türkçe karakter sorunları için çıktı kodlaması
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("API'den tatil verileri alınıyor...\n");

        try
        {
            await LoadHolidaysAsync(2023);
            await LoadHolidaysAsync(2024);
            await LoadHolidaysAsync(2025);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Veriler yüklenirken hata oluştu: " + ex.Message);
            return;
        }

        int secim = 0;
        while (secim != 5)
        {
            Console.WriteLine("===== PublicHolidayTracker =====");
            Console.WriteLine("1. Tatil listesini göster (yıl seçmeli)");
            Console.WriteLine("2. Tarihe göre tatil ara (gg-aa formatı)");
            Console.WriteLine("3. İsme göre tatil ara");
            Console.WriteLine("4. Tüm tatilleri 3 yıl boyunca göster (2023–2025)");
            Console.WriteLine("5. Çıkış");
            Console.Write("Seçiminiz: ");

            var line = Console.ReadLine();
            if (!int.TryParse(line, out secim))
            {
                Console.WriteLine("Geçersiz seçim! Lütfen 1-5 arası bir sayı girin.\n");
                continue;
            }

            Console.WriteLine();
            switch (secim)
            {
                case 1:
                    ShowHolidaysByYear();
                    break;

                case 2:
                    SearchByDate();
                    break;

                case 3:
                    SearchByName();
                    break;

                case 4:
                    ShowAllHolidays();
                    break;

                case 5:
                    Console.WriteLine("Programdan çıkılıyor...");
                    break;

                default:
                    Console.WriteLine("Geçersiz seçim!\n");
                    break;
            }
        }
    }

    // -----------------------------------------------------
    // API'den veri çekme
    // -----------------------------------------------------
    static async Task LoadHolidaysAsync(int year)
    {
        string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/TR";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var holidays = JsonSerializer.Deserialize<List<Holiday>>(json, options) ?? new List<Holiday>();

        // Tatilleri tarih sırasına göre sıralıyoruz
        holidays = holidays.OrderBy(h => DateTime.Parse(h.date)).ToList();

        holidayCache[year] = holidays;

        Console.WriteLine($"{year} tatilleri yüklendi. ({holidays.Count} adet)");
    }

    // -----------------------------------------------------
    // 1) Yıla göre listeleme (TARİHE GÖRE SIRALI)
    // -----------------------------------------------------
    static void ShowHolidaysByYear()
    {
        Console.Write("Yıl giriniz (2023-2025): ");
        if (!int.TryParse(Console.ReadLine(), out int year) || !holidayCache.ContainsKey(year))
        {
            Console.WriteLine("Geçersiz yıl!\n");
            return;
        }

        Console.WriteLine($"\n=== {year} Resmi Tatilleri ===");

        var sortedList = holidayCache[year]
            .OrderBy(h => DateTime.Parse(h.date));  // Tarihe göre sıralı

        foreach (var h in sortedList)
        {
            Console.WriteLine($"{h.date} - {h.localName} ({h.name})");
        }
        Console.WriteLine();
    }

    // -----------------------------------------------------
    // 2) Tarihe göre tatil arama
    // -----------------------------------------------------
    static void SearchByDate()
    {
        Console.Write("Aramak istediğiniz tarih (gg-aa): ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || input.Length != 5 || input[2] != '-')
        {
            Console.WriteLine("Format hatası! Lütfen gg-aa formatında girin (ör: 23-04).\n");
            return;
        }

        Console.WriteLine("\nArama Sonuçları:");
        bool found = false;

        foreach (var year in holidayCache.Keys)
        {
            foreach (var h in holidayCache[year])
            {
                // date formatı: 2023-04-23 → substring ile ay-gün karşılaştırma
                if (h.date.Substring(5) == input)
                {
                    Console.WriteLine($"{h.date} - {h.localName} ({h.name}) [{year}]");
                    found = true;
                }
            }
        }

        if (!found) Console.WriteLine("Sonuç bulunamadı.");

        Console.WriteLine();
    }

    // -----------------------------------------------------
    // 3) İsme göre tatil arama
    // -----------------------------------------------------
    static void SearchByName()
    {
        Console.Write("Tatil adı giriniz (kelime girilebilir): ");
        string name = Console.ReadLine()?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Lütfen bir anahtar kelime girin.\n");
            return;
        }

        Console.WriteLine("\nArama Sonuçları:");
        bool found = false;

        foreach (var year in holidayCache.Keys)
        {
            foreach (var h in holidayCache[year])
            {
                if (h.localName.ToLower().Contains(name) || h.name.ToLower().Contains(name))
                {
                    Console.WriteLine($"{h.date} - {h.localName} ({h.name}) [{year}]");
                    found = true;
                }
            }
        }

        if (!found) Console.WriteLine("Sonuç bulunamadı.");
        Console.WriteLine();
    }

    // -----------------------------------------------------
    // 4) 3 yılın tüm tatillerini gösterme (TARİHE GÖRE SIRALI)
    // -----------------------------------------------------
    static void ShowAllHolidays()
    {
        Console.WriteLine("=== 2023–2025 Tüm Tatiller (Tarihe göre sıralı) ===");

        var allHolidays = holidayCache.Values
            .SelectMany(h => h)
            .OrderBy(h => DateTime.Parse(h.date));  // Tarihe göre sıralı

        foreach (var h in allHolidays)
        {
            Console.WriteLine($"{h.date} - {h.localName} ({h.name})");
        }

        Console.WriteLine();
    }
}
