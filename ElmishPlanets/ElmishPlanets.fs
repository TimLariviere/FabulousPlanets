namespace ElmishPlanets

open Cmd
open Models
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews
open Xamarin.Forms

module App =
    let mutable _app: HelloWorldUrhoApp option = None

    type Model = { ShowSurface: bool; Planet: int option }

    type Msg = Appearing | Disappearing | Created of HelloWorldUrhoApp | ToggleAnimations | ShowPlanet of int

    let setApp app =
        match _app, app with
        | Some p, None -> p.Exit() |> Async.AwaitTask |> Async.StartImmediate
        | _, _ -> ()

        _app <- app
        None

    let toggleAnimations() =
        match _app with
        | None -> ()
        | Some app -> app.ToggleActions()
        None

    let init () = { ShowSurface = false; Planet = None }, Cmd.none

    let update (msg: Msg) (model: Model) =
        match msg with
        | Appearing -> { model with ShowSurface = true }, Cmd.none
        | Disappearing -> { model with ShowSurface = false; Planet = None }, Cmd.ofMsgOption (setApp None)
        | Created app -> model, Cmd.ofMsgOption (setApp (Some app))
        | ToggleAnimations -> model, Cmd.ofMsgOption (toggleAnimations())
        | ShowPlanet i -> { model with Planet = Some i }, Cmd.none

    let view (model: Model) dispatch =
        let mainPage =
            View.ContentPage(
                content=View.ListView(
                    itemTapped=(fun i -> dispatch (ShowPlanet i)),
                    items=[
                        for i in 0 .. 1 .. (solarObjects.Length - 1) do
                            yield View.Label(text=solarObjects.[i].Info.Name)
                    ]
                )
            )

        let planetPage =
            match model.Planet with
            | None -> None
            | Some i -> Some (CardPage.view { Planet = solarObjects.[i] } dispatch)

        View.NavigationPage(
            pages=[
                yield mainPage
                match planetPage with None -> () | Some p -> yield p
            ]
        )

    let program = Program.mkProgram init update view

type App () as app = 
    inherit Xamarin.Forms.Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    