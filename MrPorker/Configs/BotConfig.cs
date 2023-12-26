namespace MrPorker.Configs
{
    public class BotConfig
    {
        public required string BotToken { get; set; }
        public required ulong GuildHideoutId { get; set; }
        public required ulong ChannelGeneralId { get; set; }
        public required ulong ChannelZooId { get; set; }

        public required string HoroscopeThumbnail { get; set; }
        public required string HoroscopeUrl { get; set; }
        public required string HoroscopeXPathDate { get; set; }
        public required string HoroscopeXPathSign { get; set; }
        public required string HoroscopeXPathHoroscope { get; set; }

        public required string TwitterLinkRegex { get; set; }
        public required string TwitterLinkReplacementHost { get; set; }
    }
}
