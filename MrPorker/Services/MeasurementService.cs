using MrPorker.Configs;
using MrPorker.Data.Dtos;
using System.Reflection;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Text.RegularExpressions;
using MrPorker.Data.Enums;

namespace MrPorker.Services
{
    public class MeasurementService(BotConfig botConfig, BotService botService, DatabaseService databaseService, AddymerBotService addymerBotService, AlexBotService alexBotService, EunoraBotService eunoraBotService, BluBotService bluBotService, HogHoganBotService hogHoganBotService)
    {
        private readonly BotConfig _botConfig = botConfig;
        private readonly BotService _botService = botService;
        private readonly AddymerBotService _addymerBotService = addymerBotService;
        private readonly AlexBotService _alexBotService = alexBotService;
        private readonly EunoraBotService _eunoraBotService = eunoraBotService;
        private readonly BluBotService _bluBotService = bluBotService;
        private readonly DatabaseService _databaseService = databaseService;
        private readonly HogHoganBotService _hogHoganBotService = hogHoganBotService;

        private readonly string _embedTemplateHtmlContent = File.ReadAllText(botConfig.EmbedTemplatePath);
        private readonly string _measurementTemplateHtmlContent = File.ReadAllText(botConfig.MeasurementTemplatePath);

        public async Task<IResult> AddMeasurementAsync(MeasurementDto measurementDto, Competitor competitor)
        {
            var previousMeasurement = await _databaseService.GetLatestMeasurementAsync(competitor);

            await GenerateUiImagesAsync(measurementDto, competitor);

            if (competitor == Competitor.Paul)
                measurementDto.Height = 177;

            if (competitor == Competitor.Alex)
                measurementDto.Height = 192;

            if (competitor == Competitor.Eunora)
                measurementDto.Height = 190;

            if (competitor == Competitor.Blu)
                measurementDto.Height = 175;

            if (competitor == Competitor.Addymer)
                measurementDto.Height = 175;

            measurementDto.Strength = CharacterRanking.CalculateStrengthScore(measurementDto);
            measurementDto.Endurance = CharacterRanking.CalculateEnduranceScore(measurementDto);
            measurementDto.Agility = CharacterRanking.CalculateAgilityScore(measurementDto);
            measurementDto.Overall = CharacterRanking.CalculateOverallScore(measurementDto, measurementDto.Strength, measurementDto.Endurance, measurementDto.Agility);

            await _databaseService.AddMeasurementAsync(measurementDto, competitor);
            await _hogHoganBotService.SendCompetitorUpdate(measurementDto, competitor, previousMeasurement);

            return Results.Ok();
        }

        private async Task GenerateUiImagesAsync(MeasurementDto measurementDto, Competitor competitor)
        {
            var measurementHistory = new MeasurementHistoryDto
            {
                Today = measurementDto,
                Yesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1, competitor),
                LastWeek = await _databaseService.GetXthMostRecentMeasurementAsync(7, competitor),
                LastMonth = await _databaseService.GetXthMostRecentMeasurementAsync(30, competitor),
                AllTime = await _databaseService.GetStartingMeasurement(competitor)
            };

