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