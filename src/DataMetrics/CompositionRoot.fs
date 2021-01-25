namespace DataMetrics

open DataMetrics.ApplicationServices
open DataMetrics.Infra

type CompositionRoot (dep: GlobalDependency) =
    let _logger = dep.Logger.ForContext<CompositionRoot>()
    let _handleCommand = CommandHandler.handleCommand dep

    let init () =
        _logger.Information(sprintf "Initializing with configuration: %A" dep.Config)
        ()

    member x.ExecuteCommand(command) = _handleCommand command

    member x.Init() = init()