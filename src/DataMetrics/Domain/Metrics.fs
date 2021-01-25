namespace DataMetrics.Domain

module Metrics =

    type MerticTag = 
        {
            key: string
            value: string
        }

    [<CLIMutable>]
    type MetricValueGauge = 
        {
            name: string
            value: float
            tags: MerticTag[]
        }

    type Command = 
        | SendMetricGauge of MetricValueGauge

    type Event = 
        | GaugeMetricSent of MetricValueGauge

    type Error =
        | InvalidName
        | InvalidTags of MerticTag[]