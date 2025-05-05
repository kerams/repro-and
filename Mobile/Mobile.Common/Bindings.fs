[<AutoOpen>]
module Bindings

open Fabulous.Avalonia
open Material.Styles.Controls
open Projektanker.Icons.Avalonia
open Fabulous
open Fabulous.StackAllocatedCollections.StackList
open System.Runtime.CompilerServices
open Fabulous.StackAllocatedCollections

type CollectionBuilderExt =
    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemMarker when 'msg: equality> (_: CollectionBuilder<'msg, 'marker, 'itemMarker>, x: WidgetBuilder<'msg, 'itemMarker> option) : Content<'msg> =
        match x with
        | Some x -> { Widgets = MutStackArray1.One (x.Compile ()) }
        | _ -> { Widgets = MutStackArray1.Empty }

type IFabNavigationDrawer =
    inherit IFabContentControl

type IFabSnackbarHost =
    inherit IFabContentControl

type IFabIcon =
    inherit IFabTemplatedControl

type IFabFlex =
    inherit IFabPanel

type IFabCartesianChart =
    inherit IFabUserControl

let FloatingWidgetKey = Widgets.register<FloatingButton>()

module SnackbarHostExt =
    let WidgetKey = Widgets.register<SnackbarHost>()
    
    let Content =
        Attributes.defineAvaloniaPropertyWidget SnackbarHost.ContentProperty

    let HostName =
        Attributes.defineAvaloniaPropertyWithEquality SnackbarHost.HostNameProperty

module IconExt =
    let WidgetKey = Widgets.register<Icon>()
    
    let Value =
        Attributes.defineAvaloniaPropertyWithEquality Icon.ValueProperty

    let FontSize =
        Attributes.defineAvaloniaPropertyWithEquality Icon.FontSizeProperty

    let Animated =
        Attributes.defineAvaloniaPropertyWithEquality Icon.AnimationProperty

    let AttachedIcon =
        Attributes.defineAvaloniaPropertyWithEquality Attached.IconProperty

[<AutoOpen>]
module IconExtBuilders =
    type Fabulous.Avalonia.View with
        static member inline Icon value =
            WidgetBuilder<'msg, IFabIcon> (
                IconExt.WidgetKey,
                IconExt.Value.WithValue value
            )

        static member inline IconSpin value =
            WidgetBuilder<'msg, IFabIcon> (
                IconExt.WidgetKey,
                IconExt.Value.WithValue value,
                IconExt.Animated.WithValue IconAnimation.Spin
            )

[<AutoOpen>]
module SnackbarHostExtBuilders =
    type Fabulous.Avalonia.View with
        static member inline SnackbarHost (content: WidgetBuilder<'msg, #IFabControl>) =
            WidgetBuilder<'msg, IFabSnackbarHost> (
                SnackbarHostExt.WidgetKey,
                AttributesBundle (
                    StackList.one (SnackbarHostExt.HostName.WithValue "Root"),
                    [| SnackbarHostExt.Content.WithValue (content.Compile ()) |],
                    [||],
                    [||]
                )
            )

