module repro.Android

open Android.App
open Android.Content.PM
open Avalonia
open Avalonia.Android
open Fabulous.Avalonia
open Microsoft.Maui.ApplicationModel

[<Activity(Label = "repro",
           Theme = "@style/Theme.AppCompat.DayNight",
           LaunchMode = LaunchMode.SingleTop,
           MainLauncher = true,
           ConfigurationChanges = (ConfigChanges.Orientation ||| ConfigChanges.ScreenSize ||| ConfigChanges.UiMode))>]
[<Sealed>]
type MainActivity () =
    inherit AvaloniaMainActivity<FabApplication> ()

    override _.OnRequestPermissionsResult (c, p, r) =
        Platform.OnRequestPermissionsResult (c, p, r)
        base.OnRequestPermissionsResult (c, p, r)

    override this.OnCreate s =
        base.OnCreate s
        Platform.Init (this, s)

    override this.CreateAppBuilder () =
        (Main.configure (this :> IAvaloniaActivity).Activated).UseAndroid ()
