namespace ElmishPlanets

open Models
open Styles
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

module CardPage =
    open Xamarin.Forms

    type Model =
        {
            HasAppeared: bool
            Planet: Planet
        }

    type Msg =
        | PageAppearing
        | UrhoAppCreated of PlanetVisualizerUrhoApp

    type ExternalMsg = NoOp

    let loadPlanetModel (app: PlanetVisualizerUrhoApp) (model: Model) =
        app.LoadPlanet(model.Planet)
        None

    let init planet =
        {
            HasAppeared = false
            Planet = planet
        }

    let update msg model =
        match msg with
        | PageAppearing -> { model with HasAppeared = true }, Cmd.none, ExternalMsg.NoOp
        | UrhoAppCreated app -> model, Cmd.ofMsgOption (loadPlanetModel app model), ExternalMsg.NoOp

    let mkInfoLabel title text =
        View.StackLayout(
            orientation=StackOrientation.Horizontal,
            children=[
                View.Label(text=title).WhiteText()
                View.Label(text=text, horizontalOptions=LayoutOptions.FillAndExpand, horizontalTextAlignment=TextAlignment.End).WhiteText()
            ]
        )

    let view (model: Model) dispatch =
        View.ContentPage(
            appearing=(fun () -> dispatch PageAppearing),
            title=model.Planet.Info.Name,
            backgroundColor=Color.Black,
            content=View.Grid(
                children=[
                    View.UrhoSurface<PlanetVisualizerUrhoApp>(
                        ?options=(match model.HasAppeared with false -> None | true -> Some (View.UrhoApplicationOptions(assetsFolder="Data"))),
                        created=(UrhoAppCreated >> dispatch)
                    )
                    View.StackLayout(
                        padding=20.,
                        inputTransparent=true,
                        children=[
                            mkInfoLabel "Diameter" (kmToString model.Planet.Info.Diameter)
                            mkInfoLabel "Temperature" (celsiusToString model.Planet.Info.Temperature)
                            mkInfoLabel "Speed" (speedToString model.Planet.Info.Speed)
                            mkInfoLabel "Mass" (massToString model.Planet.Info.Mass)
                            mkInfoLabel "Year of Discovery" (intOptionToString "N/A" model.Planet.Info.YearOfDiscovery)
                            View.Label(text=model.Planet.Info.Description, horizontalTextAlignment=TextAlignment.Center, verticalOptions=LayoutOptions.FillAndExpand, verticalTextAlignment=TextAlignment.End).WhiteText()
                        ]
                    )
                ]
            )
        )