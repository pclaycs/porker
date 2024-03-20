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
            var score = measurement.MuscleMass * (176 / measurement.Height);

            return StandardizeAndScaleScore(score, 45, 65);
        }

        public static double CalculateEnduranceScore(MeasurementDto measurement)
        {
            var score = (100 - measurement.BodyFat) * (measurement.Age / measurement.MetabolicAge);

            return StandardizeAndScaleScore(score, 45, 110);
        }

        public static double CalculateAgilityScore(MeasurementDto measurement)
        {
            var score = (50 - measurement.BodyFat) * (measurement.MuscleMass / measurement.Weight);

            return StandardizeAndScaleScore(score, 3, 36);
        }

        public static double CalculateOverallScore(MeasurementDto measurement, double strengthScore, double enduranceScore, double agilityScore)
        {
            var score = ((strengthScore * 2) + (enduranceScore * 0.5) + (agilityScore * 0.5)) / 3;

            return StandardizeAndScaleScore(score, 1, 10000);
        }

        private static double StandardizeAndScaleScore(double score, double minScore, double maxScore)
        {
            var standardizedScore = (score - minScore) / (maxScore - minScore);
            return Math.Round(standardizedScore * 10000);
        }

        public static string GetGrade(int score)
        {
            if (score >= 9000)
                return "S";
            else if (score >= 8000)
                return "A";
            else if (score >= 7000)
                return "B";
            else if (score >= 6000)
                return "C";
            else if (score >= 4000)
                return "D";
            else if (score >= 2000)
                return "E";
            else
                return "F";
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

