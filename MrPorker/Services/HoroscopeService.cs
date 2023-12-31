using HtmlAgilityPack;
using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Extensions;

namespace MrPorker.Services
{
    public class HoroscopeService(IHttpClientFactory httpClientFactory, BotConfig config)
    {
        public async Task<HoroscopeDto?> GetHoroscopeAsync(int sign, bool isTomorrow = false)
        {
            var client = httpClientFactory.CreateClient("HoroscopeClient");
            var date = DateTime.Now.AddDays(isTomorrow ? -1 : -2).ToString("yyyyMMdd");

            var url = $"{config.HoroscopeUrl}?sign={sign}&laDate={date}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var horoscopeDto = new HoroscopeDto
                {
                    Date = htmlDoc.DocumentNode.SelectSingleNode(config.HoroscopeXPathDate)?.InnerText ?? string.Empty,
                    Sign = htmlDoc.DocumentNode.SelectSingleNode(config.HoroscopeXPathSign)?.InnerText.GetFirstWord() ?? string.Empty,
                    Horoscope = htmlDoc.DocumentNode.SelectSingleNode(config.HoroscopeXPathHoroscope)?.InnerText[3..] ?? string.Empty
                };

                if (DateTime.Parse(horoscopeDto.Date) != DateTime.Now && !isTomorrow)
                    return await GetHoroscopeAsync(sign, isTomorrow: true);

                return horoscopeDto;
            }

            return null;
        }
    }
}
