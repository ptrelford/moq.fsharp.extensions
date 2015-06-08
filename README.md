# README #

Extension methods and usage examples for calling Moq from F# 3.

```fsharp
let [<Test>] ``order sends mail if unfilled`` () =
    // setup data
    let order = Order("TALISKER", 51)
    let mailer = mock()
    order.SetMailer(mailer)
    // exercise
    order.Fill(mock())
    // verify
    Mock.Get(mailer).VerifyAction(fun mock -> mock.Send(any()))
```

### Download

Available as a package on [Nuget](http://nuget.org/packages/Moq.FSharp.Extensions/) or simply copy [Moq.FSharp.Extensions.fs](Available as a package on [Nuget](http://nuget.org/packages/Moq.FSharp.Extensions/) or simply copy [Moq.FSharp.Extensions.fs]) into your test project.