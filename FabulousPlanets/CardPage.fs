namespace FabulousPlanets

open Fabulous
open Models
open Styles
open Fabulous.XamarinForms
open Urho.Forms
open Xamarin.Forms

module CardPage =
    type Model =
        { HasAppeared: bool
          Planet: Planet }

    type Msg =
        | PageAppearing
        | UrhoAppCreated of PlanetVisualizerUrhoApp

    let urhoSurfaceRef = ViewRef<UrhoSurface>()
    
    let loadPlanetModel (app: PlanetVisualizerUrhoApp) (model: Model) =
        app.LoadPlanet(model.Planet)

    let init planet =
        { HasAppeared = false
          Planet = planet }

    let update msg model =
        match msg with
        | PageAppearing ->
            { model with HasAppeared = true }
        | UrhoAppCreated app ->
            loadPlanetModel app model
            model

    let mkInfoLabel title text =
        View.StackLayout(
            orientation = StackOrientation.Horizontal,
            children = [
                View.Label(
                    text = title
                ).WhiteText()
                View.Label(
                    text = text,
                    horizontalOptions = LayoutOptions.FillAndExpand,
                    horizontalTextAlignment = TextAlignment.End
                ).WhiteText()
            ]
        )

    let view (model: Model) dispatch =
        let onAppearing () = dispatch PageAppearing
        let onUrhoSurfaceCreated urhoSurface = dispatch (UrhoAppCreated urhoSurface)
        
        View.ContentPage(
            appearing = onAppearing,
            title = model.Planet.Info.Name,
            backgroundColor = Color.Black,
            content = View.Grid(
                children = [
                    View.UrhoSurface<PlanetVisualizerUrhoApp>(
                        ?options = (if model.HasAppeared then Some (View.UrhoApplicationOptions(assetsFolder = "Data")) else None),
                        created = onUrhoSurfaceCreated
                    )
                    View.StackLayout(
                        padding = Thickness(20.),
                        inputTransparent = true,
                        children = [
                            mkInfoLabel "Diameter" (kmToString model.Planet.Info.Diameter)
                            mkInfoLabel "Temperature" (celsiusToString model.Planet.Info.Temperature)
                            mkInfoLabel "Speed" (speedToString model.Planet.Info.Speed)
                            mkInfoLabel "Mass" (massToString model.Planet.Info.Mass)
                            mkInfoLabel "Year of Discovery" (intOptionToString "N/A" model.Planet.Info.YearOfDiscovery)
                            View.Label(
                                text = model.Planet.Info.Description,
                                horizontalTextAlignment = TextAlignment.Center,
                                verticalOptions = LayoutOptions.FillAndExpand,
                                verticalTextAlignment = TextAlignment.End
                            ).WhiteText()
                        ]
                    )
                ]
            )
        )