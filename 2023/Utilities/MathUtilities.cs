namespace Utilities;

public class MathUtilities
{
    public static long LCM(IEnumerable<long> numbers)
    {
        return numbers.Aggregate(LCM);
    }
    
    public static long LCM(long num1, long num2)
    {
        return Math.Abs(num1 * num2) / GCD(num1, num2);
    }
    
    public static long GCD(long num1, long num2)
    {
        return num2 == 0 ? num1 : GCD(num2, num1 % num2);
    }
}