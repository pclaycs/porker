using MrPorker.Data.Enums;

namespace MrPorker.Configs
{
    public class BotConfig
    {
        public required string FontsDirectory { get; set; } // unused?

        public required ulong PaulDiscordId { get; set; }
        public required ulong AddymerDiscordId { get; set; }
        public required ulong AlexDiscordId { get; set; }
        public required ulong EunoraDiscordId { get; set; }
        public required ulong BluDiscordId { get; set; }
        
        public required ulong ParticipantRoleId { get; set; }
        public required ulong TalkerRoleId { get; set; }

        //public required ulong ShameRoleId { get; set; }

        public required string BotToken { get; set; }
        public required string AddymerBotToken { get; set; }
        public required string AlexBotToken { get; set; }
        public required string EunoraBotToken { get; set; }
        public required string BluBotToken { get; set; }
        public required string PersonalTrainerBotToken { get; set; }
        public required string HogHoganBotToken { get; set; }
        
        public required ulong GuildHideoutId { get; set; }
        public required ulong ChannelGeneralId { get; set; }
        public required ulong ChannelPorkboardId { get; set; }
        public required ulong ChannelPorkCentralId { get; set; }

        //public required ulong GuildPorkerId { get; set; }
        //public required ulong ChannelPorkerMainId { get; set; }

        public required string HoroscopeThumbnail { get; set; }
        public required string HoroscopeUrl { get; set; }
        public required string HoroscopeXPathDate { get; set; }
        public required string HoroscopeXPathSign { get; set; }
        public required string HoroscopeXPathHoroscope { get; set; }

        public required string TwitterLinkRegex { get; set; }
        public required string TwitterLinkReplacementHost { get; set; }

        public required string FirebaseUrl { get; set; }
        public required int FirebasePollingInSeconds { get; set; }
        public required string FirebaseDatabaseSecret { get; set; }
        public required string FirebaseMessagingToken { get; set; }
        public required string FirebaseServiceAccountKeyJsonFilePath { get; set; }

        public required string EmbedTemplatePath { get; set; }
        public required string MeasurementTemplatePath { get; set; }

        public required string EmailAddress { get; set; }
        public required string EmailPassword { get; set; }
        public required string EmailEunora { get; set; }
        public required string EmailBlu { get; set; }
        public required int EmailPollingInSeconds { get; set; }


        public MeasurementThresholdConfig MeasurementThresholds { get; set; }
        public MeasurementThresholdConfig AddymerMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig AlexMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig EunoraMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig BluMeasurementThresholds { get; set; }

        public MeasurementThresholdConfig GetMeasurementThresholdConfigByCompetitor(Competitor competitor)
        {
            if (competitor == Competitor.Paul)
                return MeasurementThresholds;

            if (competitor == Competitor.Addymer)
                return AddymerMeasurementThresholds;

            if (competitor == Competitor.Alex)
                return AlexMeasurementThresholds;

            if (competitor == Competitor.Eunora)
                return EunoraMeasurementThresholds;

            if (competitor == Competitor.Blu)
                return BluMeasurementThresholds;

            return new MeasurementThresholdConfig();
        }
    }
}
