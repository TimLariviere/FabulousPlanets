namespace ElmishPlanets

open Urho
open Urho.Actions
open Models
open System

module PlanetVisualizerUrhoApp =
    let create3DScene (resourceCache: Urho.Resources.ResourceCache) =
        let scene = new Scene()
        scene.LoadXmlFromCache(resourceCache, "Scenes/PlanetScene.xml") |> ignore
        scene
        
    let removeUnusedNodes hasRings (scene: Scene) =
        match hasRings with
        | true ->
            scene.GetChild("setup_rings").Enabled <- true
            scene.RemoveChild(scene.GetChild("setup_default"))
        | false ->
            scene.GetChild("setup_default").Enabled <- true
            scene.RemoveChild(scene.GetChild("setup_rings"))
            let planet = scene.GetChild("planet")
            planet.RemoveChild(planet.GetChild("rings"))
        scene
        
    let setViewport (renderer: Renderer) (scene: Scene) =
        let cameraNode = scene.GetChild("camera", recursive=true)
        let camera = cameraNode.GetComponent<Camera>()
        let viewport = new Viewport(scene, camera, null)
        renderer.SetViewport(0u, viewport)
        scene
                
    let findPlanet (scene: Scene) =
        let planet = scene.GetChild("planet")
        let body = planet.GetChild("body")
        let rings = planet.GetChild("rings")
        (planet, body, rings)
        
    let setMaterial (resourceCache: Urho.Resources.ResourceCache) materialPath (node: Node) =
        match node with 
        | null -> ()
        | _ -> node.GetComponent<StaticModel>().SetMaterial(resourceCache.GetMaterial(materialPath))
        node
        
    let setAxialTilt axialTilt (node: Node) =
        node.Rotation <- Quaternion(0.f, 0.f, (float32 axialTilt) * -1.f)
        node
        
    let enable (node: Node) =
        node.Enabled <- true
        node
        
    let rotateForever rotationSpeed (node: Node) =
        let actions: FiniteTimeAction array = [| new RepeatForever(new RotateBy(1.f, 0.f, rotationSpeed, 0.f)) |]
        node.RunActionsAsync(actions)
        |> Async.AwaitTask
        |> Async.Ignore
        |> Async.StartImmediate
        
    let computeRotationSpeed (planet: Planet) =
        let rotationSpeedRelativeToEarth = planet.Info.RotationPeriod / 24.<h>
        let rotationSpeedInDegrees = -22.5 / rotationSpeedRelativeToEarth

        // Round rotation speed for smoother animations
        if rotationSpeedInDegrees > 1. then
            float32 (Math.Round(rotationSpeedInDegrees))
        else
            float32 rotationSpeedInDegrees
            
    let computeRemainingTime remainingTime deltaTime =
        match remainingTime with 
        | Some x when x > 0.f -> Some (x - deltaTime)
        | _ -> None
        
    let resumeAnimation planet axialTilt rotationSpeed =
        planet |> setAxialTilt axialTilt |> rotateForever rotationSpeed
        
    let userPan (planet: Node) (delta: IntVector2) =
        planet.RemoveAllActions()
        if abs(delta.X) > abs(delta.Y) then
            planet.Rotate(Quaternion(0.f, (float32 -delta.X) / 5.f, 0.f), TransformSpace.World)
        else
            planet.Rotate(Quaternion((float32 -delta.Y) / 5.f, 0.f, 0.f), TransformSpace.World)
        
open PlanetVisualizerUrhoApp

type PlanetVisualizerUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)
    
    let defaultWaitTime = 5.f
    
    let mutable planetNode: Node option = None
    let mutable rotationSpeed = 0.f
    let mutable axialTilt = 0.f
    let mutable waitTimeBeforeResumeAnimation = None

    override this.Start() =
        base.Start()

    override this.OnUpdate timeStep =
        match planetNode with
        | None -> ()
        | Some planet ->
            match this.Input.NumTouches with
            | 0u ->
                // Check if we should resume the animation
                let remainingTime = waitTimeBeforeResumeAnimation
                match remainingTime, (computeRemainingTime remainingTime timeStep) with
                | Some _, Some newTime ->
                    waitTimeBeforeResumeAnimation <- Some newTime
                | Some _, None ->
                    waitTimeBeforeResumeAnimation <- None
                    resumeAnimation planet axialTilt rotationSpeed
                | _ -> ()
                
            | _ ->
                // The user uses 1 finger to pan
                waitTimeBeforeResumeAnimation <- Some defaultWaitTime
                userPan planet (this.Input.GetTouch(0u).Delta)

    member this.LoadPlanet (planet: Planet) =
        rotationSpeed <- computeRotationSpeed planet
        axialTilt <- float32 planet.Info.AxialTilt
            
        Urho.Application.InvokeOnMain(fun() ->        
            let scene = 
                create3DScene this.ResourceCache
                |> removeUnusedNodes planet.Info.Rings.IsSome
                |> setViewport this.Renderer

            let planet = 
                scene
                |> findPlanet
                |> (fun (p, b, r) ->
                        b |> setMaterial this.ResourceCache ("Materials/" + planet.Info.Name + ".xml") |> ignore
                        r |> setMaterial this.ResourceCache ("Materials/" + planet.Info.Name + "Rings.xml") |> ignore
                        p
                    )
                |> setAxialTilt axialTilt
                |> enable
                
            planet |> rotateForever rotationSpeed
                
            planetNode <- Some planet
        )