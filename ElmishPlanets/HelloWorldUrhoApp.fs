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

    let getTextColor counter =
        match counter with
        | 0 -> Color.Cyan
        | 1 -> Color.Red
        | 2 -> Color.Yellow
        | _ -> failwith "No fourth color"

    member val Label: Text = null with get, set
    member val Counter: int = 0 with get, set

    override this.Start() =
        base.Start()
        this.Label <- createText(this.ResourceCache, getTextColor this.Counter)
        this.UI.Root.AddChild(this.Label)

    member this.ChangeTextColor() =
        this.Counter <- (this.Counter + 1) % 3
        this.Label.SetColor(getTextColor this.Counter)