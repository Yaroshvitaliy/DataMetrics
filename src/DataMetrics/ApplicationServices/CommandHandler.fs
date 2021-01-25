namespace DataMetrics.ApplicationServices

open System.Text.RegularExpressions

open FsToolkit.ErrorHandling

open DataMetrics.Domain
open DataMetrics.DomainServices
open DataMetrics.Infra

type Command = 
    | MetricsCommand of Metrics.Command

type AppError =
    | MetricsError of Metrics.Error

module internal MetricsCommandHandler =

    let tagRegex = Regex("^[a-zA-Z0-9_]*$")

    let validateTag (tag: Metrics.MerticTag) =
        tagRegex.IsMatch(tag.key)

    let validateCommand command = 
        match command with 
        | Metrics.SendMetricGauge metricValue ->            
            if not(tagRegex.IsMatch(metricValue.name)) then
                Metrics.Error.InvalidName |> Error
            else 
                let invalidTags = metricValue.tags |> Array.filter(validateTag >> not)
                if invalidTags.Length > 0 then
                    Metrics.Error.InvalidTags invalidTags |> Error
                else Ok command

    let handleCommand (dep: GlobalDependency) command = result {
        match! validateCommand(command) with
        | Metrics.SendMetricGauge metricValue ->
            let gauge = Metrics.Gauge.create dep.Config.MetricsContext metricValue
            let tags = Metrics.Tags.create metricValue.tags
            Metrics.Gauge.setValue dep.Metrics gauge tags metricValue.value
            return Metrics.GaugeMetricSent  metricValue
    }

module CommandHandler =
    
    let handleCommand dep command =
        match command with
        | MetricsCommand cmd -> 
            MetricsCommandHandler.handleCommand dep cmd
            |> Result.mapError(fun error -> 
                dep.Logger.Error("An error has occurred while executing a command: {error}", error)
                MetricsError error
            )