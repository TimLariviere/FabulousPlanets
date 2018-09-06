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
            scene.RemoveChild(scene.GetChild("planet").GetChild("rings"))
        scene
        
    let setViewport (renderer: Renderer) (scene: Scene) =
        let cameraNode = scene.GetChild("camera", recursive=true)
        let camera = cameraNode.GetComponent<Camera>()
        let viewport = new Viewport(scene, camera, null)
        renderer.SetViewport(0u, viewport)
        cameraNode
                
    let findPlanet (scene: Scene) =
        let planet = scene.GetChild("planet")
        let body = planet.GetChild("body")
        let rings = planet.GetChild("rings")
        (planet, body, rings)
        
    let setBodyMaterial (resourceCache: Urho.Resources.ResourceCache) materialPath (planet: Node, body: Node, rings: Node) =
        let staticModel = body.GetComponent<StaticModel>()
        staticModel.SetMaterial(resourceCache.GetMaterial(materialPath))
        (planet, body, rings)
        
    let setRingsMaterialIf hasRings (resourceCache: Urho.Resources.ResourceCache) materialPath (planet: Node, body: Node, rings: Node) =
        match hasRings with
        | false -> ()
        | true ->
            let staticModel = rings.GetComponent<StaticModel>()
            staticModel.SetMaterial(resourceCache.GetMaterial(materialPath))
        (planet, body, rings)
        
    let setAxialTilt axialTilt (planet: Node, body: Node, rings: Node) =
        planet.Rotation <- Quaternion(0.f, 0.f, (float32 axialTilt) * -1.f)
        (planet, body, rings)
        
    let enableNode hasRings (planet: Node, body: Node, rings: Node) =
        planet.Enabled <- true
        rings.Enabled <- hasRings
        (planet, body, rings)
        
    let rotatePlanetForever rotationSpeed (planet: Node, _, _) =
        let actions: FiniteTimeAction array = [| new RepeatForever(new RotateBy(1.f, 0.f, rotationSpeed, 0.f)) |]
        planet.RunActionsAsync(actions)
        |> Async.AwaitTask
        |> Async.Ignore
        |> Async.StartImmediate
        
open PlanetVisualizerUrhoApp

type PlanetVisualizerUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)
    
    member val PlanetNode: Node = null with get, set

    override this.Start() =
        base.Start()

    override this.OnUpdate timeStep =
        match this.Input.NumTouches with
        | 0u -> ()
        | 2u ->
            let touch = this.Input.GetTouch(0u)
            this.PlanetNode.RemoveAllActions()
            this.PlanetNode.Rotate(Quaternion(0.0f, 0.0f, (float32 -touch.Delta.X)), TransformSpace.World)
        | _ ->
            let touch = this.Input.GetTouch(0u)
            this.PlanetNode.RemoveAllActions()
            this.PlanetNode.Rotate(Quaternion((float32 -touch.Delta.Y), (float32 -touch.Delta.X), 0.f), TransformSpace.World)

    member this.LoadPlanet (planet: Planet) =
        Urho.Application.InvokeOnMain(fun() ->
            let rotationSpeedRelativeToEarth = planet.Info.RotationPeriod / 24.<h>
            let rotationSpeedInDegrees = -22.5 / rotationSpeedRelativeToEarth

            // Round rotation speed for smoother animations
            let rotationSpeed = if rotationSpeedInDegrees > 1. then Math.Round(rotationSpeedInDegrees) else rotationSpeedInDegrees
        
            let scene = 
                create3DScene this.ResourceCache
                |> removeUnusedNodes planet.Info.Rings.IsSome

            let cameraNode =
                scene
                |> setViewport this.Renderer

            scene
            |> findPlanet
            |> setBodyMaterial this.ResourceCache ("Materials/" + planet.Info.Name + ".xml")
            |> setRingsMaterialIf planet.Info.Rings.IsSome this.ResourceCache ("Materials/" + planet.Info.Name + "Rings.xml")
            |> setAxialTilt (float32 planet.Info.AxialTilt)
            |> enableNode planet.Info.Rings.IsSome
            |> (fun (p, b, r) -> this.PlanetNode <- p; (p, b, r))
            |> rotatePlanetForever (float32 rotationSpeed)
        )