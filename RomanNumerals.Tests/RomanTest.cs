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
    public void Ctor_String_Null_ThrowsArgumentException()
    {
        var ex = Assert.ThrowsExactly<ArgumentException>(() => new Roman((string)null));
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
    public void Addition_ExceedsUpperLimit_ThrowsArgumentOutOfRangeException()
    {
        var a = new Roman(3999);
        var b = new Roman(1);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a + b);
        Assert.AreEqual("b", ex.ParamName);
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
    public void Subtraction_ResultZero_ThrowsArgumentOutOfRangeException()
    {
        var a = new Roman(5);
        var b = new Roman(5);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a - b);
        Assert.AreEqual("b", ex.ParamName);
        StringAssert.Contains(ex.Message, "zero or negative");
    }

    [TestMethod]
    public void Subtraction_ResultNegative_ThrowsArgumentOutOfRangeException()
    {
        var a = new Roman(5);
        var b = new Roman(10);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a - b);
        Assert.AreEqual("b", ex.ParamName);
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
    public void Multiplication_ExceedsUpperLimit_ThrowsArgumentOutOfRangeException()
    {
        var a = new Roman(2000);
        var b = new Roman(3);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a * b);
        Assert.AreEqual("b", ex.ParamName);
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
    public void Division_ResultLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var a = new Roman(1);
        var b = new Roman(2);

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => a / b);
        Assert.AreEqual("b", ex.ParamName);
        StringAssert.Contains(ex.Message, ">= 1");
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
    public void GreaterThan_WithNull_ReturnsFalse()
    {
        var a = new Roman(5);
        Roman b = null;

        Assert.IsTrue(a > b);
        Assert.IsFalse(b > a);
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
    public void LessThan_NullAndNonNull_ReturnsTrue()
    {
        Roman a = null;
        var b = new Roman(5);

        Assert.IsTrue(a < b);
        Assert.IsFalse(b < a);
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
    public void GreaterOrEqual_BothNull_ReturnsTrue()
    {
        Roman a = null;
        Roman b = null;

        Assert.IsTrue(a >= b);
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
    public void LessOrEqual_NullValue_ReturnsTrue()
    {
        Roman a = null;
        var b = new Roman(5);

        Assert.IsTrue(a <= b);
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

    #endregion

    #region Tests Conversions

    [TestMethod]
    public void ImplicitConversion_ToInt_ReturnsCorrectValue()
    {
        var roman = new Roman(42);
        int value = roman;

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
}