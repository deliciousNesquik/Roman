namespace RomanNumerals;

public sealed class Roman : IComparable<Roman>, IEquatable<Roman>
{
    /// <summary>Значение числа в арабской системе счисления.</summary>
    private readonly int _value;

    /// <summary>
    ///     Таблица (значение, символ) по убыванию — единственный источник истины
    ///     для обоих направлений преобразования.
    /// </summary>
    private static readonly (int Value, string Symbol)[] Map =
    [
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
        (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I")
    ];

    /// <summary>
    ///     Значения одиночных символов, выведенные из <see cref="Map"/>, чтобы не
    ///     дублировать числа в парсере. Заполняется один раз при инициализации типа.
    /// </summary>
    private static readonly Dictionary<char, int> CharValues =
        Map.Where(entry => entry.Symbol.Length == 1)
            .ToDictionary(entry => entry.Symbol[0], entry => entry.Value);

    #region Конструкторы

    /// <summary>Создаёт римское число по целому значению (1–3999).</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Выбрасывается, если значение выходит за диапазон 1–3999.
    /// </exception>
    public Roman(int value)
    {
        if (value is < 1 or > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 3999.");
        _value = value;
    }

    /// <summary>Создаёт римское число из строкового представления.</summary>
    public Roman(string roman) : this(ToInt(roman))
    {
    }

    /// <summary>Создаёт копию другого римского числа.</summary>
    public Roman(Roman other) : this((other ?? throw new ArgumentNullException(nameof(other)))._value)
    {
    }

    #endregion

    #region Арифметика и сравнение

    public static Roman operator +(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var sum = (long)a._value + b._value;
        return sum > 3999 ? throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be ≤ 3999.") : new Roman((int)sum);
    }

    public static Roman operator -(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var result = a._value - b._value;
        return result < 1
            ? throw new ArgumentOutOfRangeException(nameof(b),
                "Roman numerals cannot represent zero or negative values.")
            : new Roman(result);
    }

    public static Roman operator *(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var prod = (long)a._value * b._value;
        return prod > 3999 ? throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be ≤ 3999.") : new Roman((int)prod);
    }

    public static Roman operator /(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var result = a._value / b._value;
        return result < 1
            ? throw new ArgumentOutOfRangeException(nameof(b), "Resulting value must be >= 1.")
            : new Roman(result);
    }

    public static bool operator >(Roman? a, Roman? b)
    {
        if (a is null) return false;
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(Roman? a, Roman? b)
    {
        if (a is null) return b is not null;
        return a.CompareTo(b) < 0;
    }

    public static bool operator >=(Roman? a, Roman? b)
    {
        if (a is null) return b is null;
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(Roman? a, Roman? b)
    {
        if (a is null) return true;
        return a.CompareTo(b) <= 0;
    }

    public static bool operator ==(Roman? a, Roman? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._value == b._value;
    }

    public static bool operator !=(Roman? a, Roman? b)
    {
        return !(a == b);
    }

    #endregion

    #region Преобразования

    public static Roman Parse(int value)
    {
        return new Roman(value);
    }

    public static Roman Parse(string roman)
    {
        return new Roman(roman);
    }

    /// <summary>Разбирает строку с указанным режимом (см. <see cref="RomanStyle"/>).</summary>
    /// <exception cref="ArgumentException">Строка пуста или содержит недопустимые символы.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Значение выходит за диапазон 1–3999.</exception>
    /// <exception cref="FormatException">
    ///     В режиме <see cref="RomanStyle.Strict"/> запись неканонична.
    /// </exception>
    public static Roman Parse(string roman, RomanStyle style)
    {
        var result = new Roman(roman);

        if (style == RomanStyle.Strict)
        {
            var normalized = roman.Trim().ToUpperInvariant();
            if (result.ToString() != normalized)
                throw new FormatException(
                    $"'{roman}' is not a canonical Roman numeral; the canonical form is '{result}'.");
        }

        return result;
    }

    public static bool TryParse(int value, out Roman? result)
    {
        try
        {
            result = new Roman(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = null;
            return false;
        }
    }

    public static bool TryParse(string roman, out Roman? result)
    {
        try
        {
            result = new Roman(roman);
            return true;
        }
        catch (ArgumentException)
        {
            // ArgumentOutOfRangeException наследует ArgumentException — один блок ловит оба.
            result = null;
            return false;
        }
    }

    /// <summary>Безопасный разбор строки с указанным режимом (см. <see cref="RomanStyle"/>).</summary>
    public static bool TryParse(string roman, RomanStyle style, out Roman? result)
    {
        try
        {
            result = Parse(roman, style);
            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }

    public static implicit operator int(Roman r)
    {
        ArgumentNullException.ThrowIfNull(r);
        return r._value;
    }

    public static explicit operator Roman(int value)
    {
        return new Roman(value);
    }

    public override string ToString()
    {
        return ToRoman(_value);
    }

    public int ToInt()
    {
        return _value;
    }

    #endregion

    #region Служебные методы

    private static int GetValue(char c)
    {
        return CharValues.TryGetValue(c, out var value)
            ? value
            : throw new ArgumentException($"Invalid Roman numeral character: '{c}'.");
    }

    private static int ToInt(string roman)
    {
        if (string.IsNullOrWhiteSpace(roman))
            throw new ArgumentException("Roman numeral cannot be empty.", nameof(roman));

        roman = roman.Trim().ToUpperInvariant();

        if (roman.StartsWith("-"))
            throw new ArgumentOutOfRangeException(nameof(roman), "Value must be positive.");

        long result = 0;
        for (int i = roman.Length - 1, before = 0; i >= 0; i--)
        {
            var current = GetValue(roman[i]);

            result += current < before ? -current : current;
            before = current;
        }

        return result is < 1 or > 3999 ? throw new ArgumentOutOfRangeException(nameof(roman), "Value must be between 1 and 3999.") : (int)result;
    }

    private static string ToRoman(int value)
    {
        if (value is < 1 or > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 3999.");

        // Максимальная длина римского числа (15 символов) 1 символ для безопасности
        Span<char> buffer = stackalloc char[16];

        var pos = 0;
        foreach (var (num, symbol) in Map)
            while (value >= num)
            {
                foreach (var c in symbol)
                    buffer[pos++] = c;
                value -= num;
            }

        return new string(buffer[..pos]);
    }

    #endregion

    #region Equals / GetHashCode

    public int CompareTo(Roman? other)
    {
        return other is null ? 1 : _value.CompareTo(other._value);
    }

    public bool Equals(Roman? other)
    {
        return other is not null && _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Roman other && _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    #endregion
}