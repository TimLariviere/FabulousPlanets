namespace ElmishPlanets

open Urho
open Urho.Actions
open Models

module PlanetVisualizerUrhoApp =
    let create3DScene () =
        let scene = new Scene()
        scene.CreateComponent<Octree>() |> ignore
        scene

    let addPlanet (resourceCache: Urho.Resources.ResourceCache) name axialTilt (scene: Scene) =
        scene.RemoveChild(scene.GetChild("planet"))
        let node = scene.CreateChild(name="planet")
        node.Position <- Vector3(0.f, 0.f, 3.5f)
        node.Rotation <- Quaternion(0.f, 0.f, (float32 axialTilt) * -1.f)
        node.SetScale(1.25f)
        let modelObject = node.CreateComponent<StaticModel>()
        modelObject.Model <- CoreAssets.Models.Sphere
        modelObject.SetMaterial(resourceCache.GetMaterial("Materials/" + name + ".xml"))
        (scene, node)

    let addLight (scene: Scene) =
        let light = scene.CreateChild(name = "light")
        light.SetDirection(Vector3(0.4f, -0.5f, 0.3f))
        light.CreateComponent<Light>() |> ignore
        scene

    let addCamera (renderer: Renderer) (scene: Scene) =
        let cameraNode = scene.CreateChild(name = "camera")
        let camera = cameraNode.CreateComponent<Camera>()
        let viewport = new Viewport(scene, camera, null)
        renderer.SetViewport(0u, viewport)
        scene

    let rotateNodeForever (node: Node) =
        let actions: FiniteTimeAction array = [| new RepeatForever(new RotateBy(1.f, 0.f, -90.f, 0.f)) |]
        node.RunActionsAsync(actions)
        |> Async.AwaitTask
        |> Async.Ignore
        |> Async.StartImmediate
        
open PlanetVisualizerUrhoApp

type PlanetVisualizerUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)

    member val Scene: Scene = null with get, set

    override this.Start() =
        base.Start()

        let scene =
            create3DScene()
            |> addLight
            |> addCamera this.Renderer

        this.Scene <- scene

    member this.LoadPlanet (planet: Planet) =
        this.Scene
        |> addPlanet this.ResourceCache planet.Info.Name planet.Info.AxialTilt
        |> snd
        |> rotateNodeForever