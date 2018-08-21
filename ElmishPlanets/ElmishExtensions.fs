namespace ElmishPlanets

open Elmish.XamarinForms

module Cmd =
    let ofMsgOption (p: 'msg option) : Cmd<'msg> =
        [ fun dispatch -> let msg = p in match msg with None -> () | Some msg -> dispatch msg ]

    let ofAsyncMsgOption (p: Async<'msg option>) : Cmd<'msg> =
        [ fun dispatch -> async { let! msg = p in match msg with None -> () | Some msg -> dispatch msg } |> Async.StartImmediate ]