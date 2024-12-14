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
        public required ulong BraydenDiscordId { get; set; }
        public required ulong CbriDiscordId { get; set; }

        public required ulong PaulPorkerId { get; set; }
        public required ulong AddymerPorkerId { get; set; }
        public required ulong AlexPorkerId { get; set; }
        public required ulong EunoraPorkerId { get; set; }
        public required ulong BluPorkerId { get; set; }
        public required ulong BraydenPorkerId { get; set; }
        public required ulong CbriPorkerId { get; set; }

        public required ulong ShameRoleId { get; set; }

        public required ulong Rank1RoleId { get; set; }
        public required ulong Rank2RoleId { get; set; }
        public required ulong Rank3RoleId { get; set; }
        public required ulong Rank4RoleId { get; set; }
        public required ulong Rank5RoleId { get; set; }
        public required ulong Rank6RoleId { get; set; }
        public required ulong Rank7RoleId { get; set; } 

        public required string BotToken { get; set; }
        public required string AddymerBotToken { get; set; }
        public required string AlexBotToken { get; set; }
        public required string EunoraBotToken { get; set; }
        public required string BluBotToken { get; set; }
        public required string BraydenBotToken { get; set; }
        public required string CbriBotToken { get; set; }
        public required string PersonalTrainerBotToken { get; set; }
        public required string HogHoganBotToken { get; set; }

        public required ulong GuildPorkOffId { get; set; }

        public required ulong ChannelLeaderboardId { get; set; }
        public required ulong ChannelMatchHistoryId { get; set; }
        public required ulong ChannelPorkOffGeneralId { get; set; }

        public required ulong ChannelHideoutId { get; set; }

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
        public required string EmailAddymer { get; set; }
        public required string EmailAlex { get; set; }
        public required string EmailPaul { get; set; }
        public required string EmailBrayden { get; set; }
        public required string EmailCbri { get; set; }
        public required int EmailPollingInSeconds { get; set; }


        public MeasurementThresholdConfig MeasurementThresholds { get; set; }
        public MeasurementThresholdConfig AddymerMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig AlexMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig EunoraMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig BluMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig BraydenMeasurementThresholds { get; set; }
        public MeasurementThresholdConfig CbriMeasurementThresholds { get; set; }

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

            if (competitor == Competitor.Brayden)
                return BraydenMeasurementThresholds;

            if (competitor == Competitor.Cbri)
                return CbriMeasurementThresholds;

            return new MeasurementThresholdConfig();
        }
    }
}
