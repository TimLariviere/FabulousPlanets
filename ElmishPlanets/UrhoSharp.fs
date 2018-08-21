namespace Elmish.XamarinForms.DynamicViews

open Urho
open Urho.Forms

[<AutoOpen>]
module UrhoSharpExtensions =
    type UrhoApplicationOptions =
        {
            AssetsFolder: string option
        }

    let OptionsAttribKey = AttributeKey<_> "UrhoSurface_Options"
    let CreatedAttribKey<'T when 'T :> Urho.Application> = AttributeKey<('T -> unit)> "UrhoSurface_Created"

    type Elmish.XamarinForms.DynamicViews.View with
        static member UrhoSurface<'T when 'T :> Urho.Application>(?options: UrhoApplicationOptions, ?created: ('T -> unit),
                                                                  // inherited attributes common to all views
                                                                  ?horizontalOptions, ?verticalOptions, ?margin, ?gestureRecognizers, ?anchorX, ?anchorY, ?backgroundColor,
                                                                  ?heightRequest, ?inputTransparent, ?isEnabled, ?isVisible, ?minimumHeightRequest, ?minimumWidthRequest,
                                                                  ?opacity, ?rotation, ?rotationX, ?rotationY, ?scale, ?style, ?translationX, ?translationY, ?widthRequest,
                                                                  ?resources, ?styles, ?styleSheets, ?classId, ?styleId, ?automationId) =

            let attribCount = match options with Some _ -> 1 | None -> 0
            let attribCount = match created with Some _ -> attribCount + 1 | None -> attribCount

            let attribs = 
                View.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, 
                               ?margin=margin, ?gestureRecognizers=gestureRecognizers, ?anchorX=anchorX, ?anchorY=anchorY, 
                               ?backgroundColor=backgroundColor, ?heightRequest=heightRequest, ?inputTransparent=inputTransparent, 
                               ?isEnabled=isEnabled, ?isVisible=isVisible, ?minimumHeightRequest=minimumHeightRequest,
                               ?minimumWidthRequest=minimumWidthRequest, ?opacity=opacity, ?rotation=rotation, 
                               ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?style=style, 
                               ?translationX=translationX, ?translationY=translationY, ?widthRequest=widthRequest, 
                               ?resources=resources, ?styles=styles, ?styleSheets=styleSheets, ?classId=classId, ?styleId=styleId, ?automationId=automationId)

            match options with None -> () | Some v -> attribs.Add(OptionsAttribKey, v)
            match created with None -> () | Some v -> attribs.Add(CreatedAttribKey, v)

            let createApplicationOptions (value: UrhoApplicationOptions) =
                let assetsFolder =
                    match value.AssetsFolder with
                    | None -> null
                    | Some folder -> folder

                new ApplicationOptions(assetsFolder)

            let updateApplicationOptions (target: UrhoSurface) v =
                let updateAsync = (async {
                    let applicationOptions = createApplicationOptions v
                    let! application = target.Show<'T>(applicationOptions) |> Async.AwaitTask
                    match created with
                    | None -> ()
                    | Some func -> func application
                })
                updateAsync |> Async.StartImmediate

            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: UrhoSurface) =
                View.UpdateView (prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, OptionsAttribKey, updateApplicationOptions)

            ViewElement.Create(UrhoSurface, update, attribs)