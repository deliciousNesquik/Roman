namespace RomanNumerals;

public sealed class Roman : IComparable<Roman>, IEquatable<Roman>, IComparable<int>, IEquatable<int>
{
    /// <summary>The value of a number in the Arabic numeral system.</summary>
    private readonly int _value;

    /// <summary>
    ///     The table (value, symbol) in descending order — the single source of truth
    ///     for both directions of conversion.
    /// </summary>
    private static readonly (int Value, string Symbol)[] Map =
    [
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
        (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I")
    ];

    /// <summary>
    ///     The values of single characters, derived from <see cref="Map"/>, to avoid
    ///     duplicating numbers in the parser. Populated once during type initialization.
    /// </summary>
    private static readonly Dictionary<char, int> CharValues =
        Map.Where(entry => entry.Symbol.Length == 1)
            .ToDictionary(entry => entry.Symbol[0], entry => entry.Value);

    #region Ctors

    /// <summary>Creates a Roman numeral by its integer value (1–3999).</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
    public Roman(int value)
    {
        if (value is < 1 or > 3999)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 3999.");
        _value = value;
    }

    /// <summary>
    ///     Creates a Roman numeral from its canonical string representation.
    ///     Parsing is strict (see <see cref="RomanStyle.Strict"/>) by default:
    ///     non-canonical forms such as "IIII" are rejected. For lenient parsing,
    ///     use <see cref="Parse(string, RomanStyle)"/> with <see cref="RomanStyle.Lenient"/>.
    /// </summary>
    /// <exception cref="ArgumentException">The string is empty or contains invalid characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The value is outside the range 1–3999.</exception>
    /// <exception cref="FormatException">The string is not in canonical format.</exception>
    public Roman(string roman) : this(ParseToInt(roman, RomanStyle.Strict))
    {
    }

    /// <summary>Creates a copy of another Roman numeral.</summary>
    /// <param name="other">The Roman numeral to copy.</param>
    /// <exception cref="ArgumentNullException">Thrown if the other numeral is null.</exception>
    public Roman(Roman other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this._value = other._value;
    }

    #endregion

    #region Arithmetic and Comparison

    /// <summary>Adds two Roman numerals.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>Sum of the two Roman numerals</returns>
    /// <exception cref="ArgumentNullException">If either operand is null.</exception>
    /// <exception cref="OverflowException">If the sum exceeds 3999.</exception>
    public static Roman operator +(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var sum = (long)a._value + b._value;
        return sum > 3999 ? throw new OverflowException("Sum exceeds the maximum Roman numeral value (3999).") : new Roman((int)sum);
    }

    /// <summary>Subtracts one Roman numeral from another.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>Difference of the two Roman numerals</returns>
    /// <exception cref="ArgumentNullException">If either operand is null.</exception>
    /// <exception cref="OverflowException">If the difference is below 1.</exception>
    public static Roman operator -(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var result = a._value - b._value;
        return result < 1
            ? throw new OverflowException(
                "Difference is below the minimum Roman numeral value (1); Roman numerals cannot represent zero or negative values.")
            : new Roman(result);
    }

    /// <summary>Multiplies two Roman numerals.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>Product of the two Roman numerals</returns>
    /// <exception cref="ArgumentNullException">If either operand is null.</exception>
    /// <exception cref="OverflowException">If the product exceeds 3999.</exception>
    public static Roman operator *(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var prod = (long)a._value * b._value;
        return prod > 3999 ? throw new OverflowException("Product exceeds the maximum Roman numeral value (3999).") : new Roman((int)prod);
    }

    /// <summary>Divides one Roman numeral by another.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>Quotient of the two Roman numerals</returns>
    /// <exception cref="ArgumentNullException">If either operand is null.</exception>
    /// <exception cref="OverflowException">If the quotient is below 1.</exception>
    public static Roman operator /(Roman a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var result = a._value / b._value;
        return result < 1
            ? throw new OverflowException("Quotient is below the minimum Roman numeral value (1).")
            : new Roman(result);
    }

    /// <summary>Compares two Roman numerals for greater-than relationship.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the first numeral is greater than the second, false otherwise</returns>
    public static bool operator >(Roman? a, Roman? b)
    {
        if (a is null) return false;
        return a.CompareTo(b) > 0;
    }

    /// <summary>Compares two Roman numerals for less-than relationship.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the first numeral is less than the second, false otherwise</returns>
    public static bool operator <(Roman? a, Roman? b)
    {
        if (a is null) return b is not null;
        return a.CompareTo(b) < 0;
    }

    /// <summary>Compares two Roman numerals for greater-than-or-equal relationship.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the first numeral is greater than or equal to the second, false otherwise</returns>
    public static bool operator >=(Roman? a, Roman? b)
    {
        if (a is null) return b is null;
        return a.CompareTo(b) >= 0;
    }

    /// <summary>Compares two Roman numerals for less-than-or-equal relationship.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the first numeral is less than or equal to the second, false otherwise</returns>
    public static bool operator <=(Roman? a, Roman? b)
    {
        if (a is null) return true;
        return a.CompareTo(b) <= 0;
    }

    /// <summary>Compares two Roman numerals for equality.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the numerals are equal, false otherwise</returns>
    public static bool operator ==(Roman? a, Roman? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._value == b._value;
    }

    /// <summary>Compares two Roman numerals for inequality.</summary>
    /// <param name="a">First Roman numeral</param>
    /// <param name="b">Second Roman numeral</param>
    /// <returns>True if the numerals are not equal, false otherwise</returns>
    public static bool operator !=(Roman? a, Roman? b)
    {
        return !(a == b);
    }

    #endregion

    #region Arithmetic and Comparison with int

    // These overloads exist so that mixed Roman/int expressions bind to Roman's own range-checked
    // operators. Without them -- and with any Roman-to-int conversion in scope -- overload
    // resolution selects the predefined int operators and silently bypasses the 1-3999 guard.
    // The int operand is an Arabic quantity: only the *result* has to be representable, so an
    // out-of-range operand is legal as long as the result is not. Operands widen to long before the
    // range check so that a large int operand cannot overflow the intermediate value.

    /// <summary>Adds an integer quantity to a Roman numeral.</summary>
    /// <param name="a">The Roman numeral.</param>
    /// <param name="b">The integer quantity to add.</param>
    /// <returns>The sum as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator +(Roman a, int b)
    {
        ArgumentNullException.ThrowIfNull(a);
        return FromArithmetic((long)a._value + b, "Sum");
    }

    /// <summary>Adds a Roman numeral to an integer quantity.</summary>
    /// <param name="a">The integer quantity.</param>
    /// <param name="b">The Roman numeral.</param>
    /// <returns>The sum as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator +(int a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(b);
        return FromArithmetic((long)a + b._value, "Sum");
    }

