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

        public async Task<HoroscopeDto?> GetHoroscopeAsync(int sign, bool isTomorrow = false)
        {
            var client = _httpClientFactory.CreateClient("HoroscopeClient");
            var date = DateTime.Now.AddDays(isTomorrow ? -1 : -2).ToString("yyyyMMdd");

            var url = $"https://www.horoscope.com/us/horoscopes/general/horoscope-general-daily-tomorrow.aspx?sign={sign}&laDate={date}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var horoscopeDto = new HoroscopeDto
                {
                    Date = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/main/div[1]/p[1]/strong")?.InnerText ?? string.Empty,
                    Sign = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[2]/div/div/a/h1")?.InnerText.GetFirstWord() ?? string.Empty,
                    Horoscope = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/main/div[1]/p[1]/text()")?.InnerText[3..] ?? string.Empty
                };

                if (DateTime.Parse(horoscopeDto.Date) != DateTime.Now && !isTomorrow)
                    return await GetHoroscopeAsync(sign, isTomorrow: true);

                return horoscopeDto;
            }

            return null;
        }
    }
}
