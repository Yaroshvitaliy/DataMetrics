namespace DataMetrics.Infra

open App.Metrics
open App.Metrics.Gauge

module Metrics =

    module Gauge =

        let setValue (metrics: IMetrics) (options: GaugeOptions) (tags: MetricTags) (value: float) =
            metrics.Measure.Gauge.SetValue(options, tags, value)