namespace Moq.FSharp.Extensions.SL5.Tests

open System.Windows
open Microsoft.Silverlight.Testing
            
type App() as this = 
    inherit Application()
    do Application.LoadComponent(this, new System.Uri("/Moq.FSharp.Extensions.SL5.Tests;component/App.xaml", System.UriKind.Relative));
        
    let runTests () =
        let settings = UnitTestSystem.CreateDefaultSettings()
        //settings.StartRunImmediately <- true;
        this.RootVisual <- UnitTestSystem.CreateTestPage(settings)
    do this.Startup.Add(fun _ -> runTests())
        
    do this.Exit.Add(fun _ -> ())

    let showError (e:exn) =
        let errorMsg = e.Message + e.StackTrace
        let errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n")
        System.Windows.Browser.HtmlPage.Window.Eval(
            "throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");") |> ignore

    do this.UnhandledException.Add(fun e -> 
        // If the this is running outside of the debugger then report the exception using
        // the browser's exception mechanism. On IE this will display it a yellow alert 
        // icon in the status bar and Firefox will display a script error.
        if not System.Diagnostics.Debugger.IsAttached then
            // NOTE: This will allow the this application to continue running after an exception 
            // has been thrown but not handled. 
            // For production this error handling should be replaced with something that will 
            // report the error to the website and stop the thislication.
            e.Handled <- true;
            Deployment.Current.Dispatcher.BeginInvoke(fun _ -> 
                try showError e.ExceptionObject with _ -> ()) |> ignore)
       