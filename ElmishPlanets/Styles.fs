namespace ElmishPlanets

open Elmish.XamarinForms.DynamicViews
open Xamarin.Forms

module Styles =
    type Elmish.XamarinForms.DynamicViews.ViewElement with
        member this.TitleFontSize() = this.FontSize(Device.GetNamedSize(NamedSize.Large, typeof<Label>) * 1.5)
        member this.WhiteText() = this.TextColor(Color.White)
