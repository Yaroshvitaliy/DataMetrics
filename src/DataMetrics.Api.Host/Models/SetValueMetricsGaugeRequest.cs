namespace DataMetrics.Api.Host.Models
{
    public class SetValueMetricsGaugeRequest
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public Tag[] Tags { get; set; }
    }
}
