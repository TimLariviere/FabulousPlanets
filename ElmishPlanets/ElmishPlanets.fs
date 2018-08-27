namespace ElmishPlanets

open Cmd
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
                        View.Label(text="Mercury")
                        View.Label(text="Venus")
                        View.Label(text="Earth")
                        View.Label(text="Mars")
                        View.Label(text="Jupiter")
                        View.Label(text="Saturn")
                        View.Label(text="Uranus")
                        View.Label(text="Neptune")
                    ]
                )
            )

        let planetPage =
            View.ContentPage(
                appearing=(fun () -> dispatch Appearing),
                disappearing=(fun () -> dispatch Disappearing),
                content =
                    match model.ShowSurface with
                    | false ->
                        View.Label(text = "Loading...", verticalOptions = LayoutOptions.Center, horizontalOptions = LayoutOptions.Center)
                    | true ->
                        View.Grid(
                            children=[
                                View.UrhoSurface<HelloWorldUrhoApp>(
                                    options=View.UrhoApplicationOptions(assetsFolder = "Data"),
                                    created=(fun app -> dispatch (Created app))
                                )
                            
                                View.StackLayout(
                                    padding=20.0,
                                    children=[
                                        View.Label(text = "Earth", fontSize = Device.GetNamedSize(NamedSize.Large, typeof<Label>) * 1.5, textColor = Color.White)
                                        View.Button(text="Toggle animations", command=(fun () -> dispatch ToggleAnimations), verticalOptions=LayoutOptions.EndAndExpand)
                                    ]
                                )
                            ]
                        )
            )

        View.NavigationPage(
            pages=[
                yield mainPage
                if model.Planet.IsSome then yield planetPage else ()
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