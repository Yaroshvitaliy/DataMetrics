using Microsoft.AspNetCore.Mvc;

using DataMetrics.Domain;
using DataMetrics.Api.Host.Models;
using DataMetrics.ApplicationServices;
using System.Linq;

namespace DataMetrics.Api.Host.Controllers
{
    [Route("api/metrics/gauge")]
    [ApiController]
    public class MetricsGaugeController : ControllerBase
    {
        private readonly CompositionRoot _compositionRoot;

        public MetricsGaugeController(CompositionRoot compositionRoot) => 
            _compositionRoot = compositionRoot;

        [HttpPost]
        public IActionResult SetValue(SetValueMetricsGaugeRequest request)
        {
            var tags = request.Tags.Select(x => new Metrics.MerticTag(x.Key, x.Value)).ToArray();
            var value = new Metrics.MetricValueGauge(request.Name, request.Value, tags);
            var command = Command.NewMetricsCommand(Metrics.Command.NewSendMetricGauge(value));            
            var result = _compositionRoot.ExecuteCommand(command);

            if (result.IsError) return BadRequest();
            return Ok();
        }
    }
}
