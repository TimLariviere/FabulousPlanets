namespace ElmishPlanets

open Urho
open Urho.Actions

module HelloWorldUrhoApp =
    let create3DScene () =
        let scene = new Scene()
        scene.CreateComponent<Octree>() |> ignore
        scene

    let addPyramid (resourceCache: Urho.Resources.ResourceCache) (scene: Scene) =
        let node = scene.CreateChild()
        node.Position <- Vector3(0.f, 0.f, 3.5f)
        node.Rotation <- Quaternion(0.f, 0.f, -23.439281f)
        node.SetScale(1.25f)
        let modelObject = node.CreateComponent<StaticModel>()
        modelObject.Model <- CoreAssets.Models.Sphere
        modelObject.SetMaterial(resourceCache.GetMaterial("Materials/Earth.xml"))
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
        
open HelloWorldUrhoApp

type HelloWorldUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)

    member val Node : Node = null with get, set
    member val IsPaused = false with get, set

    override this.Start() =
        base.Start()

        let (_, pyramidNode) =
            create3DScene()
            |> addLight
            |> addCamera this.Renderer
            |> addPyramid this.ResourceCache

        rotateNodeForever pyramidNode

        this.Node <- pyramidNode

    member this.ToggleActions() =
        this.IsPaused <- not this.IsPaused
        match this.IsPaused with
        | false -> this.Node.ResumeAllActions()
        | true -> this.Node.PauseAllActions()