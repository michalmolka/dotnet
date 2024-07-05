namespace CsharpFunctions;

public delegate double FunctionDelegate(bool c, double n01, double n02);
class Program
{
    static void Main(string[] args)
    {
        // #1. A ReturnNumber function, returns a double type.
        static double ReturnNumber(bool condition, double number01, double number02)
        {
            double finalNumber = 0;
            if (condition)
            {
                finalNumber = number01 + 15;
            }
            else
            {
                finalNumber = number02 + 25;
            }
            return finalNumber;
        }

        double returnedNumber = ReturnNumber(false, 5.5, 8.5) + 55;
        Console.WriteLine($"1: A ReturnNumber function: {returnedNumber}");


        // #2. A ReturnText function, returns nothing.
        static void ReturnText(int choose, string sentencePart01, string sentencePart02)
        {
            string finalSentence;
            switch (choose)
            {
                case 50:
                    finalSentence = $"A sentence part 01: {sentencePart01}, Sentence part 02: {sentencePart02}.";
                    break;
                case 100:
                    finalSentence = $"A sentence part 01: {sentencePart02}, Sentence part 02: {sentencePart02}.";
                    break;
                default:
                    finalSentence = "Only a sentence.";
                    break;
            }
            Console.WriteLine($"2: A ReturnText function: {finalSentence}");
        }

        ReturnText(50, "Part01", "Part02");


        // #3. A function as a function parameter.
        List<double> list01 = new List<double>{155.5, 255.6, 355.7};

        static double Functions01(Func<bool, double, double, double> function01, List<double> list01){

            double value01 = 0;
            foreach (double item in list01){
                value01 = value01 + function01(true, item, 2.5); 
            }
            return value01; 
        }

        Console.WriteLine($"3: A function as a function parameter: {Functions01(ReturnNumber, list01)}");


        // #4. A function as a function parameter - a delegate.
        static double Functions02(FunctionDelegate function02, List<double> list01){
  
            double value01 = 0;
            foreach (double item in list01){
                value01 = value01 + function02(true, item, 2.5); 
            }
            return value01; 
        }

        Console.WriteLine($"4: A function as a function parameter - a delegate: {Functions02(ReturnNumber, list01)}");
    }
}
