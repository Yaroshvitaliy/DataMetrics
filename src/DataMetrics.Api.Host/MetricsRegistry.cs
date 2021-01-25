using App.Metrics;
using App.Metrics.Counter;

namespace DataMetrics.Api.Host
{
    public class MetricsRegistry
    {
        public CounterOptions VersionCounter => new CounterOptions
        {
            Name = "/api/version counter",
            MeasurementUnit = Unit.Calls
        };
    }
}
