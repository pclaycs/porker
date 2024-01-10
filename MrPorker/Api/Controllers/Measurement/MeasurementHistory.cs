using MrPorker.Data.Dtos;

namespace MrPorker.Api.Controllers.Measurement
{
    public class MeasurementHistory
    {
        public MeasurementDto? Today { get; set; }
        public MeasurementDto? Yesterday { get; set; }
        public MeasurementDto? LastWeek { get; set; }
        public MeasurementDto? LastMonth { get; set; }
        public MeasurementDto? AllTime { get; set; }
    }
}
