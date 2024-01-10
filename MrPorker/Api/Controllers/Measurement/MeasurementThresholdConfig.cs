using System.Text.RegularExpressions;

namespace MrPorker.Api.Controllers.Measurement
{
    public class MeasurementThresholdConfig
    {
        public List<MeasurementThreshold> Weight { get; set; }
        public List<MeasurementThreshold> BodyMassIndex { get; set; }
        public List<MeasurementThreshold> BodyFat { get; set; }
        public List<MeasurementThreshold> FatFreeBodyWeight { get; set; }
        public List<MeasurementThreshold> SubcutaneousFat { get; set; }
        public List<MeasurementThreshold> VisceralFat { get; set; }
        public List<MeasurementThreshold> BodyWater { get; set; }
        public List<MeasurementThreshold> SkeletalMuscle { get; set; }
        public List<MeasurementThreshold> MuscleMass { get; set; }
        public List<MeasurementThreshold> BoneMass { get; set; }
        public List<MeasurementThreshold> Protein { get; set; }
        public List<MeasurementThreshold> BasalMetabolicRate { get; set; }
        public List<MeasurementThreshold> MetabolicAge => new()
        {
            new()
            {
                Category = "Younger",
                Value = GetAge()
            },
            new()
            {
                Category = "Older",
                Value = 99999
            }
        };

        private static float GetAge()
        {
            var birthDate = new DateTime(1998, 10, 27);
            var currentDate = DateTime.Today;

            var age = currentDate.Year - birthDate.Year;

            // Check if the birth date has not occurred yet this year
            if (birthDate > currentDate.AddYears(-age))
                age--;

            return age;
        }

        public string GetCategoryForMeasurement(string measurementName, float value)
        {
            if (GetType().GetProperty(measurementName)?.GetValue(this) is not List<MeasurementThreshold> thresholds)
                return "Error";

            foreach (var threshold in thresholds)
            {
                if (value <= threshold.Value)
                    return threshold.Category ?? "Error";
            }

            return "Error";
        }

        public static string GetFormattedNameForMeasurement(string measurementName)
        {
            if (string.IsNullOrWhiteSpace(measurementName))
                return string.Empty;

            if (measurementName == "BodyMassIndex") return "BMI";
            if (measurementName == "FatFreeBodyWeight") return "Fat-free Weight";
            if (measurementName == "BasalMetabolicRate") return "BMR";

            return Regex.Replace(measurementName, "([a-z])([A-Z])", "$1 $2");
        }

        public static string GetUnitForMeasurement(string measurementName)
        {
            if (string.IsNullOrWhiteSpace(measurementName))
                return string.Empty;

            if (measurementName == "Weight") return "kg";
            if (measurementName == "BodyMassIndex") return string.Empty;
            if (measurementName == "BodyFat") return "%";
            if (measurementName == "FatFreeBodyWeight") return "kg";
            if (measurementName == "SubcutaneousFat") return "%";
            if (measurementName == "VisceralFat") return string.Empty;
            if (measurementName == "BodyWater") return "%";
            if (measurementName == "SkeletalMuscle") return "%";
            if (measurementName == "MuscleMass") return "kg";
            if (measurementName == "BoneMass") return "kg";
            if (measurementName == "Protein") return "%";
            if (measurementName == "BasalMetabolicRate") return string.Empty;
            if (measurementName == "MetabolicAge") return string.Empty;

            return Regex.Replace(measurementName, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
