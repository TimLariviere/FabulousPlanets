namespace FabulousPlanets

open Models
open Styles
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms.PlatformConfiguration
open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Xamarin.Forms

module App =
    type Model =
        { CardPageModel: CardPage.Model option }

    type Msg =
        | CardPageMsg of CardPage.Msg
        | SelectPlanet of Planet

    let init () = 
        { CardPageModel = None }

    let update (msg: Msg) (model: Model) =
        match msg with
        | CardPageMsg msg ->
            let newModel = CardPage.update msg model.CardPageModel.Value
            { model with CardPageModel = Some newModel }

        | SelectPlanet planet ->
            let cardPageModel = CardPage.init planet
            { model with CardPageModel = Some cardPageModel }

    let view (model: Model) dispatch =
        let onNavigationPageAppearing () =
            NavigationPage.SetPrefersLargeTitles(Xamarin.Forms.Application.Current.MainPage, true)
            
        let onMainPageCreated (page: BindableObject) =
            Page.SetUseSafeArea(page, false)
            
        let onSelectionChanged (_, currentItems: ViewElement list option) =
            match currentItems |> Option.bind (List.tryHead) with
            | None -> ()
            | Some item ->
                let selectedPlanet = item.TryGetTag<Planet>().Value
                dispatch (SelectPlanet selectedPlanet)
        
        let mainPage =
            View.ContentPage(
                created = onMainPageCreated,
                title = "Choose a planet",
                backgroundColor = Color.Black,
                content = View.CollectionView(
                    margin = Thickness(20., 0.),
                    selectionMode = SelectionMode.Single,
                    selectionChanged = onSelectionChanged,
                    itemsLayout = GridItemsLayout(2, ItemsLayoutOrientation.Vertical, VerticalItemSpacing = 15.),
                    items = [
                        for solarObject in solarObjects do
                            yield View.StackLayout(
                                tag = solarObject,
                                children = [
                                    View.Image(
                                        source = Image.fromPath (solarObject.Info.Name.ToLower() + ".jpg")
                                    )
                                    View.Label(
                                        text = solarObject.Info.Name,
                                        horizontalTextAlignment = TextAlignment.Center,
                                        verticalOptions = LayoutOptions.End
                                    ).WhiteText()
                                ]
                            )
                    ]
                )
            )

        let planetPage =
            model.CardPageModel
            |> Option.map (fun m -> CardPage.view m (CardPageMsg >> dispatch))

        View.NavigationPage(
            appearing = onNavigationPageAppearing,
            barBackgroundColor = Color.Black,
            barTextColor = Color.White,
            backgroundColor = Color.Black,
            pages = [
                yield mainPage
                match planetPage with None -> () | Some p -> yield p
            ]
        )

    let program = Program.mkSimple init update view

type App () as app = 
    inherit Xamarin.Forms.Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    