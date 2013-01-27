#r @"..\packages\Moq.4.0.10827\lib\NET40\Moq.dll"
#load "Moq.FSharp.Extensions.fs"

open System
open Moq
open Moq.FSharp.Extensions
      
let inline Assert success = if not success then failwith "Failed"

type IFoo =
    abstract DoSomething : string -> bool
    abstract Value : int with get, set

let ``mock a method that returns a value`` =
    let mock = Mock<IFoo>()
    mock.Setup<bool>(fun foo -> foo.DoSomething("ping")).Returns(true) |> ignore
    Assert(mock.Object.DoSomething("ping"))
    
let ``mock a method that returns a value without using a type annotation`` =
    let mock = Mock<IFoo>()
    mock.SetupFunc(fun foo -> foo.DoSomething("ping")).Returns(true) |> ignore
    Assert(mock.Object.DoSomething("ping"))  
    
let ``mock a property setter`` =
    let mock = Mock<IFoo>()
    let value = ref None
    mock.SetupSet<int>(fun foo -> foo.Value <- It.IsAny<_>()).Callback(fun x -> value := Some x)
    |> ignore
    mock.Object.Value <- 1
    Assert(!value |> Option.exists((=) 1))

let ``mock a property setter without using a type annotation`` =
    let mock = Mock<IFoo>()
    let value = ref None
    mock.SetupSetAction(fun foo -> foo.Value <- It.IsAny<_>()).Callback(fun x -> value := Some x)
    |> ignore
    mock.Object.Value <- 1
    Assert(!value |> Option.exists((=) 1))

open System.ComponentModel

let ``mock raising an event`` =
    let mock = Mock<INotifyPropertyChanged>()
    let changed = ref false
    mock.Object.PropertyChanged.Add(fun x -> changed := true) 
    mock.Raise((fun (x:INotifyPropertyChanged) -> x.PropertyChanged.AddHandler(null)), 
               PropertyChangedEventArgs("Hello"))
    Assert(changed.Value)