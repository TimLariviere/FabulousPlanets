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
            scene.RemoveChild(scene.GetChild("setup_default"))
        | false ->
            scene.RemoveChild(scene.GetChild("setup_rings"))
            scene.RemoveChild(scene.GetChild("planet").GetChild("rings"))
        scene
        
    let setViewport (renderer: Renderer) hasRings (scene: Scene) =
        let setupNode = if hasRings then scene.GetChild("setup_rings") else scene.GetChild("setup_default")
        setupNode.Enabled <- true
        let cameraNode = setupNode.GetChild("camera")
        let camera = cameraNode.GetComponent<Camera>()
        let viewport = new Viewport(scene, camera, null)
        renderer.SetViewport(0u, viewport)
        scene
                
    let findPlanet (scene: Scene) =
        let planet = scene.GetChild("planet")
        let body = planet.GetChild("body")
        let rings = planet.GetChild("rings")
        (planet, body, rings)
        
    let setMaterial (resourceCache: Urho.Resources.ResourceCache) materialPath (planet: Node, body: Node, rings: Node) =
        let staticModel = body.GetComponent<StaticModel>()
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
    
    override this.Start() =
        base.Start()            

    member this.LoadPlanet (planet: Planet) =
        Urho.Application.InvokeOnMain(fun() ->
            let rotationSpeedRelativeToEarth = planet.Info.RotationPeriod / 24.<h>
            let rotationSpeedInDegrees = -22.5 / rotationSpeedRelativeToEarth

            // Round rotation speed for smoother animations
            let rotationSpeed = if rotationSpeedInDegrees > 1. then Math.Round(rotationSpeedInDegrees) else rotationSpeedInDegrees
        
            create3DScene this.ResourceCache
            |> removeUnusedNodes planet.Info.Rings.IsSome
            |> setViewport this.Renderer planet.Info.Rings.IsSome
            |> findPlanet
            |> setMaterial this.ResourceCache ("Materials/" + planet.Info.Name + ".xml")
            |> setAxialTilt (float32 planet.Info.AxialTilt)
            |> enableNode planet.Info.Rings.IsSome
            |> rotatePlanetForever (float32 rotationSpeed)
        )