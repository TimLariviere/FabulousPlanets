namespace ElmishPlanets

open Models
open Styles
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

module App =
    open Xamarin.Forms.PlatformConfiguration
    open Xamarin.Forms.PlatformConfiguration.iOSSpecific
    open Xamarin.Forms

    type Model =
        { CardPageModel: CardPage.Model option }

    type Msg =
        | CardPageMsg of CardPage.Msg
        | SelectPlanet of int

    let init () = 
        { CardPageModel = None }, Cmd.none

    let update (msg: Msg) (model: Model) =
        match msg with
        | CardPageMsg msg ->
            let m, cmd, externalMsg = CardPage.update msg model.CardPageModel.Value

            let cmd2 =
                match externalMsg with
                | CardPage.ExternalMsg.NoOp -> Cmd.none

            { model with CardPageModel = Some m }, Cmd.batch [ Cmd.map CardPageMsg cmd; cmd2 ]

        | SelectPlanet i ->
            let cardPageModel = CardPage.init solarObjects.[i]
            { model with CardPageModel = Some cardPageModel }, Cmd.none

    let view (model: Model) dispatch =
        let mainPage =
            View.ContentPage(
                backgroundColor=Color.Black,
                content=View.ListView(
                    backgroundColor=Color.Black,
                    separatorColor=Color.White,
                    itemTapped=(SelectPlanet >> dispatch),
                    items=[
                        for i in 0 .. 1 .. (solarObjects.Length - 1) do
                            yield View.StackLayout(
                                padding=Thickness(20., 10.),
                                children=[
                                    View.Label(text=solarObjects.[i].Info.Name, verticalOptions=LayoutOptions.Center).WhiteText()
                                ]
                            )
                    ]
                )
            )

        let planetPage =
            match model.CardPageModel with
            | None -> None
            | Some model -> Some (CardPage.view model (CardPageMsg >> dispatch))

        View.NavigationPage(
            appearing=(fun() -> (Xamarin.Forms.Application.Current.MainPage :?> Xamarin.Forms.NavigationPage).On<iOS>().SetPrefersLargeTitles(true) |> ignore),
            barBackgroundColor=Color.Black,
            barTextColor=Color.White,
            backgroundColor=Color.Black,
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