[<AutoOpen>]
module Extras =
    let Loaded = Attributes.Component.defineEvent "Control_Loaded" (fun target -> (target :?> Avalonia.Controls.Control).Loaded)
    //let Source = Attributes.defineAvaloniaPropertyWithEquality AsyncImageLoader.ImageLoader.SourceProperty
    let Value = Attributes.defineAvaloniaPropertyWithEquality Avalonia.Controls.ProgressBar.ValueProperty
    let Tapped = Attributes.Component.defineEvent "InputElement_Tapped" (fun t -> (t :?> Avalonia.Input.InputElement).Tapped)
    let ComboLabelAssist = Attributes.defineAvaloniaPropertyWithEquality Material.Styles.Assists.ComboBoxAssist.LabelProperty
    let TextBoxLabelAssist = Attributes.defineAvaloniaPropertyWithEquality Material.Styles.Assists.TextFieldAssist.LabelProperty
    let TextChangedRaw = Attributes.Component.defineEvent<Avalonia.Controls.TextChangedEventArgs> "TextBox_TextChanged" (fun target -> (target :?> Avalonia.Controls.TextBox).TextChanged)
    let Text = Attributes.defineAvaloniaPropertyWithEquality Avalonia.Controls.TextBox.TextProperty

    type ExtraModifiers =
        [<Extension>]
        static member inline loaded2 (this: WidgetBuilder<'msg, #IFabControl>, fn) =
            this.AddScalar (Loaded.WithValue fn)

        [<Extension>]
        static member inline tappedHandled (this: WidgetBuilder<'msg, #IFabInputElement>) =
            this.AddScalar (Tapped.WithValue (fun e -> e.Handled <- true))
    
        [<Extension>]
        static member inline autofocus (this: WidgetBuilder<'msg, IFabTextBox>) =
            this.AddScalar (Loaded.WithValue (fun x ->
                let t = unbox<Avalonia.Controls.TextBox> x.Source
                t.Focus () |> ignore
                t.CaretIndex <- 99
            ))

        [<Extension>]
        static member inline tapOnLoad (this: WidgetBuilder<'msg, IFabButton>, v) =
            if v then
                this.AddScalar (Loaded.WithValue (fun x -> (unbox<Avalonia.Controls.Button> x.Source).RaiseEvent (Avalonia.Input.TappedEventArgs (Avalonia.Controls.Button.TappedEvent , Unchecked.defaultof<_>))))
            else
                this

        [<Extension>]
        static member inline label (this: WidgetBuilder<'msg, IFabTextBox>, v) =
            this.AddScalar (TextBoxLabelAssist.WithValue v)

    type Fabulous.Avalonia.View with
        static member AnyInline<'msg, 'marker when 'msg: equality and 'marker :> IFabInline>(widget: WidgetBuilder<'msg, 'marker>) =
            WidgetBuilder<'msg, IFabInline>(widget.Key, widget.Attributes)

        static member inline ProgressBar(min: float, max: float, value: float) =
            WidgetBuilder<'msg, IFabProgressBar>(
                ProgressBar.WidgetKey,
                RangeBase.MinimumMaximum.WithValue(struct (min, max)),
                Value.WithValue(value)
            )

        static member inline Combo (label, items, template) =
            Fabulous.Avalonia.View.ComboBox(items, template).classes("outline").AddScalar (ComboLabelAssist.WithValue label)

        static member inline TextBox (label, text, fn) =
            Fabulous.Avalonia.View.TextBox(text, fn).classes("outline").AddScalar (TextBoxLabelAssist.WithValue label)

        static member inline ButtonTapHandled(text: string, fn: unit -> 'msg) =
            WidgetBuilder<'msg, IFabButton>(Button.WidgetKey, ContentControl.ContentString.WithValue(text), MvuInputElement.Tapped.WithValue (fun x -> x.Handled <- true; fn ()))

        static member inline ButtonTapHandled(text: string, fn: unit -> unit) =
            WidgetBuilder<'msg, IFabButton>(Button.WidgetKey, ContentControl.ContentString.WithValue(text), Tapped.WithValue (fun x -> x.Handled <- true; fn ()))


        static member inline ButtonWithIcon(fn: 'msg, value) =
            WidgetBuilder<'msg, IFabButton>(
                Button.WidgetKey,
                MvuButton.Clicked.WithValue (fun _ -> box fn),
                IconExt.AttachedIcon.WithValue value
            )

        static member inline ButtonWithIconTapHandled(fn: unit -> 'msg, value) =
            WidgetBuilder<'msg, IFabButton>(
                Button.WidgetKey,
                MvuInputElement.Tapped.WithValue (fun x -> x.Handled <- true; fn ()),
                IconExt.AttachedIcon.WithValue value
            )

        static member inline ButtonWithIcon(value) =
            WidgetBuilder<'msg, IFabButton>(
                Button.WidgetKey,
                IconExt.AttachedIcon.WithValue value
            )

        static member inline ButtonWithIconTapHandled(value) =
            WidgetBuilder<'msg, IFabButton>(
                Button.WidgetKey,
                Tapped.WithValue (fun x -> x.Handled <- true),
                IconExt.AttachedIcon.WithValue value
            )
