namespace FsharpNamespace

type FsharpClass(value01:string, value02:int) =
    member this.Value01 = value01
    member this.Value02 = value02

    member this.FsharpMethod01(value03) =
        let value04 = this.Value02 + value03
        let matchValues x =
            match x with
            | i when i < 0 -> printfn "Low"
            | i when i >= 0 && i < 10 -> printfn "Medium"
            | i when i >= 10 && i < 100-> printfn "High"
            | _ -> printfn "Outside the range. Additional info: %s." this.Value01
        matchValues value04

    member this.FsharpMethod02(firstValue, secondValue) =
        let listToFilter = [firstValue..secondValue]
        let listFiltered = listToFilter |> List.filter(fun i -> i > 4) |> List.sumBy(fun i -> i)
        listFiltered

// let firstObject = FsharpClass("Some info", 2)
// firstObject.FsharpMethod01(4000)
// printfn "The valuer returned by the second method is: %d" (firstObject.FsharpMethod02(1,7))