namespace csharp_objects;

public class ClassA
{
    public int firstValue;
    private int secondValue;
    private int thirdValue;
    public ClassA(int field01Value)
    {
        this.secondValue = field01Value;
    }
    public int ThirdValue
    {
        get
        {
            return thirdValue;
        }
        set
        {
            this.thirdValue = value;
        }
    }
    public string ReturnValues()
    {
        this.firstValue = this.firstValue + 15;
        string finalString = $"A first value: {this.firstValue}, a second value: {this.secondValue}, a third value: {this.thirdValue}.";
        return finalString;
    }
    public int ReturnValues(int loopCount)
    {
        int valueSum = 0;
        for (int i = 0; i < loopCount; i++)
        {
            valueSum = valueSum + ((this.firstValue + this.secondValue + this.thirdValue) * i);
        }
        return valueSum;
    }
}
class Program
{
    static void Main(string[] args)
    {
        ClassA classA = new ClassA(25);
        classA.firstValue = 15;
        classA.ThirdValue = 35;

        Console.WriteLine(classA.ReturnValues());
        Console.WriteLine(classA.ReturnValues(5));
    }
}
