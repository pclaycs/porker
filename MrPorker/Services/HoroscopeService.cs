using HtmlAgilityPack;
using MrPorker.Commands.Horoscope;
using MrPorker.Extensions;

namespace MrPorker.Services
{
    public class HoroscopeService
    {
        private readonly IHttpClientFactory _httpClientFactory;



        public HoroscopeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HoroscopeDto?> GetHoroscopeAsync(int sign)
        {
            var client = _httpClientFactory.CreateClient("HoroscopeClient");
            var date = DateTime.Now.ToString("yyyyMMdd");

            var response = await client.GetAsync($"https://www.horoscope.com/us/horoscopes/general/horoscope-archive.aspx?sign={sign}&laDate={date}");

            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                return new HoroscopeDto
                {
                    Date = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/main/div[1]/p[1]/strong")?.InnerText ?? string.Empty,
                    Sign = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/a/h1")?.InnerText.GetFirstWord() ?? string.Empty,
                    Horoscope = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/main/div[1]/p[1]/text()")?.InnerText[3..] ?? string.Empty
                };
            }

            return null;
        }
    }
}
