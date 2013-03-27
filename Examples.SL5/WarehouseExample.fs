/// Warehouse example based on Martin Fowler's code samples in
/// "Mock's Aren't Stubs" article: http://martinfowler.com/articles/mocksArentStubs.html
namespace Moq.FSharp.Extensions.SL5.Tests.Warehouse

type Product = string
type Quantity = int

type Warehouse =
    abstract HasInventory: Product * Quantity -> bool
    abstract Remove : Product * Quantity -> unit

type MailService =
    abstract Send : Message -> unit
and  Message = string

type Order(product, quantity) =
    let mutable filled = false
    let mutable mailer = { new MailService with member __.Send(_) = () }
    member order.SetMailer(newMailer) = mailer <- newMailer 
    member order.Fill(warehouse:Warehouse) =
        if warehouse.HasInventory(product, quantity) then 
            warehouse.Remove(product, quantity)
            filled <- true
        else mailer.Send("Unfilled")
    member order.IsFilled = filled

open Microsoft.Silverlight.Testing
open Microsoft.VisualStudio.TestTools.UnitTesting
open Moq
open Moq.FSharp.Extensions

[<TestClass>]
type ``Warehouse example`` () =    

    [<TestMethod>] 
    member test.``filling removes inventory if in stock`` () =
        // setup data
        let product, quantity = "TALISKER", 50
        let order = Order(product, quantity)
        // setup mock behavior
        let warehouse = Mock<Warehouse>()
        warehouse.SetupFunc(fun mock -> mock.HasInventory(product, quantity)).Returns(true).End
        // exercise
        order.Fill(warehouse.Object)
        // verify expectations
        warehouse.VerifyFunc(fun mock -> mock.HasInventory(product,quantity))
        warehouse.VerifyAction(fun mock -> mock.Remove(product, quantity))
        Assert.IsTrue(order.IsFilled)

    [<TestMethod>]
    member test.``filling does not remove if not enough in stock`` () =
        // setup data
        let product, quantity = "TALISKER", 51
        let order = Order(product, quantity)
        // setup mock behavior
        let warehouse = Mock<Warehouse>()
        warehouse.SetupFunc(fun mock -> mock.HasInventory(product, quantity)).Returns(false).End
        // exercise
        order.Fill(warehouse.Object)
        // verify expectations
        warehouse.VerifyAction((fun mock -> mock.Remove(product, quantity)), Times.Never())
        Assert.IsFalse(order.IsFilled)

    [<TestMethod>] 
    member test.``order sends mail if unfilled`` () =
        // setup data
        let order = Order("TALISKER", 51)
        let mailer = mock()
        order.SetMailer(mailer)
        // exercise
        order.Fill(mock())
        // verify
        Mock.Get(mailer).VerifyAction(fun mock -> mock.Send(any()))