namespace FabPlanets

open Fabulous.DynamicViews
open Xamarin.Forms

module Styles =
    type Fabulous.DynamicViews.ViewElement with
        member this.WhiteText() = this.TextColor(Color.White)
