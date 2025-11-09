namespace Roman;

public class Checking
{
    public static void Main()
    {
        var romanInt = new Roman(10); // X
        var romanString = new Roman("V"); // V
        var romanRoman = new Roman(romanInt + romanString); // XV
    }
}