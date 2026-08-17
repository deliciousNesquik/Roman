namespace RomanNumerals;

/// <summary>Roman numeral string parsing mode.</summary>
public enum RomanStyle
{
    /// <summary>
    ///     Strict parsing: only accepts canonical notation
    ///     (for example, "IV" but not "IIII"). Default behavior.
    /// </summary>
    Strict = 0,

    /// <summary>
    ///     Lenient parsing: accepts non-canonical forms (for example, "IIII" = 4).
    /// </summary>
    Lenient
}
