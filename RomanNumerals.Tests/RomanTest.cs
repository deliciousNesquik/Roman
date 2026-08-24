using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RomanNumerals.Tests;

[TestClass]
[TestSubject(typeof(Roman))]
public class RomanTest
{
    #region Tests Ctor(int)

    [TestMethod]
    public void Ctor_Int_LowerBoundary_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman(0));
        Assert.AreEqual("value", ex.ParamName);
        StringAssert.Contains(ex.Message, "between 1 and 3999");
    }

    [TestMethod]
    public void Ctor_Int_UpperBoundary_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman(4000));
        Assert.AreEqual("value", ex.ParamName);
        StringAssert.Contains(ex.Message, "between 1 and 3999");
    }

    [TestMethod]
    public void Ctor_Int_NegativeValue_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman(-5));
        Assert.AreEqual("value", ex.ParamName);
    }

    [TestMethod]
    [DataRow(1, "I")]
    [DataRow(4, "IV")]
    [DataRow(5, "V")]
    [DataRow(9, "IX")]
    [DataRow(10, "X")]
    [DataRow(40, "XL")]
    [DataRow(50, "L")]
    [DataRow(90, "XC")]
    [DataRow(100, "C")]
    [DataRow(400, "CD")]
    [DataRow(500, "D")]
    [DataRow(900, "CM")]
    [DataRow(1000, "M")]
    [DataRow(1984, "MCMLXXXIV")]
    [DataRow(3999, "MMMCMXCIX")]
    public void Ctor_Int_ValidValue_CreatesCorrectRoman(int value, string expected)
    {
        var roman = new Roman(value);
        Assert.AreEqual(expected, roman.ToString());
    }

    #endregion

    #region Tests Ctor(string)

    [TestMethod]
    public void Ctor_String_EmptyString_ThrowsArgumentException()
    {
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new Roman(""));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "cannot be empty");
    }

    [TestMethod]
    public void Ctor_String_Null_ThrowsArgumentNullException()
    {
        // Отсутствующий аргумент — не то же самое, что негодное значение.
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new Roman((string)null));
        Assert.AreEqual("roman", ex.ParamName);
    }

    [TestMethod]
    public void Ctor_String_NullAndEmpty_ReportDifferentProblems()
    {
        // string.IsNullOrWhiteSpace схлопывает null и пустоту в один случай, из-за чего
        // null-ссылка сообщается как «cannot be empty» с типом ArgumentException.
        Assert.ThrowsExactly<ArgumentNullException>(() => new Roman((string)null));
        Assert.ThrowsExactly<ArgumentException>(() => new Roman(""));
        Assert.ThrowsExactly<ArgumentException>(() => new Roman("   "));
    }

    [TestMethod]
    public void Parse_String_Null_ThrowsArgumentNullException()
    {
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => Roman.Parse((string)null));
        Assert.AreEqual("roman", ex.ParamName);
    }

    [TestMethod]
    [DataRow(RomanStyle.Strict)]
    [DataRow(RomanStyle.Lenient)]
    public void Parse_StringWithStyle_Null_ThrowsArgumentNullException(RomanStyle style)
    {
        // Режим разбора к null-ссылке отношения не имеет: отказ одинаков для обоих.
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => Roman.Parse(null, style));
        Assert.AreEqual("roman", ex.ParamName);
    }

    [TestMethod]
    public void Ctor_String_WhitespaceOnly_ThrowsArgumentException()
    {
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new Roman("   "));
        Assert.AreEqual("roman", ex.ParamName);
    }

    [TestMethod]
    public void Ctor_String_InvalidCharacter_ThrowsArgumentException()
    {
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new Roman("ABC"));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "Invalid Roman numeral character");
    }

    [TestMethod]
    public void Ctor_String_NegativeValue_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman("-X"));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "positive");
    }

    [TestMethod]
    [DataRow("\u200B-X")] // zero-width space, then hyphen
    [DataRow("\u00AD-X")] // soft hyphen, then hyphen
    public void Ctor_String_IgnorableCharBeforeHyphen_ReportsInvalidCharacter(string roman)
    {
        // ICU считает U+200B и U+00AD игнорируемыми при сравнении, поэтому культуро-зависимый
        // StartsWith("-") отвечает true для строки, которая на '-' не начинается, и настоящая
        // причина отказа — недопустимый символ, а не знак — подменяется на «Value must be
        // positive». Ordinal-проверка одного символа такой подмены не делает.
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new Roman(roman));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "Invalid Roman numeral character");
    }

    [TestMethod]
    public void Ctor_String_ExceedsUpperLimit_ThrowsArgumentOutOfRangeException()
    {
        // MMMM = 4000
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman("MMMM"));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "between 1 and 3999");
    }

    [TestMethod]
    [DataRow("I", 1)]
    [DataRow("i", 1)]
    [DataRow("  V  ", 5)]
    [DataRow("IV", 4)]
    [DataRow("IX", 9)]
    [DataRow("XL", 40)]
    [DataRow("XC", 90)]
    [DataRow("CD", 400)]
    [DataRow("CM", 900)]
    [DataRow("MCMLXXXIV", 1984)]
    [DataRow("MMMCMXCIX", 3999)]
    public void Ctor_String_ValidRoman_ParsesCorrectly(string roman, int expected)
    {
        var r = new Roman(roman);
        Assert.AreEqual(expected, r.ToInt());
    }

    [TestMethod]
    public void Ctor_String_OverflowsIntAccumulator_ThrowsArgumentOutOfRangeException()
    {
        // 4 294 968 символов 'M' => 1000 * 4 294 968 = 4 294 968 000.
        // При накоплении в int значение заворачивалось по модулю 2^32 в 704
        // и проходило финальную проверку диапазона, молча возвращая неверный
        // результат. С аккумулятором long переполнения нет — должно бросать.
        var garbage = new string('M', 4_294_968);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Roman(garbage));
        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "between 1 and 3999");
    }

    #endregion

    #region Tests Ctor(Roman)

    [TestMethod]
    public void Ctor_Roman_Null_ThrowsArgumentNullException()
    {
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new Roman((Roman)null));
        Assert.AreEqual("other", ex.ParamName);
    }

    [TestMethod]
    public void Ctor_Roman_ValidRoman_CreatesCopy()
    {
        var original = new Roman(42);
        var copy = new Roman(original);

        Assert.AreEqual(original.ToInt(), copy.ToInt());
        Assert.AreEqual(original, copy);
        Assert.IsFalse(ReferenceEquals(original, copy));
    }

    #endregion

    #region Tests Arithmetic: Addition

    [TestMethod]
    public void Addition_ValidValues_ReturnsCorrectSum()
    {
        var a = new Roman(10);
        var b = new Roman(5);
        var result = a + b;

        Assert.AreEqual(15, result.ToInt());
        Assert.AreEqual("XV", result.ToString());
    }

    [TestMethod]
    public void Addition_ArgumentANull_ThrowsArgumentNullException()
    {
        Roman a = null;
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a + b);
        Assert.AreEqual("a", ex.ParamName);
    }

    [TestMethod]
    public void Addition_ArgumentBNull_ThrowsArgumentNullException()
    {
        var a = new Roman(5);
        Roman b = null;

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a + b);
        Assert.AreEqual("b", ex.ParamName);
    }

    [TestMethod]
    public void Addition_ExceedsUpperLimit_ThrowsOverflowException()
    {
        var a = new Roman(3999);
        var b = new Roman(1);

        var ex = Assert.ThrowsExactly<OverflowException>(() => a + b);
        StringAssert.Contains(ex.Message, "3999");
    }

    #endregion

    #region Tests Arithmetic: Subtraction

    [TestMethod]
    public void Subtraction_ValidValues_ReturnsCorrectDifference()
    {
        var a = new Roman(10);
        var b = new Roman(3);
        var result = a - b;

        Assert.AreEqual(7, result.ToInt());
        Assert.AreEqual("VII", result.ToString());
    }

    [TestMethod]
    public void Subtraction_ArgumentANull_ThrowsArgumentNullException()
    {
        Roman a = null;
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a - b);
        Assert.AreEqual("a", ex.ParamName);
    }

    [TestMethod]
    public void Subtraction_ArgumentBNull_ThrowsArgumentNullException()
    {
        var a = new Roman(5);
        Roman b = null;

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a - b);
        Assert.AreEqual("b", ex.ParamName);
    }

    [TestMethod]
    public void Subtraction_ResultZero_ThrowsOverflowException()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<OverflowException>(() => a - b);
        StringAssert.Contains(ex.Message, "zero or negative");
    }

    [TestMethod]
    public void Subtraction_ResultNegative_ThrowsOverflowException()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.ThrowsExactly<OverflowException>(() => a - b);
    }

    #endregion

    #region Tests Arithmetic: Multiplication

    [TestMethod]
    public void Multiplication_ValidValues_ReturnsCorrectProduct()
    {
        var a = new Roman(7);
        var b = new Roman(8);
        var result = a * b;

        Assert.AreEqual(56, result.ToInt());
        Assert.AreEqual("LVI", result.ToString());
    }

    [TestMethod]
    public void Multiplication_ArgumentANull_ThrowsArgumentNullException()
    {
        Roman a = null;
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a * b);
        Assert.AreEqual("a", ex.ParamName);
    }

    [TestMethod]
    public void Multiplication_ArgumentBNull_ThrowsArgumentNullException()
    {
        var a = new Roman(5);
        Roman b = null;

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a * b);
        Assert.AreEqual("b", ex.ParamName);
    }

    [TestMethod]
    public void Multiplication_ExceedsUpperLimit_ThrowsOverflowException()
    {
        var a = new Roman(2000);
        var b = new Roman(3);

        var ex = Assert.ThrowsExactly<OverflowException>(() => a * b);
        StringAssert.Contains(ex.Message, "3999");
    }

    #endregion

    #region Tests Arithmetic: Division

    [TestMethod]
    public void Division_ValidValues_ReturnsCorrectQuotient()
    {
        var a = new Roman(20);
        var b = new Roman(4);
        var result = a / b;

        Assert.AreEqual(5, result.ToInt());
        Assert.AreEqual("V", result.ToString());
    }

    [TestMethod]
    public void Division_IntegerDivision_TruncatesResult()
    {
        var a = new Roman(10);
        var b = new Roman(3);
        var result = a / b;

        Assert.AreEqual(3, result.ToInt());
    }

    [TestMethod]
    public void Division_ArgumentANull_ThrowsArgumentNullException()
    {
        Roman a = null;
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a / b);
        Assert.AreEqual("a", ex.ParamName);
    }

    [TestMethod]
    public void Division_ArgumentBNull_ThrowsArgumentNullException()
    {
        var a = new Roman(5);
        Roman b = null;

        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => a / b);
        Assert.AreEqual("b", ex.ParamName);
    }

    [TestMethod]
    public void Division_ResultLessThanOne_ThrowsOverflowException()
    {
        var a = new Roman(1);
        var b = new Roman(2);

        var ex = Assert.ThrowsExactly<OverflowException>(() => a / b);
        StringAssert.Contains(ex.Message, "minimum");
    }

    #endregion

    #region Tests Comparison Operators

    [TestMethod]
    public void GreaterThan_FirstLarger_ReturnsTrue()
    {
        var a = new Roman(10);
        var b = new Roman(5);

        Assert.IsTrue(a > b);
        Assert.IsFalse(b > a);
    }

    [TestMethod]
    public void GreaterThan_Equal_ReturnsFalse()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.IsFalse(a > b);
    }

    [TestMethod]
    public void GreaterThan_NullOperands_NullSortsLowest()
    {
        var value = new Roman(5);
        Roman firstNull = null;
        Roman secondNull = null;

        Assert.IsTrue(value > firstNull, "a value is greater than null");
        Assert.IsFalse(firstNull > value, "null is not greater than a value");
        Assert.IsFalse(firstNull > secondNull, "null is not greater than null");
    }

    [TestMethod]
    public void LessThan_FirstSmaller_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.IsTrue(a < b);
        Assert.IsFalse(b < a);
    }

    [TestMethod]
    public void LessThan_NullOperands_NullSortsLowest()
    {
        var value = new Roman(5);
        Roman firstNull = null;
        Roman secondNull = null;

        Assert.IsTrue(firstNull < value, "null is less than a value");
        Assert.IsFalse(value < firstNull, "a value is not less than null");
        Assert.IsFalse(firstNull < secondNull, "null is not less than null");
    }

    [TestMethod]
    public void GreaterOrEqual_Greater_ReturnsTrue()
    {
        var a = new Roman(10);
        var b = new Roman(5);

        Assert.IsTrue(a >= b);
    }

    [TestMethod]
    public void GreaterOrEqual_Equal_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.IsTrue(a >= b);
    }

    [TestMethod]
    public void GreaterOrEqual_NullOperands_NullSortsLowest()
    {
        var value = new Roman(5);
        Roman firstNull = null;
        Roman secondNull = null;

        Assert.IsTrue(value >= firstNull, "a value is greater than or equal to null");
        Assert.IsFalse(firstNull >= value, "null is not greater than or equal to a value");
        Assert.IsTrue(firstNull >= secondNull, "null is greater than or equal to null");
    }

    [TestMethod]
    public void LessOrEqual_Smaller_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.IsTrue(a <= b);
    }

    [TestMethod]
    public void LessOrEqual_Equal_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.IsTrue(a <= b);
    }

    [TestMethod]
    public void LessOrEqual_NullOperands_NullSortsLowest()
    {
        var value = new Roman(5);
        Roman firstNull = null;
        Roman secondNull = null;

        Assert.IsTrue(firstNull <= value, "null is less than or equal to a value");
        Assert.IsFalse(value <= firstNull, "a value is not less than or equal to null");
        Assert.IsTrue(firstNull <= secondNull, "null is less than or equal to null");
    }

    #endregion

    #region Tests Equality Operators

    [TestMethod]
    public void Equals_Operator_SameValue_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void Equals_Operator_DifferentValue_ReturnsFalse()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.IsFalse(a == b);
    }

    [TestMethod]
    public void Equals_Operator_SameReference_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = a;

        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void Equals_Operator_BothNull_ReturnsTrue()
    {
        Roman a = null;
        Roman b = null;

        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void Equals_Operator_OneNull_ReturnsFalse()
    {
        var a = new Roman(5);
        Roman b = null;

        Assert.IsFalse(a == b);
        Assert.IsFalse(b == a);
    }

    [TestMethod]
    public void NotEquals_Operator_DifferentValue_ReturnsTrue()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.IsTrue(a != b);
    }

    [TestMethod]
    public void NotEquals_Operator_SameValue_ReturnsFalse()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void NotEquals_Operator_OneNull_ReturnsTrue()
    {
        var a = new Roman(5);
        Roman b = null;

        Assert.IsTrue(a != b);
        Assert.IsTrue(b != a);
    }

    [TestMethod]
    public void NotEquals_Operator_BothNull_ReturnsFalse()
    {
        Roman a = null;
        Roman b = null;

        Assert.IsFalse(a != b);
    }

    #endregion

    #region Tests Equals / GetHashCode

    [TestMethod]
    public void Equals_Method_SameValue_ReturnsTrue()
    {
        var a = new Roman(42);
        var b = new Roman(42);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b.Equals(a));
    }

    [TestMethod]
    public void Equals_Method_DifferentValue_ReturnsFalse()
    {
        var a = new Roman(42);
        var b = new Roman(24);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_Method_Null_ReturnsFalse()
    {
        var a = new Roman(42);

        Assert.IsFalse(a.Equals(null));
    }

    [TestMethod]
    public void Equals_Object_SameValue_ReturnsTrue()
    {
        var a = new Roman(42);
        object b = new Roman(42);

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        var a = new Roman(42);
        object b = 42;

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        var a = new Roman(42);
        var b = new Roman(42);

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    #region Tests CompareTo

    [TestMethod]
    public void CompareTo_Smaller_ReturnsNegative()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        Assert.IsTrue(a.CompareTo(b) < 0);
    }

    [TestMethod]
    public void CompareTo_Greater_ReturnsPositive()
    {
        var a = new Roman(10);
        var b = new Roman(5);

        Assert.IsTrue(a.CompareTo(b) > 0);
    }

    [TestMethod]
    public void CompareTo_Equal_ReturnsZero()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        Assert.AreEqual(0, a.CompareTo(b));
    }

    [TestMethod]
    public void CompareTo_Null_ReturnsPositive()
    {
        var a = new Roman(5);

        Assert.IsTrue(a.CompareTo(null) > 0);
    }

    #endregion

    #region Tests Parse

    [TestMethod]
    public void Parse_Int_ValidValue_ReturnsRoman()
    {
        var result = Roman.Parse(42);

        Assert.AreEqual(42, result.ToInt());
        Assert.AreEqual("XLII", result.ToString());
    }

    [TestMethod]
    public void Parse_Int_InvalidValue_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Roman.Parse(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Roman.Parse(4000));
    }

    [TestMethod]
    public void Parse_String_ValidValue_ReturnsRoman()
    {
        var result = Roman.Parse("XLII");

        Assert.AreEqual(42, result.ToInt());
    }

    [TestMethod]
    public void Parse_String_InvalidValue_ThrowsException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => Roman.Parse(""));
        Assert.ThrowsExactly<ArgumentException>(() => Roman.Parse("ABC"));
    }

    #endregion

    #region Tests TryParse

    [TestMethod]
    public void TryParse_Int_ValidValue_ReturnsTrue()
    {
        var success = Roman.TryParse(42, out var result);

        Assert.IsTrue(success);
        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.ToInt());
    }

    [TestMethod]
    public void TryParse_Int_InvalidValue_ReturnsFalse()
    {
        var success = Roman.TryParse(0, out var result);

        Assert.IsFalse(success);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryParse_String_ValidValue_ReturnsTrue()
    {
        var success = Roman.TryParse("XLII", out var result);

        Assert.IsTrue(success);
        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.ToInt());
    }

    [TestMethod]
    public void TryParse_String_InvalidValue_ReturnsFalse()
    {
        Assert.IsFalse(Roman.TryParse("", out _));
        Assert.IsFalse(Roman.TryParse("ABC", out _));
        Assert.IsFalse(Roman.TryParse("MMMM", out _));
    }

    [TestMethod]
    public void TryParse_String_Null_ReturnsFalse()
    {
        var success = Roman.TryParse(null, out var result);

        Assert.IsFalse(success);
        Assert.IsNull(result);
    }

    #endregion

    #region Tests RomanStyle (Lenient / Strict)

    [TestMethod]
    [DataRow("IV", 4)]
    [DataRow("MCMLXXXIV", 1984)]
    [DataRow("  xlii  ", 42)]
    [DataRow("MMMCMXCIX", 3999)]
    public void Parse_Strict_CanonicalForm_ReturnsRoman(string roman, int expected)
    {
        var result = Roman.Parse(roman, RomanStyle.Strict);

        Assert.AreEqual(expected, result.ToInt());
    }

    [TestMethod]
    [DataRow("IIII")]
    [DataRow("VX")]
    [DataRow("IM")]
    [DataRow("XXXX")]
    public void Parse_Strict_NonCanonicalForm_ThrowsFormatException(string roman)
    {
        var ex = Assert.ThrowsExactly<FormatException>(() => Roman.Parse(roman, RomanStyle.Strict));
        StringAssert.Contains(ex.Message, "canonical");
    }

    [TestMethod]
    [DataRow("IIII", 4)]
    [DataRow("VX", 5)]
    public void Parse_Lenient_NonCanonicalForm_Parses(string roman, int expected)
    {
        // Явный Lenient по-прежнему принимает неканонические формы.
        var result = Roman.Parse(roman, RomanStyle.Lenient);

        Assert.AreEqual(expected, result.ToInt());
    }

    [TestMethod]
    [DataRow("IIII")]
    [DataRow("VX")]
    [DataRow("XXXX")]
    public void Parse_Default_NonCanonicalForm_ThrowsFormatException(string roman)
    {
        // По умолчанию разбор строгий: неканоническая запись отвергается.
        var ex = Assert.ThrowsExactly<FormatException>(() => Roman.Parse(roman));
        StringAssert.Contains(ex.Message, "canonical");
    }

    [TestMethod]
    [DataRow("IIII")]
    [DataRow("VX")]
    public void Ctor_String_DefaultStrict_NonCanonicalForm_ThrowsFormatException(string roman)
    {
        // Конструктор строки использует строгий разбор по умолчанию.
        var ex = Assert.ThrowsExactly<FormatException>(() => new Roman(roman));
        StringAssert.Contains(ex.Message, "canonical");
    }

    [TestMethod]
    public void TryParse_String_DefaultStrict_NonCanonicalForm_ReturnsFalse()
    {
        // TryParse без указания режима также строгий и не пробрасывает FormatException.
        var success = Roman.TryParse("IIII", out var result);

        Assert.IsFalse(success);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Parse_Strict_InvalidInput_ThrowsSameAsLenient()
    {
        // Строгий режим не «глотает» базовые ошибки разбора.
        Assert.ThrowsExactly<ArgumentException>(() => Roman.Parse("ABC", RomanStyle.Strict));
        Assert.ThrowsExactly<ArgumentException>(() => Roman.Parse("", RomanStyle.Strict));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Roman.Parse("MMMM", RomanStyle.Strict));
    }

    [TestMethod]
    public void TryParse_Strict_CanonicalForm_ReturnsTrue()
    {
        var success = Roman.TryParse("IV", RomanStyle.Strict, out var result);

        Assert.IsTrue(success);
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.ToInt());
    }

    [TestMethod]
    public void TryParse_Strict_NonCanonicalForm_ReturnsFalse()
    {
        var success = Roman.TryParse("IIII", RomanStyle.Strict, out var result);

        Assert.IsFalse(success);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryParse_Lenient_NonCanonicalForm_ReturnsTrue()
    {
        var success = Roman.TryParse("IIII", RomanStyle.Lenient, out var result);

        Assert.IsTrue(success);
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.ToInt());
    }

    [TestMethod]
    [DataRow("IIX", 8)]
    [DataRow("IIIX", 7)]
    [DataRow("XIIX", 18)]
    [DataRow("IIXX", 18)]
    [DataRow("XXIIX", 28)]
    [DataRow("IIC", 98)]
    public void Parse_Lenient_MultiCharacterSubtractiveRun_SubtractsEveryLesserSymbol(string roman, int expected)
    {
        // Каждый символ, меньший максимума справа от него, должен вычитаться. Сравнение только
        // с непосредственным правым соседом даёт для "IIX" 10, а для "XIIX" — 20: второй I
        // прибавляется, потому что его сосед справа тоже I. Формы IIX/XIIX/IIC засвидетельствованы
        // в надписях и означают 8, 18 и 98.
        var result = Roman.Parse(roman, RomanStyle.Lenient);

        Assert.AreEqual(expected, result.ToInt());
    }

    [TestMethod]
    public void TryParse_Lenient_MultiCharacterSubtractiveRun_ReportsCorrectValue()
    {
        var success = Roman.TryParse("IIX", RomanStyle.Lenient, out var result);

        Assert.IsTrue(success);
        Assert.IsNotNull(result);
        Assert.AreEqual(8, result.ToInt());
    }

    [TestMethod]
    public void Parse_Lenient_SubtractiveRunReachingZero_ThrowsArgumentOutOfRangeException()
    {
        // Десять I перед X дают 0 — непредставимо, поэтому это должен быть отказ, а не
        // положительное число, полученное сложением «лишних» единиц.
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Roman.Parse("IIIIIIIIIIX", RomanStyle.Lenient));

        Assert.AreEqual("roman", ex.ParamName);
        StringAssert.Contains(ex.Message, "between 1 and 3999");
    }

    [TestMethod]
    [DataRow("IIII", 4)]
    [DataRow("VIIII", 9)]
    [DataRow("XXXX", 40)]
    [DataRow("IIIIIIIIII", 10)]
    [DataRow("VX", 5)]
    [DataRow("MCMXCIV", 1994)]
    [DataRow("MMMCMXCIX", 3999)]
    [DataRow("XIX", 19)]
    [DataRow("XLII", 42)]
    public void Parse_Lenient_AdditiveAndCanonicalForms_Unchanged(string roman, int expected)
    {
        // Страховка от регрессии: в канонической записи символ, меньший максимума справа,
        // всегда стоит непосредственно перед этим максимумом, поэтому оба правила совпадают.
        var result = Roman.Parse(roman, RomanStyle.Lenient);

        Assert.AreEqual(expected, result.ToInt());
    }

    [TestMethod]
    [DataRow(7)]
    [DataRow(2)]
    [DataRow(-1)]
    [DataRow(int.MaxValue)]
    public void Parse_UndefinedStyle_ThrowsArgumentOutOfRangeException(int raw)
    {
        // C# не проверяет enum-аргументы, поэтому в метод может прийти любое int-значение.
        // Сравнение `style == Strict` при таком значении ложно, и каноническая проверка
        // пропускается целиком — мягкий разбор без запроса мягкого разбора.
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Roman.Parse("IIII", (RomanStyle)raw));

        Assert.AreEqual("style", ex.ParamName);
    }

    [TestMethod]
    public void Parse_UndefinedStyle_CanonicalInput_StillThrows()
    {
        // Отказ относится к аргументу, а не к входной строке: канонический вход не должен
        // маскировать мусорный режим.
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Roman.Parse("IV", (RomanStyle)7));

        Assert.AreEqual("style", ex.ParamName);
    }

    [TestMethod]
    public void TryParse_UndefinedStyle_ThrowsArgumentOutOfRangeException()
    {
        // `int.TryParse` на невалидном NumberStyles бросает, а не возвращает false: негодный
        // аргумент — ошибка программиста, а не сбой разбора, от которого вызывающий оправится.
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Roman.TryParse("IIII", (RomanStyle)7, out _));

        Assert.AreEqual("style", ex.ParamName);
    }

    [TestMethod]
    public void Parse_DefaultStyleValue_IsStrict()
    {
        // Страховка: `default(RomanStyle)` — это Strict (0), определённое значение, поэтому
        // валидация режима его не отвергает, а разбор остаётся строгим.
        Assert.ThrowsExactly<FormatException>(() => Roman.Parse("IIII", default));
    }

    [TestMethod]
    [DataRow("IIX")]
    [DataRow("XIIX")]
    [DataRow("IIC")]
    public void Parse_Strict_MultiCharacterSubtractiveRun_ThrowsFormatException(string roman)
    {
        // Строгий режим не затронут: каноническая перепроверка отвергает такую запись
        // независимо от того, какое значение вычислил разбор.
        var ex = Assert.ThrowsExactly<FormatException>(() => Roman.Parse(roman, RomanStyle.Strict));
        StringAssert.Contains(ex.Message, "canonical");
    }

    #endregion

    #region Tests Conversions

    [TestMethod]
    public void ExplicitConversion_ToInt_ReturnsCorrectValue()
    {
        var roman = new Roman(42);
        var value = (int)roman;

        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public void ExplicitConversion_FromInt_CreatesRoman()
    {
        var roman = (Roman)42;

        Assert.AreEqual(42, roman.ToInt());
        Assert.AreEqual("XLII", roman.ToString());
    }

    [TestMethod]
    public void ExplicitConversion_FromInt_InvalidValue_ThrowsException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => (Roman)0);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => (Roman)4000);
    }

    [TestMethod]
    public void ToInt_Method_ReturnsCorrectValue()
    {
        var roman = new Roman(42);

        Assert.AreEqual(42, roman.ToInt());
    }

    #endregion

    #region Tests ToString

    [TestMethod]
    [DataRow(1, "I")]
    [DataRow(2, "II")]
    [DataRow(3, "III")]
    [DataRow(4, "IV")]
    [DataRow(5, "V")]
    [DataRow(6, "VI")]
    [DataRow(7, "VII")]
    [DataRow(8, "VIII")]
    [DataRow(9, "IX")]
    [DataRow(10, "X")]
    [DataRow(11, "XI")]
    [DataRow(14, "XIV")]
    [DataRow(19, "XIX")]
    [DataRow(20, "XX")]
    [DataRow(27, "XXVII")]
    [DataRow(40, "XL")]
    [DataRow(44, "XLIV")]
    [DataRow(49, "XLIX")]
    [DataRow(50, "L")]
    [DataRow(90, "XC")]
    [DataRow(99, "XCIX")]
    [DataRow(100, "C")]
    [DataRow(400, "CD")]
    [DataRow(444, "CDXLIV")]
    [DataRow(500, "D")]
    [DataRow(900, "CM")]
    [DataRow(999, "CMXCIX")]
    [DataRow(1000, "M")]
    [DataRow(1994, "MCMXCIV")]
    [DataRow(2023, "MMXXIII")]
    [DataRow(3888, "MMMDCCCLXXXVIII")]
    [DataRow(3999, "MMMCMXCIX")]
    public void ToString_VariousValues_ReturnsCorrectRomanNumeral(int value, string expected)
    {
        var roman = new Roman(value);

        Assert.AreEqual(expected, roman.ToString());
    }

    [TestMethod]
    public void ToString_EveryValueInRange_FitsTheStackBuffer()
    {
        // Страховка для MaxNumeralLength: если буфер окажется мал для какого-то значения,
        // ToRoman перезапишет его границу и бросит IndexOutOfRangeException прямо здесь.
        // Самый длинный вывод — 15 символов на 3888, а не на верхней границе диапазона:
        // вычитательные пары короче аддитивных серий, которые они заменяют, поэтому
        // MMMCMXCIX (3999) занимает всего 9. Границы приходится дублировать — константы
        // private, тест до них не достаёт.
        var longest = 0;
        var longestAt = 0;

        for (var value = 1; value <= 3999; value++)
        {
            var length = new Roman(value).ToString().Length;
            if (length <= longest) continue;

            longest = length;
            longestAt = value;
        }

        Assert.AreEqual(15, longest);
        Assert.AreEqual(3888, longestAt);
        Assert.AreEqual("MMMDCCCLXXXVIII", new Roman(longestAt).ToString());
    }

    #endregion

    #region Tests Round-trip

    [TestMethod]
    [DataRow(1)]
    [DataRow(4)]
    [DataRow(9)]
    [DataRow(42)]
    [DataRow(99)]
    [DataRow(500)]
    [DataRow(1984)]
    [DataRow(3999)]
    public void RoundTrip_IntToStringToInt_PreservesValue(int original)
    {
        var roman = new Roman(original);
        var str = roman.ToString();
        var parsed = new Roman(str);

        Assert.AreEqual(original, parsed.ToInt());
    }

    #endregion

    #region Tests Mixed Roman/int Arithmetic

    // The 1-3999 guard must hold for every arithmetic expression involving a Roman, not only for
    // Roman-with-Roman. These tests pin that contract down: before the int overloads existed, the
    // implicit Roman-to-int conversion let overload resolution select the predefined int operators
    // and every one of these silently produced an out-of-range int instead of throwing.

    [TestMethod]
    public void Add_RomanPlusInt_ExceedsMaximum_ThrowsOverflowException()
    {
        var max = new Roman(3999);

        Assert.ThrowsExactly<OverflowException>(() => { _ = max + 1; });
    }

    [TestMethod]
    public void Add_IntPlusRoman_ExceedsMaximum_ThrowsOverflowException()
    {
        var max = new Roman(3999);

        Assert.ThrowsExactly<OverflowException>(() => { _ = 1 + max; });
    }

    [TestMethod]
    public void Subtract_RomanMinusInt_BelowMinimum_ThrowsOverflowException()
    {
        var five = new Roman(5);

        Assert.ThrowsExactly<OverflowException>(() => { _ = five - 5; });
    }

    [TestMethod]
    public void Multiply_RomanTimesInt_ExceedsMaximum_ThrowsOverflowException()
    {
        var value = new Roman(2000);

        Assert.ThrowsExactly<OverflowException>(() => { _ = value * 2; });
    }

    [TestMethod]
    public void Divide_RomanByLargerInt_BelowMinimum_ThrowsOverflowException()
    {
        var three = new Roman(3);

        Assert.ThrowsExactly<OverflowException>(() => { _ = three / 4; });
    }

    [TestMethod]
    public void Add_RomanPlusInt_ValidResult_StaysRoman()
    {
        var result = new Roman(10) + 5;

        // Currently "15": the expression collapses to int, so the Roman formatting is lost.
        Assert.AreEqual("XV", result.ToString());
    }

    [TestMethod]
    public void Equality_RomanEqualsInt_AgreesWithEqualsObject()
    {
        var roman = new Roman(42);

        // `roman == 42` compiles to an int comparison (true) while Equals(object) returns false.
        Assert.AreEqual(roman.Equals(42), roman == 42);
    }

    [TestMethod]
    public void ExplicitInt_NullRoman_ThrowsArgumentNullException()
    {
        Roman nothing = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => { _ = (int)nothing; });
    }

    [TestMethod]
    [DataRow(10, 5, "XV")]
    [DataRow(1, 3998, "MMMCMXCIX")]
    [DataRow(5, -3, "II")]
    public void Add_RomanPlusInt_ValidResult_ReturnsExpectedRoman(int left, int right, string expected)
    {
        // A negative or out-of-range operand is legal as long as the *result* is representable.
        Assert.AreEqual(expected, (new Roman(left) + right).ToString());
    }

    [TestMethod]
    public void Subtract_IntMinusRoman_ValidResult_ReturnsRoman()
    {
        Assert.AreEqual("XXX", (50 - new Roman(20)).ToString());
    }

    [TestMethod]
    public void Multiply_IntTimesRoman_ValidResult_ReturnsRoman()
    {
        Assert.AreEqual("LVI", (7 * new Roman(8)).ToString());
    }

    [TestMethod]
    public void Divide_IntByRoman_TruncatesTowardZero()
    {
        Assert.AreEqual("III", (10 / new Roman(3)).ToString());
    }

    [TestMethod]
    public void Add_RomanPlusIntMaxValue_ThrowsOverflowException()
    {
        // Widening to long is what keeps this from wrapping around into a valid-looking result.
        var one = new Roman(1);

        Assert.ThrowsExactly<OverflowException>(() => { _ = one + int.MaxValue; });
    }

    [TestMethod]
    public void Subtract_RomanMinusIntMinValue_ThrowsOverflowException()
    {
        var one = new Roman(1);

        Assert.ThrowsExactly<OverflowException>(() => { _ = one - int.MinValue; });
    }

    [TestMethod]
    public void Divide_RomanByZero_ThrowsDivideByZeroException()
    {
        // Unreachable while both operands were Roman, since zero is not representable.
        var ten = new Roman(10);

        Assert.ThrowsExactly<DivideByZeroException>(() => { _ = ten / 0; });
    }

    [TestMethod]
    public void Arithmetic_NullRomanOperand_ThrowsArgumentNullException()
    {
        Roman nothing = null;

        Assert.AreEqual("a", Assert.ThrowsExactly<ArgumentNullException>(() => { _ = nothing + 1; }).ParamName);
        Assert.AreEqual("b", Assert.ThrowsExactly<ArgumentNullException>(() => { _ = 1 + nothing; }).ParamName);
        Assert.AreEqual("a", Assert.ThrowsExactly<ArgumentNullException>(() => { _ = nothing - 1; }).ParamName);
        Assert.AreEqual("a", Assert.ThrowsExactly<ArgumentNullException>(() => { _ = nothing * 1; }).ParamName);
        Assert.AreEqual("a", Assert.ThrowsExactly<ArgumentNullException>(() => { _ = nothing / 1; }).ParamName);
    }

    #endregion

    #region Tests Comparison and Equality against int

    [TestMethod]
    public void Comparison_RomanAgainstInt_MatchesIntegerOrdering()
    {
        var fifty = new Roman(50);

        Assert.IsTrue(fifty > 30);
        Assert.IsFalse(fifty > 50);
        Assert.IsTrue(fifty >= 50);
        Assert.IsTrue(fifty < 80);
        Assert.IsTrue(fifty <= 50);
        Assert.IsFalse(fifty < 50);
    }

    [TestMethod]
    public void Comparison_IntAgainstRoman_MatchesIntegerOrdering()
    {
        var fifty = new Roman(50);

        Assert.IsTrue(80 > fifty);
        Assert.IsFalse(30 > fifty);
        Assert.IsTrue(50 >= fifty);
        Assert.IsTrue(30 < fifty);
        Assert.IsTrue(50 <= fifty);
    }

    [TestMethod]
    public void Comparison_RomanAgainstOutOfRangeInt_DoesNotThrow()
    {
        // Comparison never constructs a Roman, so an unrepresentable bound is a fair question.
        var max = new Roman(3999);

        Assert.IsTrue(max < 5000);
        Assert.IsTrue(max > 0);
        Assert.IsFalse(max == 4000);
    }

    [TestMethod]
    public void Comparison_NullRomanAgainstInt_SortsLowest()
    {
        Roman nothing = null;

        Assert.IsFalse(nothing > 1);
        Assert.IsTrue(nothing < 1);
        Assert.IsFalse(nothing >= 1);
        Assert.IsTrue(nothing <= 1);

        Assert.IsTrue(1 > nothing);
        Assert.IsFalse(1 < nothing);
        Assert.IsTrue(1 >= nothing);
        Assert.IsFalse(1 <= nothing);
    }

    [TestMethod]
    public void Equality_RomanAgainstInt_ComparesByValue()
    {
        var roman = new Roman(42);

        Assert.IsTrue(roman == 42);
        Assert.IsTrue(42 == roman);
        Assert.IsFalse(roman != 42);
        Assert.IsTrue(roman != 43);
    }

    [TestMethod]
    public void Equality_NullRomanAgainstInt_ReturnsFalse()
    {
        Roman nothing = null;

        Assert.IsFalse(nothing == 42);
        Assert.IsFalse(42 == nothing);
        Assert.IsTrue(nothing != 42);
    }

    [TestMethod]
    public void EqualsInt_MatchesValue()
    {
        var roman = new Roman(42);

        Assert.IsTrue(roman.Equals(42));
        Assert.IsFalse(roman.Equals(43));
    }

    [TestMethod]
    public void CompareToInt_MatchesIntegerOrdering()
    {
        var roman = new Roman(42);

        Assert.IsTrue(roman.CompareTo(30) > 0);
        Assert.AreEqual(0, roman.CompareTo(42));
        Assert.IsTrue(roman.CompareTo(50) < 0);
    }

    [TestMethod]
    public void GetHashCode_RomanAndEqualInt_Agree()
    {
        // Equals(int) returning true obliges the hash codes to match.
        var roman = new Roman(42);

        Assert.AreEqual(42.GetHashCode(), roman.GetHashCode());
    }

    #endregion
}
