open FSharp.Data
open System
////dotnet fsi abc.fsx

// An immutable variable
let variable00 = 50 // a type not defined
let (variable01: int) = 100 // a type defined

// A mutable variable
let mutable variable02 = "This is a string" // a type not defined
let mutable (variable03:int) = 100 // a type defined
variable02 <- "A string second version" // assign a new value

printfn "%d" variable00
// 50
printfn $"{variable01}"
// 100
printfn "%s" variable02
// A string second version


// A list
let list00 = [ 5; 6; 8 ]
let list01 = [ 20..30 ]
let list02 = List.init<int> 10 (fun i -> i)

let list03 = list01 |> List.append list00

printfn "%A" list03
// [5; 6; 8; 20; 21; 22; 23; 24; 25; 26; 27; 28; 29; 30]

printfn "%d" (list00.Item(1))
// 6

printfn "%d" (list00.Length)
// 3

// A sequence
let sequence00 = { 1..50 } // increment +1
let sequence01 = {1..10..101} // increment +10
let sequence02 = Seq.init<int> 20 (fun i -> i)

//  A tuple
let tuple00 = (2, 3, 55)

//  An array
let array00 = [| 1; 4; 5 |]
let array01 = Array.init<int> 10 (fun i -> i)
let array02 = Array2D.init<int> 5 5 (fun a b -> a + 1)

// A record
type record00 = { Value01: int; Value02: string }
let record00Row = { Value01 = 15; Value02 = "element01" } // a row record
let record00RowUpdated = { record00Row with Value01 = 50 } // update a record

printfn $"{record00Row.Value01}"
// 15
printfn $"{record00RowUpdated.Value01}"
// 50

//Discriminated union
type ProjectState =
    | Active
    | Closed

type CompanyProject = // this is a normal record, but State is enforced by Discriminated union
    {
        Name:string
        Type:int
        State:ProjectState
    }

let (firstProject:CompanyProject) =
    {
        Name = "Project01"
        Type = 5
        State = Active
    }

printfn $"{firstProject.State}"
// Active

// A list of records
type Transaction = {
    Date:DateTime;
    CustomerId:int;
    Amount:double
    }

let mutable transactions00 = [
    {
        Date = new DateTime(2023, 12, 01);
        CustomerId = 10;
        Amount = 10.5
    };
    {
        Date = new DateTime(2023, 12, 02);
        CustomerId = 11;
        Amount = 11.5
    };
    {
        Date = new DateTime(2023, 4, 4);
        CustomerId = 14;
        Amount = 11.4
    }
]

printfn $"{transactions00.Head}" // a first record
printfn $"{transactions00.Tail}" // a last record
printfn $"{transactions00.[0]}" // a record with index 0

// lokk for particular records, you can use List.tryFind as well
let tF = transactions00 |> List.find (fun transaction -> transaction.CustomerId = 10)

printfn $"{tF.Amount}"
// 10.5

// add a record to a transaction records list
transactions00 <- [{Date = new DateTime(2024, 05,05);CustomerId = 15; Amount = 12.5}] |> List.append transactions00

//convert list to an array
let transactionsArray = transactions00 |> Array.ofList

// modify an Amount value for every record
let multipler = 2.0
let transactions01 = transactions00 |> List.map (fun tr ->  {| Amount = tr.Amount * multipler|})

// print all the records
transactions00 |> List.iter(fun tr -> printfn $"{tr.Amount}")

// aggregate, sum in this case, 
transactions00 |> List.sumBy(fun tr -> tr.Amount)

//filter and sort
transactions00 |> List.filter(fun tr -> tr.Date = DateTime(2023, 4,4)) |> List.sortBy(fun tr -> tr.CustomerId)


// Clases
type Customer(firstName:string, lastName:string, age:int) =
    //properties
    member this.FirstName = firstName //immutable
    member this.LastName = lastName
    member val Age = age with get,set //mutable
    
    //method
    member this.ageAdd =
        this.Age <- this.Age + 5

let firstCustomer = Customer("John", "Doe", 45) // an object initialisation

printfn $"{firstCustomer.Age}"
// 45
firstCustomer.ageAdd
printfn $"{firstCustomer.Age}"
// 50

// A for loop
for i=3 to 20 do
    printfn "%d" i

for i=20 downto 1 do
    printfn "%d" i

let listA = [1..5]
let listB = [for t in listA do t + 5]

// A for each loop
listB |> List.iter(fun tr -> printfn "%d" tr)

for item in listB do
    printfn "%d" item

// A while loop
let mutable input = ""

while (input <> "q") do
    input <- Console.ReadLine()
    printfn "%s" input

// An if condition
if 2=2 then
    printfn "%d" 1
elif 2=3 then
    printfn "%d" 2
else
    printfn "%d" 3


// Pattern matching - switch
let matchFunction x =
    match x with
    | 1 -> printfn "%s" "is 1"
    | 2 -> printfn "%s" "is 2"
    |i when i  >2 && i <5 -> printfn "< 5 > 2"
     | _ -> printfn "%s" "is more than 5"
printfn "----"
matchFunction 6
// is more than 5


//// Try catch
let someFunction x y =
    try
        x/y
    with
        | :? System.DivideByZeroException -> printfn "Division custom exception";0
        | ex -> printfn "Some other err";0

someFunction 5 0
// Division custom exception


// Functions
let addValues00 valueA valueB = valueA + valueB
let nextValue00 (valueA:int) (valueB:int):int = valueA + valueB + 1
let nextValueCalc = nextValue00 1 6
let addValues01 = fun valueA valueB -> valueA + valueB

printfn $"Function: {addValues00 5 15}, {addValues01 5 5}"
// "Function: 20, 10"


// Pipelines
let seqence00 = seq{10..20}

let finalSeq =
    seqence00
    |> Seq.filter(fun c -> (c%2)=0)
    |> Seq.map((*) 2)
    |> Seq.map(sprintf"The value is %i")

printfn "%A" finalSeq
// seq["The value is 20"; "The value is 24"; "The value is 28"; "The value is 32";...]

let finalFunc =
    2
    |> addValues00 <| 3
    |> nextValue00 <| 20
    |> printfn "%d"


// Composition
let addValues03 (valueA:int):int = valueA*2
let nextValue01 (valueA:int):int = (valueA + 1)*2

let finalFunc01 = addValues03 >> nextValue01 >> printfn "%d"


// Modules
module addFunctionModule =
    let addFunction x y =
        x + y

open addFunctionModule
addFunction 5 5


// Asynchronous approach
let asyncFunction x  =
    async {
        let a = x * 5
        return a
    }

5 |> asyncFunction |> Async.RunSynchronously |> printfn "%d"

let list05 = [3;4;5]

list05
|> List.map asyncFunction
|> Async.Parallel
|> Async.RunSynchronously
|> printfn "%A"

list05
|> List.map asyncFunction
|> Async.Sequential
|> Async.RunSynchronously
|> printfn "%A"
