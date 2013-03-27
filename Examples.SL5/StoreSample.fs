/// Store sample based on Moq's store sample:
//  http://code.google.com/p/moq/source/browse/trunk/Samples/StoreSample/#StoreSample
namespace Moq.FSharp.Extensions.SL5.Tests.Store

open System

type Category = { Id : int; Name : string }
type Product = { Id : int; Name : string }
type Order(product:Product, quantity:int) =
    member val Product = product
    member val Quantity = quantity
    member val Filled = false with get, set

type ICatalogService = 
    abstract GetCategories : unit -> Category seq
    abstract GetProducts : categoryId:int -> Product seq
    abstract HasInventory : productId:int * quantity:int -> bool
    abstract Remove : productId:int * quantity:int -> unit

type IProductsView =
    [<CLIEvent>]
    abstract CategorySelected : IEvent<CategoryEventArgs>
    abstract SetCategories : categories:Category seq -> unit
    abstract SetProducts : products:Product seq -> unit
and CategoryEventArgs(category:Category) =
    inherit EventArgs()
    member args.Category = category

type ProductsPresenter(catalog:ICatalogService, view:IProductsView) =
    do  view.SetCategories(catalog.GetCategories()) 
        view.CategorySelected.Add(fun args -> 
            view.SetProducts(catalog.GetProducts(args.Category.Id))
        )
    member presenter.PlaceOrder(order:Order) =
        if catalog.HasInventory(order.Product.Id, order.Quantity) then
            try catalog.Remove(order.Product.Id, order.Quantity)
                order.Filled <- true
            with :? InvalidOperationException -> () // LOG?

open Microsoft.Silverlight.Testing
open Microsoft.VisualStudio.TestTools.UnitTesting
open Moq
open Moq.FSharp.Extensions

[<TestClass>]
type ``Store Sample`` () =
    
    [<TestMethod>] 
    member test.``should set view categories`` () =
        // Arrange
        let catalog = Mock.Of<ICatalogService>()
        let view = Mock.Of<IProductsView>()   
        // Act
        let presenter = ProductsPresenter(catalog, view)
        // Assert
        Mock.Get(view).VerifyAction(fun view -> view.SetCategories(any()))

    [<TestMethod>] 
    member test.``should category selection set products`` () =
        // Arrange
        let catalog = Mock.Of<ICatalogService>()   
        let view = Mock<IProductsView>()
        let presenter = ProductsPresenter(catalog, view.Object)
        // Act
        view.RaiseHandler(
            (fun v -> v.CategorySelected.AddHandler(null)), 
            CategoryEventArgs({Category.Id=1; Name="" }))
        // Assert
        view.VerifyAction(fun v -> v.SetProducts(any()))

    [<TestMethod>] 
    member test.``should not place order if not enough inventory`` () =
        // Arrange
        let catalog = Mock<ICatalogService>()
        catalog.SetupFunc(fun c -> c.HasInventory(1,5)).Returns(false).End
        let view = Mock.Of<IProductsView>()
        let presenter = ProductsPresenter(catalog.Object, view);
        let order = Order(product={Id=1; Name=""}, quantity=5)
        // Act
        presenter.PlaceOrder(order);
        // Assert
        Assert.IsFalse(order.Filled)
        catalog.VerifyFunc(fun catalog -> catalog.HasInventory(1, 5))

    [<TestMethod>] 
    member test.``should not place order if fails to remove`` () = 
        // Arrange
        let catalog = Mock<ICatalogService>()
        catalog.SetupFunc(fun c -> c.HasInventory(1, 5)).Returns(true).End
        catalog.SetupAction(fun c -> c.Remove(1,5)).Throws<InvalidOperationException>().End
        let view = Mock.Of<IProductsView>();
        let presenter = ProductsPresenter(catalog.Object, view);
        let order = Order(product={Id=1; Name=""}, quantity=5)
        // Act
        presenter.PlaceOrder(order)
        // Assert
        Assert.IsFalse(order.Filled);
        catalog.VerifyFunc(fun c -> c.HasInventory(1, 5))
        catalog.VerifyAction(fun c -> c.Remove(1, 5))