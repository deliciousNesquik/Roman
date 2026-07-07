namespace RomanNumerals;

/// <summary>Режим разбора строкового римского числа.</summary>
public enum RomanStyle
{
    /// <summary>
    ///     Строгий разбор: принимает только каноническую запись
    ///     (например, "IV", но не "IIII"). Поведение по умолчанию.
    /// </summary>
    Strict = 0,

    /// <summary>
    ///     Лояльный разбор: принимает неканонические формы (например, "IIII" = 4).
    /// </summary>
    Lenient
}
