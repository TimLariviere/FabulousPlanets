namespace ElmishPlanets

open Urho
open Urho.Actions
open Urho.Gui

module HelloWorldUrhoApp =
    let create3DObject (resourceCache: Urho.Resources.ResourceCache) =
        let scene = new Scene()
        scene.CreateComponent<Octree>() |> ignore

        let node = scene.CreateChild()
        node.Position <- Vector3(0.f, 0.f, 3.5f)
        node.Rotation <- Quaternion(15.f, 0.f, 0.f)
        node.SetScale(1.f)

        // Add Pyramid Model
        let modelObject = node.CreateComponent<StaticModel>()
        modelObject.Model <- resourceCache.GetModel("Models/Pyramid.mdl")

        let light = scene.CreateChild(name = "light")
        light.SetDirection(Vector3(0.4f, -0.5f, 0.3f))
        light.CreateComponent<Light>()|> ignore

        let cameraNode = scene.CreateChild(name = "camera")
        let camera = cameraNode.CreateComponent<Camera>()

        (scene, camera, node)


type HelloWorldUrhoApp(options: ApplicationOptions) =
    inherit Urho.Application(options)

    member val Node : Node = null with get, set
    member val IsPaused = false with get, set

    override this.Start() =
        base.Start()

        let (scene, camera, node) = HelloWorldUrhoApp.create3DObject this.ResourceCache
        this.Renderer.SetViewport(0u, new Viewport(scene, camera, null))
        this.Node <- node

        let actions: FiniteTimeAction array = [| new RepeatForever(new RotateBy(1.f, 0.f, 90.f, 0.f)) |]
        node.RunActionsAsync(actions)
        |> Async.AwaitTask
        |> Async.Ignore
        |> Async.StartImmediate

    member this.ToggleActions() =
        this.IsPaused <- not this.IsPaused
        match this.IsPaused with
        | false -> this.Node.ResumeAllActions()
        | true -> this.Node.PauseAllActions()