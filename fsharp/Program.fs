open System
open FSharp.Linq.RuntimeHelpers
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open Swensen.Unquote

type TypeExtentions =
    static member PropertyName([<ReflectedDefinition>] exp:Expr<_ -> _>) =
        match exp with
        | Lambda(arg, PropertyGet(_, propertyInfo, _)) ->
            Some(propertyInfo.DeclaringType.Name, propertyInfo.Name)
        | _ -> None

    static member MethodName([<ReflectedDefinition>] exp:Expr<_ -> _>) =
        match exp with
        | Lambdas(_, Call(_, methodInfo,_)) ->
            Some (methodInfo.DeclaringType.Name, methodInfo.Name)
        | _ -> None

type Foo =
    {Bar: int}
    member x.Baz(red, green) = 0

[<EntryPoint>]
let main argv =
    printfn "PropertyName %A" (TypeExtentions.PropertyName (fun (x: Foo) -> x.Bar))
    printfn "MethodName %A" (TypeExtentions.MethodName (fun (x: Foo) -> x.Baz))

    //Evaluation
    let add = <@ Func<unit, int>(fun () -> 1 + 2) @>
    let func = LeafExpressionConverter.QuotationToLambdaExpression add
    let compiled = func.Compile()
    let answer = compiled.Invoke()
    Console.WriteLine("The answer is {0}", answer)

    //unquote
    let answer = <@ fun () -> 1 + 2 @>.Eval()()
    Console.WriteLine("The answer is {0}", answer)

    0 // return an integer exit code
