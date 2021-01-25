using App.Metrics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DataMetrics.Api.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IMetrics _metrics;
        private readonly MetricsRegistry _metricsRegistry;

        public VersionController(IMetrics metrics, MetricsRegistry metricsRegistry)
        {
            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
            _metricsRegistry = metricsRegistry ?? throw new ArgumentNullException(nameof(metricsRegistry));
        }

        /// <summary>
        /// Gets API version.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            _metrics.Measure.Counter.Increment(_metricsRegistry.VersionCounter);
            return Ok(GetType().Assembly.GetName().Version.ToString());
        }
    }
}
