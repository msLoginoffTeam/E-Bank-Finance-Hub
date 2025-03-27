using Core.Data;
using Core.Data.Models;
using Core_Api.Data.Models;
using System.Text;
using System.Xml.Linq;

namespace Core_Api.Services.Utils
{

    public class CurrencyCoursesGetter : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HttpClient client;
        private Timer _timer;

        public CurrencyCoursesGetter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            client = new HttpClient();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(GetCourses, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void GetCourses(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                List<CurrencyCourse> CurrencyCourses = new List<CurrencyCourse>();
                CurrencyCourses.Add(new CurrencyCourse(Currency.Ruble, 100));

                try
                {
                    HttpResponseMessage Response = await client.GetAsync("https://www.cbr.ru/scripts/XML_val.asp");
                    Response.EnsureSuccessStatusCode();

                    List<string> CurrenciesCodes = new List<string>();
                    string CurrenciesNames = await Response.Content.ReadAsStringAsync();
                    XDocument XmlCurrenciesNames = XDocument.Parse(CurrenciesNames);

                    Response = await client.GetAsync("https://www.cbr.ru/scripts/XML_daily.asp");
                    Response.EnsureSuccessStatusCode();

                    string CurrenciesValues = await Response.Content.ReadAsStringAsync();
                    XDocument XmlCurrenciesValues = XDocument.Parse(CurrenciesValues);

                    Dictionary<string, Currency > CurrenciesToFindDict = new Dictionary<string, Currency>
                    {
                        { "Евро", Currency.Euro },
                        { "Доллар США", Currency.Dollar }
                    };

                    foreach (var CurrencyName in CurrenciesToFindDict.Keys)
                    {  
                        var CurrencyCode = XmlCurrenciesNames.Descendants("Item")
                                                 .Where(item => (string)item.Element("Name") == CurrencyName)
                                                 .Select(item => (string)item.Attribute("ID"))
                                                 .FirstOrDefault();

                        if (CurrencyCode != null)
                        {
                            var CurrencyValue = XmlCurrenciesValues.Descendants("Valute")
                                                     .Where(valute => (string)valute.Attribute("ID") == CurrencyCode)
                                                     .Select(valute => (string)valute.Element("Value"))
                                                     .FirstOrDefault();

                            CurrencyCourses.Add(new CurrencyCourse(CurrenciesToFindDict[CurrencyName], (int)float.Parse(CurrencyValue) * 100));
                        }
                    }
                    _context.CurrencyCourses.RemoveRange(_context.CurrencyCourses.ToList());
                    _context.CurrencyCourses.AddRange(CurrencyCourses);
                    _context.SaveChanges();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Неудачный запрос к ЦБ");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
