namespace ElmishPlanets

open Cmd
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews
open Xamarin.Forms
open Urho

module App =
    let mutable _app: HelloWorldUrhoApp option = None

    type Model = { ShowSurface: bool }

    type Msg = Appearing | Created of HelloWorldUrhoApp | ChangeColor

    let setApp app =
        _app <- Some app
        None

    let changeTextColor() =
        match _app with
        | None -> ()
        | Some app -> app.ChangeTextColor()
        None

    let init () = { ShowSurface = false }, Cmd.none

    let update (msg: Msg) (model: Model) =
        match msg with
        | Appearing -> { model with ShowSurface = true }, Cmd.none
        | Created app -> model, Cmd.ofMsgOption (setApp app)
        | ChangeColor -> model, Cmd.ofMsgOption (changeTextColor())

    let view (model: Model) dispatch =
        View.ContentPage(
            appearing=(fun () -> dispatch Appearing),
            content = View.StackLayout(
                padding = 20.0,
                children = [
                    match model.ShowSurface with
                    | false -> yield View.Label(text = "Loading...", verticalOptions = LayoutOptions.CenterAndExpand)
                    | true -> 
                        yield View.UrhoSurface<HelloWorldUrhoApp>(
                                    options = { AssetsFolder = None },
                                    created = (fun app -> dispatch (Created app)),
                                    verticalOptions = LayoutOptions.FillAndExpand)
                        yield View.Button(text = "Change color", command = (fun () -> dispatch ChangeColor))
                ]
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
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