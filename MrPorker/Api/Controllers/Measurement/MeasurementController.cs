using MrPorker.Configs;
using MrPorker.Data.Dtos;
using MrPorker.Services;
using System.Reflection;
using HtmlAgilityPack;
using System.IO;
using PuppeteerSharp;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;



namespace MrPorker.Api.Controllers.Measurement
{
    public class MeasurementController
    {
        private readonly BotConfig _botConfig;
        private readonly BotService _botService;
        private readonly DatabaseService _databaseService;

        private readonly string _embedTemplateHtmlContent;
        private readonly string _measurementTemplateHtmlContent;

        public MeasurementController(BotConfig botConfig, BotService botService, DatabaseService databaseService)
        {
            _botConfig = botConfig;
            _botService = botService;
            _databaseService = databaseService;

            // TODO CONFIGURABLE PATHS
            var embedTemplateFilePath = "D:\\Development\\Projects\\PorkerSuite\\porker\\MrPorker\\Api\\Controllers\\Measurement\\EmbedTemplate.html";
            _embedTemplateHtmlContent = File.ReadAllText(embedTemplateFilePath);

            // TODO CONFIGURABLE PATHS
            var measurementTemplateFilePath = "D:\\Development\\Projects\\PorkerSuite\\porker\\MrPorker\\Api\\Controllers\\Measurement\\MeasurementTemplate.html";
            _measurementTemplateHtmlContent = File.ReadAllText(measurementTemplateFilePath);
        }


        public async Task<IResult> AddMeasurementAsync(MeasurementDto measurementDto)
        {
            await GenerateUiImagesAsync(measurementDto);
            await _databaseService.AddMeasurementAsync(measurementDto);

            return Results.Ok();
        }

        public async Task GenerateUiImagesAsync(MeasurementDto measurementDto)
        {
            var measurementHistory = new MeasurementHistory
            {
                Today = measurementDto,
                Yesterday = await _databaseService.GetXthMostRecentMeasurementAsync(1),
                LastWeek = await _databaseService.GetXthMostRecentMeasurementAsync(7),
                LastMonth = await _databaseService.GetXthMostRecentMeasurementAsync(30),
                AllTime = await _databaseService.GetStartingMeasurement()
            };

            await new BrowserFetcher().DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
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
            await SendMessageForChunk(primaryChunk, measurementHistory, browser);
            await SendMessageForChunk(secondaryChunk, measurementHistory, browser);

            await browser.CloseAsync();
        }

        private async Task SendMessageForChunk(List<PropertyInfo> chunk, MeasurementHistory measurementHistory, IBrowser browser)
        {
            var embedDocument = new HtmlDocument();
            embedDocument.LoadHtml(_embedTemplateHtmlContent);
            var embedNode = embedDocument.DocumentNode.SelectSingleNode("//div[@class='embed-container']");

            if (embedNode != null )
            {
                foreach (PropertyInfo property in chunk)
                {
                    if (property.PropertyType == typeof(float))
                    {
                        float value = (float)(property.GetValue(measurementHistory.Today) ?? -1);

                        var statName = property.Name.ToLower();
                        var statNameFormatted = MeasurementThresholdConfig.GetFormattedNameForMeasurement(property.Name);
                        var category = _botConfig.MeasurementThresholds.GetCategoryForMeasurement(property.Name, value).ToLower();
                        var categoryClass = Regex.Replace(_botConfig.MeasurementThresholds.GetCategoryForMeasurement(property.Name, value).ToLower(), @"\s+", "");;
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

                await _botService.SendFileToGeneralAsync(imageStream);
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
