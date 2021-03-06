﻿/// Balance Calculator based on Richard Carr's code samples in
/// Using Mocks: http://www.blackwasp.co.uk/Mocks.aspx
namespace Moq.FSharp.Extensions.SL5.Tests.Balance

type ITransactionRetriever = 
    abstract GetTransactions : period:int -> decimal[]

type BalanceCalculator(retriever:ITransactionRetriever) =
    member this.GetBalance(startBalance:decimal, period:int) =
        try
            let transactions = retriever.GetTransactions(period)
            startBalance + (transactions |> Array.sum)
        with 
            :? System.ArgumentException -> startBalance
    member this.GetBalance(startBalance:decimal, startPeriod:int, endPeriod:int) =
        let mutable runningTotal = startBalance
        for period = startPeriod to endPeriod do
            runningTotal <- this.GetBalance(runningTotal, period)
        runningTotal

open Microsoft.Silverlight.Testing
open Microsoft.VisualStudio.TestTools.UnitTesting
open Moq
open Moq.FSharp.Extensions

[<TestClass>]
type ``Balance Example`` () =

    [<TestMethod>]
    member test.``total is correct for a period with transactions`` () =
        let retriever = Mock<ITransactionRetriever>()
        retriever.SetupFunc(fun m -> m.GetTransactions(1)).Returns([|1M; 2M; 3M; 4M|]).End
        let calculator = BalanceCalculator(retriever.Object)
        Assert.AreEqual(15M, calculator.GetBalance(5M, 1))

    [<TestMethod>]
    member test.``total is correct for multiple periods with transactions`` () =
        let money = Array.map decimal
        let retriever = Mock<ITransactionRetriever>()
        retriever.SetupFunc(fun m -> m.GetTransactions(1)).Returns(money [|1; 2; 3; 4|]).End
        retriever.SetupFunc(fun m -> m.GetTransactions(2)).Returns(money [|5; 6; 7|]).End
        retriever.SetupFunc(fun m -> m.GetTransactions(3)).Returns(money [|8; 9; 10|]).End           
        let calculator = BalanceCalculator(retriever.Object);
        Assert.AreEqual(60M, calculator.GetBalance(5M, 1, 3))

    [<TestMethod>]
    member test.``total is zero for an invalid period`` () =
        let retriever = Mock<ITransactionRetriever>()
        retriever.SetupFunc(fun m -> m.GetTransactions(0)).Throws<System.ArgumentException>().End
        let calculator = BalanceCalculator(retriever.Object)
        Assert.AreEqual(5M, calculator.GetBalance(5M, 0))

    [<TestMethod>]
    member test.``total is correct for multiple matching periods with transactions`` () =
        let retriever = Mock<ITransactionRetriever>()
        retriever.SetupFunc(fun m -> m.GetTransactions(any())).Returns([|1M;2M;3M;4M|]).End
        let calculator = new BalanceCalculator(retriever.Object)
        Assert.AreEqual(35M, calculator.GetBalance(5M, 1, 3))