    /// <summary>Subtracts an integer quantity from a Roman numeral.</summary>
    /// <param name="a">The Roman numeral.</param>
    /// <param name="b">The integer quantity to subtract.</param>
    /// <returns>The difference as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator -(Roman a, int b)
    {
        ArgumentNullException.ThrowIfNull(a);
        return FromArithmetic((long)a._value - b, "Difference");
    }

    /// <summary>Subtracts a Roman numeral from an integer quantity.</summary>
    /// <param name="a">The integer quantity.</param>
    /// <param name="b">The Roman numeral to subtract.</param>
    /// <returns>The difference as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator -(int a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(b);
        return FromArithmetic((long)a - b._value, "Difference");
    }

    /// <summary>Multiplies a Roman numeral by an integer quantity.</summary>
    /// <param name="a">The Roman numeral.</param>
    /// <param name="b">The integer multiplier.</param>
    /// <returns>The product as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator *(Roman a, int b)
    {
        ArgumentNullException.ThrowIfNull(a);
        return FromArithmetic((long)a._value * b, "Product");
    }

    /// <summary>Multiplies an integer quantity by a Roman numeral.</summary>
    /// <param name="a">The integer multiplier.</param>
    /// <param name="b">The Roman numeral.</param>
    /// <returns>The product as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator *(int a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(b);
        return FromArithmetic((long)a * b._value, "Product");
    }

    /// <summary>Divides a Roman numeral by an integer quantity (integer division).</summary>
    /// <param name="a">The Roman numeral.</param>
    /// <param name="b">The integer divisor.</param>
    /// <returns>The quotient as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="DivideByZeroException">If the divisor is zero.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator /(Roman a, int b)
    {
        ArgumentNullException.ThrowIfNull(a);
        return FromArithmetic(a._value / b, "Quotient");
    }

    /// <summary>Divides an integer quantity by a Roman numeral (integer division).</summary>
    /// <param name="a">The integer dividend.</param>
    /// <param name="b">The Roman numeral divisor.</param>
    /// <returns>The quotient as a Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">If the Roman operand is null.</exception>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    public static Roman operator /(int a, Roman b)
    {
        ArgumentNullException.ThrowIfNull(b);
        return FromArithmetic(a / b._value, "Quotient");
    }

    // Comparison and equality against int keep the null-sorts-lowest semantics of the all-Roman
    // operators. An int is never null, so there is no both-null case to consider.

    /// <summary>Determines whether a Roman numeral is greater than an integer value.</summary>
    public static bool operator >(Roman? a, int b)
    {
        return a is not null && a._value > b;
    }

    /// <summary>Determines whether an integer value is greater than a Roman numeral.</summary>
    public static bool operator >(int a, Roman? b)
    {
        return b is null || a > b._value;
    }

    /// <summary>Determines whether a Roman numeral is less than an integer value.</summary>
    public static bool operator <(Roman? a, int b)
    {
        return a is null || a._value < b;
    }

    /// <summary>Determines whether an integer value is less than a Roman numeral.</summary>
    public static bool operator <(int a, Roman? b)
    {
        return b is not null && a < b._value;
    }

    /// <summary>Determines whether a Roman numeral is greater than or equal to an integer value.</summary>
    public static bool operator >=(Roman? a, int b)
    {
        return a is not null && a._value >= b;
    }

    /// <summary>Determines whether an integer value is greater than or equal to a Roman numeral.</summary>
    public static bool operator >=(int a, Roman? b)
    {
        return b is null || a >= b._value;
    }

    /// <summary>Determines whether a Roman numeral is less than or equal to an integer value.</summary>
    public static bool operator <=(Roman? a, int b)
    {
        return a is null || a._value <= b;
    }

    /// <summary>Determines whether an integer value is less than or equal to a Roman numeral.</summary>
    public static bool operator <=(int a, Roman? b)
    {
        return b is not null && a <= b._value;
    }

    /// <summary>Determines whether a Roman numeral equals an integer value.</summary>
    public static bool operator ==(Roman? a, int b)
    {
        return a is not null && a._value == b;
    }

    /// <summary>Determines whether an integer value equals a Roman numeral.</summary>
    public static bool operator ==(int a, Roman? b)
    {
        return b is not null && a == b._value;
    }

    /// <summary>Determines whether a Roman numeral differs from an integer value.</summary>
    public static bool operator !=(Roman? a, int b)
    {
        return !(a == b);
    }

    /// <summary>Determines whether an integer value differs from a Roman numeral.</summary>
    public static bool operator !=(int a, Roman? b)
    {
        return !(a == b);
    }

    #endregion

    #region Parsing and Conversion

    /// <summary>Parses an integer value into a Roman numeral.</summary>
    /// <param name="value">The integer value to parse.</param>
    /// <returns>The Roman numeral representing the value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
    public static Roman Parse(int value)
    {
        return new Roman(value);
    }

    /// <summary>Parses a string into a Roman numeral using strict canonical format.</summary>
    /// <param name="roman">The string to parse.</param>
    /// <returns>The Roman numeral representing the string.</returns>
    /// <exception cref="ArgumentException">Thrown if the string is empty or contains invalid characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
    /// <exception cref="FormatException">Thrown if the string is not in canonical format.</exception>
    public static Roman Parse(string roman)
    {
        return new Roman(roman);
    }

    /// <summary>Parses the string using the specified mode (see <see cref="RomanStyle"/>).</summary>
    /// <exception cref="ArgumentException">The string is empty or contains invalid characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The value is outside the range 1–3999.</exception>
    /// <exception cref="FormatException">
    ///     In the <see cref="RomanStyle.Strict"/> mode, the string is not in canonical format.
    /// </exception>
    public static Roman Parse(string roman, RomanStyle style)
    {
        return new Roman(ParseToInt(roman, style));
    }

    /// <summary>Safe parsing of an integer value into a Roman numeral using strict canonical format.</summary>
    /// <param name="value">The integer value to parse.</param>
    /// <param name="result">When the method returns, contains the Roman numeral representing the value, or null if the parsing fails.</param>
    /// <returns>True if the parsing is successful, false otherwise.</returns>
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

    /// <summary>Safe parsing of a string into a Roman numeral using strict canonical format.</summary>
    /// <param name="roman">The string value to parse.</param>
    /// <param name="result">When the method returns, contains the Roman numeral representing the string, or null if the parsing fails.</param>
    /// <returns>True if the parsing is successful, false otherwise.</returns>
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
        catch (FormatException)
        {
            // В строгом режиме (по умолчанию) неканоническая запись бросает FormatException.
            result = null;
            return false;
        }
    }

    /// <summary>Safe parsing of a string with the specified mode (see <see cref="RomanStyle"/>).</summary>
    /// <param name="roman">The string value to parse.</param>
    /// <param name="style">The parsing style to use (see <see cref="RomanStyle"/>).</param>
    /// <param name="result">When the method returns, contains the Roman numeral representing the string, or null if the parsing fails.</param>
    /// <returns>True if the parsing is successful, false otherwise.</returns>
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

    /// <summary>
    ///     Explicitly converts a Roman numeral to its integer value. The conversion is deliberately
    ///     explicit: an implicit one would make Roman's own operators inapplicable in any mixed
    ///     Roman/int expression (int-to-Roman is explicit, so they are not candidates), letting
    ///     overload resolution fall back to the predefined int operators and silently bypass the
    ///     1–3999 range guard. Being explicit also permits the null check below, which the
    ///     Framework Design Guidelines forbid in an implicit conversion.
    /// </summary>
    /// <param name="r">The Roman numeral to convert.</param>
    /// <returns>The integer value of the Roman numeral.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the Roman numeral is null.</exception>
    public static explicit operator int(Roman r)
    {
        ArgumentNullException.ThrowIfNull(r);
        return r._value;
    }

    /// <summary>Explicitly converts an integer value to a Roman numeral.</summary>
    /// <param name="value">The integer value to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
    /// <returns>The Roman numeral representing the integer.</returns>
    public static explicit operator Roman(int value)
    {
        return new Roman(value);
    }

    /// <summary>Returns the canonical string representation of the Roman numeral.</summary>
    /// <returns>The string representing the Roman numeral.</returns>
    public override string ToString()
    {
        return ToRoman(_value);
    }

    /// <summary>Returns the integer value of the Roman numeral.</summary>
    /// <returns>The integer value of the Roman numeral.</returns>
    public int ToInt()
    {
        return _value;
    }

    #endregion

    #region Helpers

    /// <summary>
    ///     Converts the string to a canonical form for parsing: trims whitespace and
    ///     converts to uppercase. A single normalization point for all parsing paths.
    /// </summary>
    /// <exception cref="ArgumentException">The string is empty or consists of whitespace.</exception>
    private static string Normalize(string roman)
    {
        if (string.IsNullOrWhiteSpace(roman))
            throw new ArgumentException("Roman numeral cannot be empty.", nameof(roman));

        return roman.Trim().ToUpperInvariant();
    }

    /// <summary>Returns the integer value of a single Roman numeral character.</summary>
    /// <param name="c">The Roman numeral character to convert.</param>
    /// <returns>The integer value of the Roman numeral character.</returns>
    /// <exception cref="ArgumentException">Thrown if the character is not a valid Roman numeral character.</exception>
    private static int GetValue(char c)
    {
        return CharValues.TryGetValue(c, out var value)
            ? value
            : throw new ArgumentException($"Invalid Roman numeral character: '{c}'.");
    }

    /// <summary>Parses a Roman numeral string into an integer, using the specified parsing style.</summary>
    /// <param name="roman">The string to parse.</param>
    /// <param name="style">The parsing style to use (see <see cref="RomanStyle"/>).</param>
    /// <returns>The integer value of the Roman numeral.</returns>
    /// <exception cref="ArgumentException">The string is empty or consists of whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
    /// <exception cref="FormatException">Thrown if the string is not in canonical format.</exception>
    private static int ParseToInt(string roman, RomanStyle style)
    {
        var normalized = Normalize(roman);

        if (normalized.StartsWith("-"))
            throw new ArgumentOutOfRangeException(nameof(roman), "Value must be positive.");

        long result = 0;
        for (int i = normalized.Length - 1, before = 0; i >= 0; i--)
        {
            var current = GetValue(normalized[i]);

            result += current < before ? -current : current;
            before = current;
        }

        if (result is < 1 or > 3999)
            throw new ArgumentOutOfRangeException(nameof(roman), "Value must be between 1 and 3999.");

        var value = (int)result;

        if (style == RomanStyle.Strict && ToRoman(value) != normalized)
            throw new FormatException(
                $"'{roman}' is not a canonical Roman numeral; the canonical form is '{ToRoman(value)}'.");

        return value;
    }

    /// <summary>
    ///     Validates the result of an arithmetic operation against the representable range and
    ///     wraps it. Callers pass a widened <see cref="long"/> so that an operand large enough to
    ///     overflow <see cref="int"/> is still caught here rather than wrapping around silently.
    /// </summary>
    /// <param name="result">The widened arithmetic result.</param>
    /// <param name="operation">The noun naming the operation, used in the exception message.</param>
    /// <returns>The result as a Roman numeral.</returns>
    /// <exception cref="OverflowException">If the result falls outside 1–3999.</exception>
    private static Roman FromArithmetic(long result, string operation)
    {
        return result switch
        {
            > 3999 => throw new OverflowException(
                $"{operation} exceeds the maximum Roman numeral value (3999)."),
            < 1 => throw new OverflowException(
                $"{operation} is below the minimum Roman numeral value (1); Roman numerals cannot represent zero or negative values."),
            _ => new Roman((int)result)
        };
    }

    /// <summary>Converts an integer value to its canonical Roman numeral string representation.</summary>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The string representing the Roman numeral.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the range 1–3999.</exception>
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

    #region Compare, Equals and GetHashCode

    /// <summary>Compares this Roman numeral with another for ordering.</summary>
    /// <param name="other">The other Roman numeral to compare with.</param>
    /// <returns>A value indicating the relative order of the objects.</returns>
    public int CompareTo(Roman? other)
    {
        return other is null ? 1 : _value.CompareTo(other._value);
    }

    /// <summary>Compares this Roman numeral with another for equality.</summary>
    /// <param name="other">The other Roman numeral to compare with.</param>
    /// <returns>A value indicating whether the objects are equal.</returns>
    public bool Equals(Roman? other)
    {
        return other is not null && _value == other._value;
    }

    /// <summary>Compares this Roman numeral with an integer value for ordering.</summary>
    /// <param name="other">The integer value to compare with.</param>
    /// <returns>A value indicating the relative order of the two values.</returns>
    public int CompareTo(int other)
    {
        return _value.CompareTo(other);
    }

    /// <summary>
    ///     Compares this Roman numeral with an integer value for equality. Present so that
    ///     <c>roman.Equals(42)</c> agrees with <c>roman == 42</c>; the <see cref="Equals(object?)"/>
    ///     overload still returns false for a boxed int, matching how the framework's own numeric
    ///     types behave.
    /// </summary>
    /// <param name="other">The integer value to compare with.</param>
    /// <returns>A value indicating whether the two values are equal.</returns>
    public bool Equals(int other)
    {
        return _value == other;
    }

    /// <summary>Compares this Roman numeral with another object for equality.</summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>A value indicating whether the objects are equal.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Roman other && _value == other._value;
    }

    /// <summary>Returns a hash code for this Roman numeral.</summary>
    /// <returns>A hash code for the object.</returns>
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    #endregion
}