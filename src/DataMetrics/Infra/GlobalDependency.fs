namespace DataMetrics.Infra

open System

open App.Metrics
open Serilog

open DataMetrics

type GlobalDependency = 
    { Id: string
      Logger: ILogger
      Metrics: IMetrics
      Config: DataMetricsConfig }
    static member Create (logger, metrics, config) = 
    { Id = sprintf "DataMetricsApi-%s" (Guid.NewGuid().ToString("N"))
      Logger = logger
      Metrics = metrics
      Config = config }