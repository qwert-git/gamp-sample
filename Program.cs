using Newtonsoft.Json;

const string CurrencyUri = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json";
var client = new HttpClient();

Console.WriteLine("Starting the process..");
while (true)
{
    var allCurrencyRates = await  GetAllCurrencyRates();
    
    var usd = allCurrencyRates!.First(c => c.cc == "USD");
    Console.WriteLine($"cc: {usd.cc}, rate: {usd.rate}");

    Console.WriteLine("Sending the hit");

    var hitRes = await client.SendAsync(GetGaCollectRequestMessage(usd));

    Console.WriteLine($"Hit result: {hitRes.IsSuccessStatusCode}");

    Thread.Sleep(TimeSpan.FromSeconds(5));
}

async Task<dynamic[]?> GetAllCurrencyRates()
{
    var res = await client.GetAsync(CurrencyUri);
    if (!res.IsSuccessStatusCode)
    {
        Console.WriteLine("Getting currency failed!");
        return null;
    }

    var currencyRateJson = await res.Content.ReadAsStringAsync();
    return JsonConvert.DeserializeObject<dynamic[]>(currencyRateJson);
}

static HttpRequestMessage GetGaCollectRequestMessage(dynamic usdCurrency)
{
    return new HttpRequestMessage(HttpMethod.Post, "https://www.google-analytics.com/collect")
    {
        Content = GetGaPayload(usdCurrency.rate)
    };
}

static StringContent GetGaPayload(double rate) => new StringContent($"v=1&t=event&tid=UA-137822631-2&cid=555&ec=currency-rate&ea={rate}&el=projector-homework");