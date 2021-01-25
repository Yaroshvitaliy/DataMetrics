namespace DataMetrics.DomainServices

open App.Metrics
open App.Metrics.Gauge

open DataMetrics.Domain

module Metrics =
    
    module Tags =

        let create (tags: Metrics.MerticTag[]) =
            let keys = tags |> Array.map(fun tag -> tag.key)
            let values = tags |> Array.map(fun tag -> tag.value)
            MetricTags(keys, values)

    module Gauge =

        let create context (metricValue: Metrics.MetricValueGauge) =
            GaugeOptions(Context = context, Name = metricValue.name)