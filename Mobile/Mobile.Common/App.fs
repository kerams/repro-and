module Main

open System
open Fabulous
open Fabulous.Avalonia
open Projektanker.Icons.Avalonia
open type Fabulous.Avalonia.View
open System.Diagnostics.CodeAnalysis

[<NoComparison; NoEquality>]
type AudioState =
    | Downloading of key: string
    | Playing of key: string
    | FailedToLoad of key: string
    | NotPlaying

type Msg =
    | AudioLoaded of key: string * byte[]

type Model = {
    Audio: AudioState
    AudioPlayer: Plugin.Maui.Audio.AudioPlayer }

let init (_onResume: IEvent<_, _>) =
    let model = {
        Audio = Downloading ""
        AudioPlayer = new Plugin.Maui.Audio.AudioPlayer (Plugin.Maui.Audio.BaseOptions ())
    }
    model, []

let update msg model =
    match msg with
    | AudioLoaded (key, data) ->
        try
            model.AudioPlayer.Play data
            { model with Audio = Playing key }, []
        with e ->
            Material.Styles.Controls.SnackbarHost.Post (e.Message, null, Avalonia.Threading.DispatcherPriority.Normal)
            { model with Audio = FailedToLoad key }, []

let view _model =
    Fabulous.Avalonia.View.ButtonWithIconTapHandled((fun () -> AudioLoaded ("", [| 0uy |])), "mdi-play").center()
    |> SnackbarHost
    |> SingleViewApplication

[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _ * _ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _ * _ * _ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof<_ * _ * _ * _ * _ * _ * _ * _>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof<Map<_, _>>)>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "Fable.Remoting.DotnetClient.Remoting.ServiceCallerFuncTask2`2", "Fable.Remoting.DotnetClient")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "Fable.Remoting.DotnetClient.Remoting.ServiceCallerFuncTask3`3", "Fable.Remoting.DotnetClient")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeArray`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpRecord`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpUnion`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpUnionCase`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeTuple`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpList`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpOption`1", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeFSharpMap`2", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ShapeMember`2", "Fable.Remoting.MsgPack")>]
[<DynamicDependency(DynamicallyAccessedMemberTypes.All, "TypeShape.ReadOnlyMember`2", "Fable.Remoting.MsgPack")>]
[<DynamicDependency("OfSeq`1", "Microsoft.FSharp.Collections.ListModule", "FSharp.Core")>]
let configure onResume =
    let app =
        Program.statefulWithCmd init (fun msg model -> update msg model)
        |> Program.withView view

    IconProvider.Current.Register<MaterialDesign.MaterialDesignIconProvider> () |> ignore
    FabulousAppBuilder.Configure ((fun () -> Avalonia.Markup.Xaml.Styling.StyleInclude (baseUri = null, Source = Uri "avares://Mobile.Common/Styles/App.xaml")), app, onResume)
