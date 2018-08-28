namespace ElmishPlanets

open Elmish.XamarinForms.DynamicViews
open Xamarin.Forms

module Styles =
    type Elmish.XamarinForms.DynamicViews.ViewElement with
        member this.WhiteText() = this.TextColor(Color.White)
