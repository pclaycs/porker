using Discord.WebSocket;
using MrPorker.Data.Dtos;

namespace MrPorker
{
    public class ScoreHolder
    {
        public required IEnumerable<KeyValuePair<SocketGuildUser, double>> Strength;
        public required IEnumerable<KeyValuePair<SocketGuildUser, double>> Endurance;
        public required IEnumerable<KeyValuePair<SocketGuildUser, double>> Agility;
        public required IEnumerable<KeyValuePair<SocketGuildUser, double>> Overall;
    }

    public class CharacterRanking
    {
        public static double CalculateStrengthScore(MeasurementDto measurement)
        {
            var score = measurement.FatFreeBodyWeight * (1 - measurement.BodyFat / 100) * (measurement.BodyMassIndex / 25);

            return StandardizeAndScaleScore(score, 23, 80);
        }

        public static double CalculateEnduranceScore(MeasurementDto measurement)
        {
            double heightM = measurement.Height / 100.0;
            var score = (measurement.BasalMetabolicRate + measurement.BodyWater * 10 + (measurement.FatFreeBodyWeight / Math.Pow(heightM, 2))) * (1 - measurement.BodyFat / 100) / 100;

            return StandardizeAndScaleScore(score, 12, 26);
        }

        public static double CalculateAgilityScore(MeasurementDto measurement)
        {
            var score = Math.Sqrt(measurement.MuscleMass) / Math.Sqrt(measurement.Weight) * 100 * Math.Exp(-measurement.BodyFat / 100) * (100 - measurement.BodyMassIndex) + 50;

            return StandardizeAndScaleScore(score, 4000, 7100);
        }

        public static double CalculateOverallScore(MeasurementDto measurement, double strengthScore, double enduranceScore, double agilityScore)
        {
            double heightM = measurement.Height / 100.0;
            var score = (strengthScore * 0.7) + (enduranceScore * 0.15) + (agilityScore * 0.15) + (measurement.MuscleMass / Math.Pow(heightM, 2) * 0.6) + (measurement.BodyFat * 0.9);

            return StandardizeAndScaleScore(score, 900, 10000);
        }

        private static double StandardizeAndScaleScore(double score, double minScore, double maxScore)
        {
            var standardizedScore = (score - minScore) / (maxScore - minScore);
            return Math.Round(standardizedScore * 10000);
        }

        public static string GetGrade(int score)
        {
            return score switch
            {
                >= 9000 => "S",
                >= 8000 => "A",
                >= 7000 => "B",
                >= 6000 => "C",
                >= 4000 => "D",
                >= 2000 => "E",
                _ => "F"
            };
        }

        public static ScoreHolder PopulateScoreHolder(Dictionary<SocketGuildUser, MeasurementDto> characters)
        {
            var strengthScores = new Dictionary<SocketGuildUser, double>();
            var enduranceScores = new Dictionary<SocketGuildUser, double>();
            var agilityScores = new Dictionary<SocketGuildUser, double>();
            var overallScores = new Dictionary<SocketGuildUser, double>();

            foreach (var (name, data) in characters)
            {
                strengthScores.Add(name, data.Strength);
                enduranceScores.Add(name, data.Endurance);
                agilityScores.Add(name, data.Agility);
                overallScores.Add(name, data.Overall);
            }

            return new ScoreHolder()
            {
                Strength = strengthScores.OrderByDescending(x => x.Value),
                Endurance = enduranceScores.OrderByDescending(x => x.Value),
                Agility = agilityScores.OrderByDescending(x => x.Value),
                Overall = overallScores.OrderByDescending(x => x.Value),
            };
        }
    }
}

