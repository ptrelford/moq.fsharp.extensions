﻿module Moq.FSharp.Extensions

open System
open System.Linq.Expressions

// Moq extensions for F#
type Moq.Mock<'TAbstract> when 'TAbstract : not struct with
    /// Specifies a setup on the mocked type for a call to a function
    member mock.SetupFunc<'TResult>(expression:Expression<Func<'TAbstract,'TResult>>) =
        mock.Setup<'TResult>(expression)
    /// Specifies a setup on the mocked type for a call to a void method
    member mock.SetupAction(expression:Expression<Action<'TAbstract>>) = 
        mock.Setup(expression)
    /// Specifies a setup on the mocked type for a call to a property setter
    member mock.SetupSetAction<'TProperty>(setupExpression:Action<'TAbstract>) 
        : Moq.Language.Flow.ISetupSetter<'TAbstract,'TProperty> = 
        mock.SetupSet<'TProperty>(setupExpression)

type Moq.Language.Flow.IReturnsResult<'TMock> with
    /// Ends setup
    member __.End = ()

type Moq.Language.Flow.ICallbackResult with
    /// Ends setup
    member __.End = () 

/// Matches any value
let inline any () = Moq.It.IsAny()
/// Matches any value that satisfies the given predicate
let inline is f = Moq.It.Is(f)
/// Creates a mock object
let inline mock () = Moq.Mock.Of()
