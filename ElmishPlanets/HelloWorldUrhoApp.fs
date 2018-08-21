namespace ElmishPlanets

open Urho
open Urho.Gui

type HelloWorldUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)

    let createText(resourceCache: Urho.Resources.ResourceCache, textColor: Color) =
        let text = new Text()
        text.Value <- "Hello World!"
        text.HorizontalAlignment <- HorizontalAlignment.Center
        text.VerticalAlignment <- VerticalAlignment.Center
        text.SetColor(textColor)
        text.SetFont(font=resourceCache.GetFont("Fonts/Anonymous Pro.ttf"), size=30.f) |> ignore
        text

    member val Label: Text = null with get, set

    override this.Start() =
        base.Start()
        this.Label <- createText(this.ResourceCache, Color.Cyan)
        this.UI.Root.AddChild(this.Label)

    member this.ChangeTextColor() =
        this.Label.SetColor(Color.Red)