using System.Text;

namespace Roman;

public sealed class Roman : IComparable<Roman>, IEquatable<Roman>
{
    /// <summary>Значение числа в арабской системе счисления.</summary>
    private readonly int _value;

    /// <summary>Таблица для преобразования чисел в римские символы.</summary>
    private static readonly (int Value, string Symbol)[] Map =
    {
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"),  (90, "XC"),  (50, "L"),  (40, "XL"),
        (10, "X"),   (9, "IX"),   (5, "V"),   (4, "IV"), (1, "I")
    };

    #region Конструкторы

    /// <summary>Создаёт римское число по целому значению (1–3999).</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Выбрасывается, если значение выходит за диапазон 1–3999.
    /// </exception>
    public Roman(int value)
    {
        if (value < 1 || value > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 3999.");
        _value = value;
    }

    /// <summary>Создаёт римское число из строкового представления.</summary>
    public Roman(string roman) : this(ToInt(roman)) { }

    /// <summary>Создаёт копию другого римского числа.</summary>
    public Roman(Roman other) : this(other._value) { }

    #endregion
    
    #region Арифметика и сравнение

    public static Roman operator +(Roman a, Roman b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));

        long sum = (long)a._value + b._value;
        if (sum > 3999) throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be ≤ 3999.");
        return new Roman((int)sum);
    }
    public static Roman operator -(Roman a, Roman b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));
        
        var result = a._value - b._value;
        if (result < 1)
            throw new ArgumentOutOfRangeException(nameof(b), "Roman numerals cannot represent zero or negative values.");
        return new Roman(result);
    }

    public static Roman operator *(Roman a, Roman b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));

        long prod = (long)a._value * b._value;
        if (prod > 3999) throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be ≤ 3999.");
        return new Roman((int)prod);
    }

    public static Roman operator /(Roman a, Roman b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));
        
        var result = a._value / b._value;
        if (result < 1)
            throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be >= 1.");
        return new Roman(result);
    }

    public static bool operator >(Roman? a, Roman? b) => a is not null && b is not null && a._value > b._value;
    public static bool operator <(Roman? a, Roman? b) => a is not null && b is not null && a._value < b._value;
    public static bool operator >=(Roman? a, Roman? b) => a is not null && b is not null && a._value >= b._value;
    public static bool operator <=(Roman? a, Roman? b) => a is not null && b is not null && a._value <= b._value;
    public static bool operator ==(Roman? a, Roman? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._value == b._value;
    }

    public static bool operator !=(Roman? a, Roman? b) => a is not null && b is not null && !(a == b);

    #endregion
    
    #region Преобразования

    public static Roman Parse(int value) => new(value);
    public static Roman Parse(string roman) => new(roman);

    public static bool TryParse(int value)
    {
        try
        {
            Parse(value);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public static implicit operator int(Roman r) => r._value;
    public static explicit operator Roman(int value) => new(value);

    public override string ToString() => ToRoman(_value);
    public string ToString(IFormatProvider? provider) => ToRoman(_value);
    public int ToInt() => _value;

    #endregion
    
    #region Служебные методы

    private static int GetValue(char c) => c switch
    {
        'I' => 1,
        'V' => 5,
        'X' => 10,
        'L' => 50,
        'C' => 100,
        'D' => 500,
        'M' => 1000,
        _ => throw new ArgumentException($"Invalid Roman numeral character: '{c}'.")
    };
    
    private static int ToInt(string roman)
    {
        if (string.IsNullOrEmpty(roman))
            throw new ArgumentException("Roman numeral cannot be empty.");
        
        roman = roman.ToUpperInvariant();

        if (roman[0] == '-')
            throw new ArgumentOutOfRangeException(nameof(roman), "Value must be positive.");

        var result = 0;
        for (int i = roman.Length - 1, before = 0; i >= 0; i--)
        {
            var current = GetValue(roman[i]);

            result += current < before ? -current : current;
            before = current;
        }

        if (result < 1 || result > 3999)
            throw new ArgumentOutOfRangeException(nameof(roman), "Value must be between 1 and 3999.");

        return result;
    }

    private static string ToRoman(int value)
    {
        if (value < 1 || value > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 3999.");

        // Максимальная длинна римского числа (15 символов) 1 символ для безопастности
        Span<char> buffer = stackalloc char[16];
        var pos = 0;

        foreach (var (num, symbol) in Map)
        {
            while (value >= num)
            {
                foreach (var c in symbol)
                    buffer[pos++] = c;
                value -= num;
            }
        }

        return new string(buffer[..pos]);
    }

    #endregion 
    
    #region Equals / GetHashCode

    public int CompareTo(Roman? other)
    {
        if (other is null) return 1;
        return _value.CompareTo(other._value);
    }

    public bool Equals(Roman? other)
    {
        return other is not null && _value == other._value;
    }

    public override bool Equals(object? obj) =>
        obj is Roman other && _value == other._value;

    public override int GetHashCode() => _value.GetHashCode();
    
    #endregion
}
