namespace FabulousPlanets

open Fabulous.XamarinForms
open Xamarin.Forms

module Styles =
    type Fabulous.ViewElement with
        member this.WhiteText() = this.TextColor(Color.White)
