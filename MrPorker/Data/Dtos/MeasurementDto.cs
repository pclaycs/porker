namespace MrPorker.Data.Dtos
{
    public class MeasurementDto
    {
        public long Timestamp { get; set; }

        public float Weight { get; set; } // in kg
        public float BodyMassIndex { get; set; } // int for BMI
        public float BodyFat { get; set; } // in %
        public float FatFreeBodyWeight { get; set; } // in kg
        public float SubcutaneousFat { get; set; } // in %
        public float VisceralFat { get; set; } // an int on a scale from 1 to 20, 1-6 being excellent, 15-20 being excessive
        public float BodyWater { get; set; } // in %
        public float SkeletalMuscle { get; set; } // in %
        public float MuscleMass { get; set; } // in kg
        public float BoneMass { get; set; } // in kg
        public float Protein { get; set; } // in %
        public float BasalMetabolicRate { get; set; } // int for BMR
        public float MetabolicAge { get; set; } // int for metabolic age

        public string? Remarks { get; set; }

        // HOG HOGAN PATCH
        public float Height { get; set; } // in cm
        public float Age { get; set; } // int for age

        public double Strength { get; set; }
        public double Endurance { get; set; }
        public double Agility { get; set; }
        public double Overall { get; set; }
    }
}
