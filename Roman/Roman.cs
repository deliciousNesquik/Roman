namespace Roman;

public class Roman
{
    
    private readonly int _value;
    private readonly string _svalue;
    
    private static readonly Dictionary<char, int> RomanMap = new()
    {
        {'N', 0}, {'I', 1}, {'V', 5}, {'X', 10}, {'L', 50}, {'C', 100}, {'D', 500}, {'M', 1000}
    };
    
    public static Roman operator +(Roman a, Roman b) => new(a._value + b._value);
    public static Roman operator -(Roman a, Roman b) => new(a._value - b._value);
    public static Roman operator *(Roman a, Roman b) => new(a._value * b._value);
    public static Roman operator /(Roman a, Roman b) => new(a._value / b._value);
    public static bool operator >(Roman a, Roman b) => a._value > b._value;
    public static bool operator <(Roman a, Roman b) => a._value < b._value;
    public static bool operator >=(Roman a, Roman b) => a._value >= b._value;
    public static bool operator <=(Roman a, Roman b) => a._value <= b._value;
    public static bool operator ==(Roman a, Roman b) => a._value == b._value;
    public static bool operator !=(Roman a, Roman b) => a._value != b._value;
    public static bool operator ! (Roman a) => a._value == 0;
    public static bool operator true (Roman a) => a._value != 0;
    public static bool operator false (Roman a) => a._value == 0;

    public Roman(int value){
        _value = Math.Abs(value);
        _svalue = ToRoman(_value);
    }
    public Roman() : this(0) 
    {}
    public Roman(string svalue) : this(ToInt(svalue)) 
    {}
    public Roman(Roman other) : this(other._value) 
    {}
    
    /// <summary>Преобразует целочисленное значение в объект римского числа</summary>
    /// <param name="number">Целочисленное значение для преобразования</param>
    /// <returns>Возвращает новый объект римского числа</returns>
    public static Roman Parse(int number) => new(number);
    
    /// <summary>Преобразует строковое римское представление в объект римского числа</summary>
    /// <param name="roman">Строковое римское представление</param>
    /// <returns>Возвращает новый объект римского числа</returns>
    public static Roman Parse(string roman) => new(roman);
    
    /// <summary>
    /// Возвращает строковое римское представление числа
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _svalue;
    
    /// <summary>
    /// Возвращает целочисленное значение римского числа
    /// </summary>
    /// <returns></returns>
    public int ToInt() => _value;
    
    
    private static int ToInt(string roman){
        var number = 0;
        var before = 0;
        
        roman = roman[0] == '-' ? roman.Replace("-", "") : roman;

        if (string.IsNullOrEmpty(roman)) return number;
        
        for(var i = roman.Length - 1; i >= 0; i--) {

            if (RomanMap.TryGetValue(roman[i], out _))
            {
                var current = RomanMap[roman[i]];
            
                if (current < before) {
                    number -= current;
                } else {
                    number += current;
                }
                before = current;
            }
            else
            {
                throw new ArgumentException("Invalid Roman numeral character.");
            }
        }
        return number;
    }
    
    private static string ToRoman(int number)
    {
        if (number == 0) return "N";
        
        string[] thousands = {"", "M", "MM", "MMM", "MMMM", "MMMMM",
            "MMMMMM", "MMMMMMM", "MMMMMMMMM", "MMMMMMMMMM"};
        
        string[] hundreds =  {"", "C", "CC", "CCC", "CD", "D",
            "DC", "DCC", "DCCC", "CM"};
        
        string[] dozens =    {"", "X", "XX", "XXX", "XL", "L",
            "LX", "LXX", "LXXX", "XC"};
        
        string[] units =     {"", "I", "II", "III", "IV", "V",
            "VI", "VII", "VIII", "IX"};

        return  thousands[number / 1000] +
                hundreds[(number % 1000) / 100] +
                dozens[(number % 100) / 10] +
                units[number % 10];
    }
}