using HtmlAgilityPack;
using MrPorker.Commands.Horoscope;
using MrPorker.Configs;
using MrPorker.Extensions;

namespace MrPorker.Services
{
    public class HoroscopeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BotConfig _botConfig;

        public HoroscopeService(IHttpClientFactory httpClientFactory, BotConfig config)
        {
            _httpClientFactory = httpClientFactory;
            _botConfig = config;
        }

        public async Task<HoroscopeDto?> GetHoroscopeAsync(int sign, bool isTomorrow = false)
        {
            var client = _httpClientFactory.CreateClient("HoroscopeClient");
            var date = DateTime.Now.AddDays(isTomorrow ? -1 : -2).ToString("yyyyMMdd");

            var url = $"{_botConfig.HoroscopeUrl}?sign={sign}&laDate={date}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var horoscopeDto = new HoroscopeDto
                {
                    Date = htmlDoc.DocumentNode.SelectSingleNode(_botConfig.HoroscopeXPathDate)?.InnerText ?? string.Empty,
                    Sign = htmlDoc.DocumentNode.SelectSingleNode(_botConfig.HoroscopeXPathSign)?.InnerText.GetFirstWord() ?? string.Empty,
                    Horoscope = htmlDoc.DocumentNode.SelectSingleNode(_botConfig.HoroscopeXPathHoroscope)?.InnerText[3..] ?? string.Empty
                };

                if (DateTime.Parse(horoscopeDto.Date) != DateTime.Now && !isTomorrow)
                    return await GetHoroscopeAsync(sign, isTomorrow: true);

                return horoscopeDto;
            }

            return null;
        }
    }
}