            await new BrowserFetcher().DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = "/usr/bin/chromium-browser"
            });

            var measurementProperties = measurementDto.GetType().GetProperties();
            var primaryChunk = new List<PropertyInfo>();
            var secondaryChunk = new List<PropertyInfo>();

            var i = 0;
            while (primaryChunk.Count < 6 && i < measurementProperties.Length)
            {
                if (measurementProperties[i].PropertyType == typeof(float))
                    primaryChunk.Add(measurementProperties[i]);

                i++;
            }

            while (secondaryChunk.Count < 7 && i < measurementProperties.Length)
            {
                if (measurementProperties[i].PropertyType == typeof(float))
                    secondaryChunk.Add(measurementProperties[i]);

                i++;
            }

            // i don't know why i called these chunks i just like the word chunk
            await SendMessageForChunk(primaryChunk, measurementHistory, browser, competitor);
            await SendMessageForChunk(secondaryChunk, measurementHistory, browser, competitor);

            await browser.CloseAsync();
        }

        private async Task SendMessageForChunk(List<PropertyInfo> chunk, MeasurementHistoryDto measurementHistory, IBrowser browser, Competitor competitor)
        {
            var embedDocument = new HtmlDocument();
            embedDocument.LoadHtml(_embedTemplateHtmlContent);
            var embedNode = embedDocument.DocumentNode.SelectSingleNode("//div[@class='embed-container']");

            if (embedNode != null)
            {
                foreach (PropertyInfo property in chunk)
                {
                    if (property.PropertyType == typeof(float))
                    {
                        if (string.Equals(property.Name.ToLower(), "weight"))
                        {
                            if (competitor == Competitor.Addymer)
                                embedNode.InnerHtml += "<div class=\"title\"><img src=\"https://i.imgur.com/xyniH6c.png\"></img></div>\n";
                            else if (competitor == Competitor.Paul)
                                embedNode.InnerHtml += "<div class=\"title\"><img src=\"https://i.imgur.com/sF72DRf.png\"></img></div>\n";
                            else if (competitor == Competitor.Alex)
                                embedNode.InnerHtml += "<div class=\"title\"><img src=\"https://i.imgur.com/FAU0d5E.png\"></img></div>\n";
                            else if (competitor == Competitor.Eunora)
                                embedNode.InnerHtml += "<div class=\"title\"><img src=\"https://i.imgur.com/4FlSVKz.png\"></img></div>\n";
                            else if (competitor == Competitor.Blu)
                                embedNode.InnerHtml += "<div class=\"title\"><img src=\"https://i.imgur.com/NCrZxCC.png\"></img></div>\n";
                        }

                        if (!string.Equals(property.Name.ToLower(), "weight") && !string.Equals(property.Name.ToLower(), "bodywater"))
                            embedNode.InnerHtml += "<div class=\"top-spacer\"></div>\n";

                        float value = (float)(property.GetValue(measurementHistory.Today) ?? -1);

                        var statName = property.Name.ToLower();
                        var statNameFormatted = MeasurementThresholdConfig.GetFormattedNameForMeasurement(property.Name);
                        var category = _botConfig.GetMeasurementThresholdConfigByCompetitor(competitor).GetCategoryForMeasurement(property.Name, value).ToLower();
                        var categoryClass = Regex.Replace(_botConfig.GetMeasurementThresholdConfigByCompetitor(competitor).GetCategoryForMeasurement(property.Name, value).ToLower(), @"\s+", ""); ;
                        var stat = $"{value}{MeasurementThresholdConfig.GetUnitForMeasurement(property.Name)}";
                        var deltaYesterdayFormatted = GetFormattedDelta(property, measurementHistory.Yesterday, value);
                        var deltaLastWeekFormatted = GetFormattedDelta(property, measurementHistory.LastWeek, value);
                        var deltaLastMonthFormatted = GetFormattedDelta(property, measurementHistory.LastMonth, value);
                        var deltaTotalFormatted = GetFormattedDelta(property, measurementHistory.AllTime, value);

                        var measurementHtmlContent = _measurementTemplateHtmlContent
                            .Replace("{{STAT-NAME}}", statName)
                            .Replace("{{STAT-NAME-FORMATTED}}", statNameFormatted)
                            .Replace("{{CATEGORY}}", category)
                            .Replace("{{CATEGORY-CLASS}}", categoryClass)
                            .Replace("{{STAT}}", stat)
                            .Replace("{{1-DAY-DELTA}}", deltaYesterdayFormatted)
                            .Replace("{{7-DAY-DELTA}}", deltaLastWeekFormatted)
                            .Replace("{{30-DAY-DELTA}}", deltaLastMonthFormatted)
                            .Replace("{{TOTAL-DELTA}}", deltaTotalFormatted);

                        embedNode.InnerHtml += measurementHtmlContent + "\n";
                        embedNode.InnerHtml += "<div class=\"top-spacer\"></div>" + "\n";

                        if (!string.Equals(property.Name.ToLower(), "metabolicage"))
                            embedNode.InnerHtml += "<hr/>\n";
                    }
                }

                var page = await browser.NewPageAsync();

                // Set the viewport size
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 390,
                    Height = 1,
                });

                //File.WriteAllText("example.html", embedDocument.DocumentNode.OuterHtml);
                await page.SetContentAsync(embedDocument.DocumentNode.OuterHtml);

                await page.EvaluateExpressionAsync("document.body.style.background = 'transparent'");
                var screenshotOptions = new ScreenshotOptions
                {
                    FullPage = true,
                    Type = ScreenshotType.Png,
                    OmitBackground = true,
                };

                byte[] screenshotData = await page.ScreenshotDataAsync(screenshotOptions);
                var imageStream = new MemoryStream(screenshotData);

                // discord needs stream position to be reset to the beginning
                imageStream.Position = 0;

                if (competitor == Competitor.Addymer)
                    await _addymerBotService.SendFileToChannelAsync(imageStream, _botConfig.ChannelPorkboardId);
                else if (competitor == Competitor.Paul)
                    await _botService.SendFileToChannelAsync(imageStream, _botConfig.ChannelPorkboardId);
                else if (competitor == Competitor.Alex)
                    await _alexBotService.SendFileToChannelAsync(imageStream, _botConfig.ChannelPorkboardId);
                else if (competitor == Competitor.Eunora)
                    await _eunoraBotService.SendFileToChannelAsync(imageStream, _botConfig.ChannelPorkboardId);
                else if (competitor == Competitor.Blu)
                    await _bluBotService.SendFileToChannelAsync(imageStream, _botConfig.ChannelPorkboardId);
            }
        }

        private string GetFormattedDelta(PropertyInfo property, MeasurementDto? measurementDto, float valueToday)
        {
            if (measurementDto != null)
            {
                var valuePrevious = (float)(property.GetValue(measurementDto) ?? -1);
                if (valuePrevious != -1)
                {
                    var deltaYesterday = valueToday - valuePrevious;
                    return $"{(deltaYesterday < 0 ? "" : "+")}{(property.Name == "BasalMetabolicRate" || property.Name == "VisceralFat" || property.Name == "MetabolicAge" ? Math.Round(deltaYesterday, 0).ToString("0") : Math.Round(deltaYesterday, 2).ToString("0.00"))}{MeasurementThresholdConfig.GetUnitForMeasurement(property.Name)}";
                }
            }

            return "???";
        }
    }
}
