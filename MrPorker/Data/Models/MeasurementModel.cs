﻿namespace MrPorker.Data.Models
{
    public class MeasurementModel
    {
        public int Id { get; set; }

        public long Timestamp { get; set; }

        public float Weight { get; set; }
        public float BodyMassIndex { get; set; }
        public float BodyFat { get; set; }
        public float FatFreeBodyWeight { get; set; }
        public float SubcutaneousFat { get; set; }
        public float VisceralFat { get; set; }
        public float BodyWater { get; set; }
        public float SkeletalMuscle { get; set; }
        public float MuscleMass { get; set; }
        public float BoneMass { get; set; }
        public float Protein { get; set; }
        public float BasalMetabolicRate { get; set; }

        public float MetabolicAge { get; set; }
        public string? Remarks { get; set; }
    }
}
