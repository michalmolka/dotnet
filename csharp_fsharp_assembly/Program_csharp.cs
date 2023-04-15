using FsharpNamespace;

namespace csharp;
class Program
{
    static void Main(string[] args)
    {
        FsharpClass fsharpclass = new FsharpClass("Some info", 5);
        fsharpclass.FsharpMethod01(4);
        fsharpclass.FsharpMethod02(2,5);
        Console.WriteLine($"The value returned by the second method is: {fsharpclass.FsharpMethod02(1,7)}");
    }
}
