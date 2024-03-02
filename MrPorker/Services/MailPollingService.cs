using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Data.Enums;
using System.Globalization;
using System.Text;

namespace MrPorker.Services
{
    public class MailPollingService(MeasurementService measurementService, BotConfig botConfig)
    {
        private ImapClient _client = new ImapClient();
        private TimeSpan _pollingInterval;

        public async Task StartPollingAsync(CancellationToken cancellationToken)
        {
            _pollingInterval = TimeSpan.FromSeconds(botConfig.EmailPollingInSeconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                await ConsumeEmailsAsync();
                await Task.Delay(_pollingInterval);
            }
        }

        private async Task ConsumeEmailsAsync()
        {
            try
            {
                _client.Connect("imap.gmail.com", 993, true); // Use SSL

                // Use your Gmail address and app-specific password (or account password)
                _client.Authenticate(botConfig.EmailAddress, botConfig.EmailPassword);

                // The Inbox folder is always available on all IMAP servers...
                _client.Inbox.Open(FolderAccess.ReadWrite);

                // Search the inbox using the combined query
                var uids = _client.Inbox.Search(SearchQuery.All);

                foreach (var uid in uids)
                {
                    try
                    {
                        var message = _client.Inbox.GetMessage(uid);

                        if (message.From.Mailboxes.FirstOrDefault()?.Address == botConfig.EmailEunora || message.From.Mailboxes.FirstOrDefault()?.Address == botConfig.EmailBlu)
                        {
                            foreach (var attachment in message.Attachments)
                            {
                                if (attachment is MimePart part && part.FileName.EndsWith(".csv"))
                                {
                                    using var memoryStream = new MemoryStream();

                                    part.Content.DecodeTo(memoryStream);
                                    var content = Encoding.UTF8.GetString(memoryStream.ToArray());

                                    // Process your MeasurementDto object as needed
                                    if (message.From.Mailboxes.FirstOrDefault()?.Address == botConfig.EmailEunora)
                                    {
                                        var measurementDto = ParseData(content, false);
                                        if (measurementDto != null)
                                            await measurementService.AddMeasurementAsync(measurementDto, Competitor.Eunora);
                                    }
                                    else if (message.From.Mailboxes.FirstOrDefault()?.Address == botConfig.EmailBlu)
                                    {
                                        var measurementDto = ParseData(content, true);
                                        if (measurementDto != null)
                                            await measurementService.AddMeasurementAsync(measurementDto, Competitor.Blu);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        _client.Inbox.AddFlags(uid, MessageFlags.Deleted, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                try
                {
                    _client.Inbox.Expunge();
                    _client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }


        public MeasurementDto? ParseData(string data, bool isQueenslander)
        {
            var rows = data.Split('\n');
            var headers = new string[]
            {
            "timestamp", "weight", "bodyMassIndex", "bodyFat", "fatFreeBodyWeight",
            "subcutaneousFat", "visceralFat", "bodyWater", "skeletalMuscle", "muscleMass",
            "boneMass", "protein", "basalMetabolicRate", "metabolicAge", "remarks"
            };
            var values = rows[1].Split(',');
            var result = headers.Zip(values, (header, value) => new { header, value })
                                .ToDictionary(x => x.header, x => x.value);

            if (result.TryGetValue("timestamp", out var timestamp))
            {
                var dto = new MeasurementDto();
                DateTime localDateTime;

                try
                {
                    // Try the first format
                    var formatter = new CultureInfo("en-AU").DateTimeFormat;
                    formatter.ShortDatePattern = "yyyy-MM-dd";
                    formatter.LongTimePattern = "HH:mm:ss";
                    localDateTime = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", formatter);
                }
                catch (FormatException)
                {
                    try
                    {
                        // Try the second format if the first one fails
                        var secondFormatter = new CultureInfo("en-AU").DateTimeFormat;
                        secondFormatter.ShortDatePattern = "dd MMM yyyy 'at' HH:mm:ss";
                        secondFormatter.LongTimePattern = "HH:mm:ss";
                        localDateTime = DateTime.ParseExact(timestamp, "dd MMM yyyy 'at' HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                }

                if (isQueenslander)
                {
                    var goldCoastTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                    var goldCoastDateTime = TimeZoneInfo.ConvertTime(localDateTime, goldCoastTimeZone);
                    dto.Timestamp = new DateTimeOffset(goldCoastDateTime).ToUnixTimeSeconds();
                }
                else
                {
                    var sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
                    var sydneyDateTime = TimeZoneInfo.ConvertTime(localDateTime, sydneyTimeZone);

                    dto.Timestamp = new DateTimeOffset(sydneyDateTime).ToUnixTimeSeconds();
                }

                dto.Weight = float.Parse(result["weight"]);
                dto.BodyMassIndex = float.Parse(result["bodyMassIndex"]);
                dto.BodyFat = float.Parse(result["bodyFat"]);
                dto.FatFreeBodyWeight = float.Parse(result["fatFreeBodyWeight"]);
                dto.SubcutaneousFat = float.Parse(result["subcutaneousFat"]);
                dto.VisceralFat = float.Parse(result["visceralFat"]);
                dto.BodyWater = float.Parse(result["bodyWater"]);
                dto.SkeletalMuscle = float.Parse(result["skeletalMuscle"]);
                dto.MuscleMass = float.Parse(result["muscleMass"]);
                dto.BoneMass = float.Parse(result["boneMass"]);
                dto.Protein = float.Parse(result["protein"]);
                dto.BasalMetabolicRate = float.Parse(result["basalMetabolicRate"]);
                dto.MetabolicAge = float.Parse(result["metabolicAge"]);
                dto.Remarks = result.TryGetValue("remarks", out var remarks) ? remarks : null;

                return dto;
            }

            return null;
        }
    }
}
