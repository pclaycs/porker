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
        private static double CalculateFFMI(double heightCm, double fatFreeBodyWeightKg)
        {
            double heightM = heightCm / 100.0;
            return fatFreeBodyWeightKg / (heightM * heightM);
        }

        private static double CalculateMMI(double heightCm, double muscleMassKg)
        {
            double heightM = heightCm / 100.0;
            return muscleMassKg / (heightM * heightM);
        }

        public static double CalculateStrengthScore(MeasurementDto dto)
        {
            double heightCm = dto.Height;
            double fatFreeBodyWeightKg = dto.FatFreeBodyWeight;
            double muscleMassKg = dto.MuscleMass;

            double FFMI = CalculateFFMI(heightCm, fatFreeBodyWeightKg);
            double MMI = CalculateMMI(heightCm, muscleMassKg);

            // Adjusted parameters to spread out scores
            double x0_FFMI = 20.0;
            double k_FFMI = 0.6; // Increased from 0.4 to 0.6
            double exponent_FFMI = -k_FFMI * (FFMI - x0_FFMI);
            double FFMI_Score = 10000 / (1 + Math.Exp(exponent_FFMI));

            double x0_MMI = 17.0;
            double k_MMI = 0.6; // Increased from 0.4 to 0.6
            double exponent_MMI = -k_MMI * (MMI - x0_MMI);
            double MMI_Score = 10000 / (1 + Math.Exp(exponent_MMI));

            // Strength Score
            return (FFMI_Score + MMI_Score) / 2.0;
        }

        public static double CalculateAgilityScore(MeasurementDto dto)
        {
            double bodyFatPercentage = dto.BodyFat;
            double subcutaneousFatPercentage = dto.SubcutaneousFat;
            double visceralFat = dto.VisceralFat;

            // Agility Score calculations remain the same
            double x0_BF = 18.0;
            double k_BF = 0.3;
            double exponent_BF = k_BF * (bodyFatPercentage - x0_BF);
            double BF_Score = 10000 / (1 + Math.Exp(exponent_BF));

            double x0_SF = 15.0;
            double k_SF = 0.3;
            double exponent_SF = k_SF * (subcutaneousFatPercentage - x0_SF);
            double SF_Score = 10000 / (1 + Math.Exp(exponent_SF));

            double x0_VF = 10.0;
            double k_VF = 0.5;
            double exponent_VF = k_VF * (visceralFat - x0_VF);
            double VF_Score = 10000 / (1 + Math.Exp(exponent_VF));

            // Agility Score
            return (BF_Score + SF_Score + VF_Score) / 3.0;
        }

        public static double CalculateEnduranceScore(MeasurementDto dto)
        {
            double bodyWaterPercentage = dto.BodyWater;
            double weightKg = dto.Weight;
            double heightCm = dto.Height;
            float age = dto.Age;
            double basalMetabolicRate = dto.BasalMetabolicRate;
            double proteinPercentage = dto.Protein;

            // Body Water Score (remains the same)
            double x0_BW = 60.0;
            double k_BW = 0.4;
            double exponent_BW = -k_BW * (bodyWaterPercentage - x0_BW);
            double BW_Score = 10000 / (1 + Math.Exp(exponent_BW));

            // Predicted BMR using Mifflin-St Jeor Equation
            double predictedBMR = (10 * weightKg) + (6.25 * heightCm) - (5 * age) + 5;

            // BMR Ratio
            double BMR_Ratio = basalMetabolicRate / predictedBMR;

            // BMR Score
            double x0_BMR = 1.0; // Midpoint at BMR Ratio of 1
            double k_BMR = 10.0; // Increased steepness to spread out scores
            double exponent_BMR = -k_BMR * (BMR_Ratio - x0_BMR);
            double BMR_Score = 10000 / (1 + Math.Exp(exponent_BMR));

            // Protein Percentage Score (remains the same)
            double x0_Protein = 18.0;
            double k_Protein = 0.5;
            double exponent_Protein = -k_Protein * (proteinPercentage - x0_Protein);
            double Protein_Score = 10000 / (1 + Math.Exp(exponent_Protein));

            // Endurance Score
            return (BW_Score + BMR_Score + Protein_Score) / 3.0;
        }

        public static double CalculateOverallScore(MeasurementDto dto, double strengthScore, double enduranceScore, double agilityScore)
        {
            // Fight Score is the average of Strength, Agility, and Endurance
            return strengthScore * 0.625 + agilityScore * 0.125 + enduranceScore * 0.25;
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

        //private static double CalculateAdjustedBmrScore(double bmr, double bmi)
        //{
        //    double baseScore = LinearScore(bmr, 1200, 2500, 0, 10000);

        //    if (bmi < 18.5) return baseScore * 0.9; // Underweight
        //    if (bmi < 25) return baseScore; // Normal weight
        //    if (bmi < 30) return baseScore * 0.95; // Overweight
        //    return baseScore * 0.9; // Obesity
        //}

        //public static double CalculateEnduranceScore(MeasurementDto dto)
        //{
        //    double bodyFatScore = CalculateBodyFatScore(dto.BodyFat);
        //    double bodyWaterScore = CalculateBodyWaterScore(dto.BodyWater);
        //    //double basalMetabolicRateScore = CalculateBasalMetabolicRateScore(dto.BasalMetabolicRate * (177 / dto.Height));

        //    double adjustedBmrScore = CalculateAdjustedBmrScore(dto.BasalMetabolicRate, dto.BodyMassIndex);

        //    return 0.4 * bodyFatScore + 0.3 * bodyWaterScore + 0.3 * adjustedBmrScore;

        //    //return 0.4 * bodyFatScore + 0.3 * bodyWaterScore + 0.3 * basalMetabolicRateScore;
        //}

        //public static double CalculateStrengthScore(MeasurementDto dto)
        //{
        //    double muscleMassScore = CalculateMuscleMassScore(dto.MuscleMass * (177 / dto.Height));

        //    double boneMassScore = CalculateBoneMassScore(dto.BoneMass * (177 / dto.Height));

        //    double proteinScore = CalculateProteinScore(dto.Protein);

        //    return 0.7 * muscleMassScore + 0.2 * proteinScore + 0.1 * boneMassScore;
        //}

        //public static double CalculateAgilityScore(MeasurementDto dto)
        //{
        //    double subcutaneousFatScore = CalculateSubcutaneousFatScore(dto.SubcutaneousFat);
        //    double visceralFatScore = CalculateVisceralFatScore(dto.VisceralFat);
        //    double skeletalMuscleScore = CalculateSkeletalMuscleScore(dto.SkeletalMuscle);

        //    return 0.4 * subcutaneousFatScore + 0.35 * visceralFatScore + 0.25 * skeletalMuscleScore;
        //}

        //public static double CalculateOverallScore(MeasurementDto dto, double strengthScore, double enduranceScore, double agilityScore)
        //{
        //    return 0.3 * enduranceScore + 0.6 * strengthScore + 0.1 * agilityScore;
        //}

        //private static double CalculateBodyFatScore(double value)
        //{
        //    if (value < 6) return PenaltyScore(value, 2, 6, 7500); // Below Essential Fat
        //    if (value <= 13) return LinearScore(value, 6, 13, 7500, 10000); // Athletes
        //    if (value <= 17) return LinearScore(value, 13, 17, 9000, 5500); // Healthy
        //    if (value <= 25) return LinearScore(value, 17, 25, 5500, 3000); // Acceptable
        //    return PenaltyScore(value, 25, 35, 3000); // Obesity
        //}

        //private static double CalculateBodyWaterScore(double value)
        //{
        //    if (value < 50) return LinearScore(value, 40, 50, 0, 4000); // Low
        //    if (value < 65) return LinearScore(value, 50, 65, 4000, 8000); // Normal
        //    return LinearScore(value, 65, 80, 8000, 10000); // High
        //}

        //private static double CalculateBasalMetabolicRateScore(double value)
        //{
        //    return value >= 1752 ? 10000 : LinearScore(value, 1200, 1752, 0, 10000);
        //}

        //private static double CalculateMuscleMassScore(double value)
        //{
        //    if (value < 49.4) return LinearScore(value, 30, 49.4, 0, 4000); // Inadequate
        //    if (value < 59.4) return LinearScore(value, 49.4, 59.4, 4000, 8000); // Standard
        //    return LinearScore(value, 59.4, 80, 8000, 10000); // Adequate
        //}

        //private static double CalculateSkeletalMuscleScore(double value)
        //{
        //    if (value < 49) return LinearScore(value, 30, 49, 0, 4000); // Low
        //    if (value < 59) return LinearScore(value, 49, 59, 4000, 8000); // Standard
        //    return LinearScore(value, 59, 70, 8000, 10000); // High
        //}

        //private static double CalculateBoneMassScore(double value)
        //{
        //    if (value < 2.67) return LinearScore(value, 1.5, 2.67, 0, 4000); // Below Average
        //    if (value < 4.44) return LinearScore(value, 2.67, 4.44, 4000, 8000); // Average
        //    return LinearScore(value, 4.44, 6, 8000, 10000); // Above Average
        //}

        //private static double CalculateSubcutaneousFatScore(double value)
        //{
        //    if (value < 8.6) return LinearScore(value, 2, 8.6, 10000, 8000); // Low
        //    if (value < 16.7) return LinearScore(value, 8.6, 16.7, 8000, 4000); // Standard
        //    return PenaltyScore(value, 16.7, 30, 4000); // High
        //}

        //private static double CalculateVisceralFatScore(double value)
        //{
        //    if (value < 6) return LinearScore(value, 1, 6, 10000, 8000); // Excellent
        //    if (value < 11) return LinearScore(value, 6, 11, 8000, 6000); // Acceptable
        //    if (value < 15) return LinearScore(value, 11, 15, 6000, 4000); // High
        //    return PenaltyScore(value, 15, 20, 4000); // Excessive
        //}

        //private static double CalculateProteinScore(double value)
        //{
        //    if (value < 16) return LinearScore(value, 10, 16, 0, 4000); // Inadequate
        //    if (value < 18) return LinearScore(value, 16, 18, 4000, 8000); // Standard
        //    return LinearScore(value, 18, 25, 8000, 10000); // Adequate
        //}

        //private static double LinearScore(double value, double minValue, double maxValue, double minScore, double maxScore)
        //{
        //    if (value <= minValue) return minScore;
        //    if (value >= maxValue) return maxScore;
        //    return minScore + (value - minValue) * (maxScore - minScore) / (maxValue - minValue);
        //}

        //private static double PenaltyScore(double value, double minValue, double maxValue, double maxScore)
        //{
        //    if (value <= minValue) return maxScore;
        //    if (value >= maxValue) return 0;
        //    return maxScore - (value - minValue) * maxScore / (maxValue - minValue);
        //}

        ////public static double CalculateStrengthScore(MeasurementDto measurement)
        ////{
        ////    double powerScore = (measurement.MuscleMass / Math.Pow(measurement.Height / 100, 2)) * 4000;
        ////    return powerScore;
        ////}

        ////public static double CalculateEnduranceScore(MeasurementDto measurement)
        ////{
        ////    double enduranceScore = (measurement.BasalMetabolicRate * (1 - measurement.BodyFat / 100)) * 5;
        ////    return enduranceScore;
        ////}

        ////public static double CalculateAgilityScore(MeasurementDto measurement)
        ////{
        ////    double agilityScore = (measurement.SkeletalMuscle * (1 - measurement.VisceralFat / 20)) * 200;
        ////    return agilityScore;
        ////}

        ////public static double CalculateOverallScore(MeasurementDto measurement, double powerScore, double enduranceScore, double agilityScore)
        ////{
        ////    double overallScore = (powerScore * 0.4) + (enduranceScore * 0.3) + (agilityScore * 0.3);
        ////    return overallScore;
        ////}

        ////public static double CalculateEnduranceScore(MeasurementDto dto)
        ////{
        ////    var bodyFatScore = CalculateScoreForRange(dto.BodyFat, new float[] { 6, 13, 17, 25 }, new float[] { 8001, 6001, 4001, 2001 });
        ////    var bodyWaterScore = CalculateScoreForRange(dto.BodyWater, new float[] { 50, 65 }, new float[] { 3334, 6667 });
        ////    var bmrScore = dto.BasalMetabolicRate >= 1752 ? (dto.BasalMetabolicRate - 1752) / (2500 - 1752) * (10000 - 5001) + 5001 : (dto.BasalMetabolicRate - 1000) / (1752 - 1000) * 5000;

        ////    return (bodyFatScore + bodyWaterScore + bmrScore) / 3;
        ////}

        ////public static double CalculateStrengthScore(MeasurementDto dto)
        ////{
        ////    var muscleMassScore = CalculateScoreForRange(dto.MuscleMass, new float[] { 49.40f, 59.40f }, new float[] { 3334, 6667 });
        ////    var skeletalMuscleScore = CalculateScoreForRange(dto.SkeletalMuscle, new float[] { 49, 59 }, new float[] { 3334, 6667 });
        ////    var boneMassScore = CalculateScoreForRange(dto.BoneMass, new float[] { 2.67f, 4.44f }, new float[] { 3334, 6667 });

        ////    return (muscleMassScore + skeletalMuscleScore + boneMassScore) / 3;
        ////}

        ////public static double CalculateAgilityScore(MeasurementDto dto)
        ////{
        ////    var subcutaneousFatScore = CalculateInverseScoreForRange(dto.SubcutaneousFat, new float[] { 8.6f, 16.7f }, new float[] { 6667, 3334 });
        ////    var visceralFatScore = CalculateInverseScoreForRange(dto.VisceralFat, new float[] { 6, 11, 15 }, new float[] { 7501, 5001, 2501 });
        ////    var proteinScore = CalculateScoreForRange(dto.Protein, new float[] { 16, 18 }, new float[] { 3334, 6667 });

        ////    return (subcutaneousFatScore + visceralFatScore + proteinScore) / 3;
        ////}

        ////public static double CalculateOverallScore(MeasurementDto dto, double enduranceStaminaScore, double strengthPowerScore, double agilitySpeedScore)
        ////{
        ////    //var enduranceStaminaScore = CalculateEnduranceScore(dto);
        ////    //var strengthPowerScore = CalculateStrengthScore(dto);
        ////    //var agilitySpeedScore = CalculateAgilityScore(dto);

        ////    // Weighting can be adjusted here if necessary
        ////    return (enduranceStaminaScore + strengthPowerScore + agilitySpeedScore) / 3;
        ////}

        ////private static double CalculateScoreForRange(float value, float[] thresholds, float[] baseScores)
        ////{
        ////    for (int i = 0; i < thresholds.Length; i++)
        ////    {
        ////        if (value <= thresholds[i] || i == thresholds.Length - 1)
        ////        {
        ////            float L = i > 0 ? thresholds[i - 1] : 0;
        ////            float U = thresholds[i];
        ////            float B = baseScores[i];
        ////            float T = i < baseScores.Length - 1 ? baseScores[i + 1] : 10000;
        ////            return B + (value - L) / (U - L) * (T - B);
        ////        }
        ////    }
        ////    return 0; // Default case, should not be reached
        ////}

        ////private static double CalculateInverseScoreForRange(float value, float[] thresholds, float[] baseScores)
        ////{
        ////    // Similar to CalculateScoreForRange but designed for metrics where lower values are better
        ////    float inverseValue = thresholds[thresholds.Length - 1] - value;
        ////    return CalculateScoreForRange(inverseValue, thresholds.Select(t => thresholds[thresholds.Length - 1] - t).ToArray(), baseScores);
        ////}

        ////public static double CalculateStrengthScore(MeasurementDto measurement)
        ////{
        ////    var score = measurement.MuscleMass * (176 / measurement.Height);

        ////    return StandardizeAndScaleScore(score, 45, 65);
        ////}

        ////public static double CalculateEnduranceScore(MeasurementDto measurement)
        ////{
        ////    var score = (100 - measurement.BodyFat) * (measurement.Age / measurement.MetabolicAge);

        ////    return StandardizeAndScaleScore(score, 45, 110);
        ////}

        ////public static double CalculateAgilityScore(MeasurementDto measurement)
        ////{
        ////    var score = (50 - measurement.BodyFat) * (measurement.MuscleMass / measurement.Weight);

        ////    return StandardizeAndScaleScore(score, 3, 36);
        ////}

        ////public static double CalculateOverallScore(MeasurementDto measurement, double strengthScore, double enduranceScore, double agilityScore)
        ////{
        ////    var score = ((strengthScore * 2) + (enduranceScore * 0.5) + (agilityScore * 0.5)) / 3;

        ////    return StandardizeAndScaleScore(score, 1, 10000);
        ////}

        //private static double StandardizeAndScaleScore(double score, double minScore, double maxScore)
        //{
        //    var standardizedScore = (score - minScore) / (maxScore - minScore);
        //    return Math.Round(standardizedScore * 10000);
        //}

        //public static string GetGrade(int score)
        //{
        //    if (score >= 9000)
        //        return "S";
        //    else if (score >= 8000)
        //        return "A";
        //    else if (score >= 7000)
        //        return "B";
        //    else if (score >= 6000)
        //        return "C";
        //    else if (score >= 4000)
        //        return "D";
        //    else if (score >= 2000)
        //        return "E";
        //    else
        //        return "F";
        //}

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

