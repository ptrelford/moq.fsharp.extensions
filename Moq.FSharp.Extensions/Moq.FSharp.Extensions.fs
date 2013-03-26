module Moq.FSharp.Extensions

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
    /// Verifies that a specific invocation matching the given expression was performed on the mock
    member mock.VerifyFunc(expression:Expression<Func<'TAbstract,'TResult>>, times:Moq.Times) =
        mock.Verify(expression, times)
    /// Verifies that a specific invocation matching the given expression was performed on the mock
    member mock.VerifyFunc(expression:Expression<Func<'TAbstract,'TResult>>) =
        mock.Verify(expression)
    /// Verifies that a specific invocation matching the given expression was performed on the mock
    member mock.VerifyAction(expression:Expression<Action<'TAbstract>>) =
        mock.Verify(expression)
    /// Verifies that a specific invocation matching the given expression was performed on the mock
    member mock.VerifyAction(expression:Expression<Action<'TAbstract>>, times:Moq.Times) =
        mock.Verify(expression, times)
    /// Raises the event referenced in the event expression using the given args
    member mock.RaiseHandler(eventExpression:Action<'TAbstract>, args:#EventArgs) =
        mock.Raise(eventExpression, args)

type Moq.Language.Flow.IReturnsResult<'TMock> with
    /// Ends setup
    member __.End = ()

type Moq.Language.Flow.ICallbackResult with
    /// Ends setup
    member __.End = () 

type Moq.Language.Flow.IThrowsResult with
    /// Ends setup
    member __.End = ()

/// Matches any value
let inline any () = Moq.It.IsAny()
/// Matches any value that satisfies the given predicate
let inline is f = Moq.It.Is(f)
/// Creates a mock object
let inline mock () = Moq.Mock.Of()