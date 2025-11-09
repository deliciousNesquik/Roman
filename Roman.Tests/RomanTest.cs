using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roman;

namespace Roman.Tests;

[TestClass]
[TestSubject(typeof(Roman))]
public class RomanTest
{

    #region Tests Ctor

    [TestMethod]
    public void Ctor_LowerLimit_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Roman(0));
        Assert.AreEqual("Value must be between 1 and 3999.", ex.Message);
    }
    
    [TestMethod]
    public void Ctor_UpperLimit_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Roman(4000));
        Assert.AreEqual("Value must be between 1 and 3999.", ex.Message);
    }
    
    [TestMethod]
    public void Ctor_IntValue_CreateObject()
    {
        var roman = new Roman(1);
        Assert.AreEqual("I", roman.ToString());
    }
    
    [TestMethod]
    public void Ctor_StringValue_CreateObject()
    {
        var roman = new Roman("I");
        Assert.AreEqual("I", roman.ToString());
    }
    
    [TestMethod]
    public void Ctor_InvalidStringValue_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Roman("B"));
        Assert.AreEqual("Invalid Roman numeral character: 'B'.", ex.Message);
    }
    
    [TestMethod]
    public void Ctor_EmptyValue_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Roman(""));
        Assert.AreEqual("Roman numeral cannot be empty.", ex.Message);
    }
    
    [TestMethod]
    public void Ctor_NegativeStringValue_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Roman("-X"));
        Assert.AreEqual("Value must be positive.", ex.Message);
    }
    
    [TestMethod]
    public void Ctor_OtherRomanValue_CreateObject()
    {
        var otherRoman = new Roman(10);
        var roman = new Roman(otherRoman);
        Assert.AreEqual("X", roman.ToString());
    }

    #endregion

    #region Tests conversion

    [TestMethod]
    public void Conversion_RomanConvInt_ReturnInteger()
    {
        var roman = new Roman("I");
        Assert.AreEqual(1, (int)roman);
    }

    #endregion


    #region Tests arithmetic

    [TestMethod]
    public void Arithmetic_ValidSum_ReturnValidValue()
    {
        var roman = new Roman("I");
        var roman1 = new Roman("II");
        Assert.AreEqual("III", (roman + roman1).ToString());
    }
    
    [TestMethod]
    public void Arithmetic_SumWithArgumentANull_ArgumentNullException()
    {
        Roman roman1 = null;
        var roman2 = new Roman("II");

        var ex = Assert.Throws<ArgumentNullException>(() => roman1 + roman2);
        Assert.AreEqual("Value cannot be null. (Parameter 'a')", ex.Message);
    }
    
    [TestMethod]
    public void Arithmetic_SumWithArgumentBNull_ArgumentNullException()
    {
        Roman roman2 = null;
        var roman1 = new Roman("II");

        var ex = Assert.Throws<ArgumentNullException>(() => roman1 + roman2);
        Assert.AreEqual("Value cannot be null. (Parameter 'b')", ex.Message);
    }
    
    [TestMethod]
    public void Arithmetic_UpperLimitSum_ArgumentOutOfRangeException()
    {
        var roman1 = new Roman(3999);
        var roman2 = new Roman(1);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => roman1 + roman2);
        Assert.AreEqual("Resulting value must be ≤ 3999.", ex.Message);
    }
    
    [TestMethod]
    public void Arithmetic_ValidSubtraction_ReturnValidValue()
    {
        var roman = new Roman("V");
        var roman1 = new Roman("II");
        Assert.AreEqual("III", (roman - roman1).ToString());
    }
    
    [TestMethod]
    public void Arithmetic_SubWithArgumentANull_ArgumentNullException()
    {
        Roman roman1 = null;
        var roman2 = new Roman("II");

        var ex = Assert.Throws<ArgumentNullException>(() => roman1 - roman2);
        Assert.AreEqual("Value cannot be null. (Parameter 'a')", ex.Message);
    }
    
    [TestMethod]
    public void Arithmetic_SubWithArgumentBNull_ArgumentNullException()
    {
        Roman roman2 = null;
        var roman1 = new Roman("II");

        var ex = Assert.Throws<ArgumentNullException>(() => roman1 - roman2);
        Assert.AreEqual("Value cannot be null. (Parameter 'b')", ex.Message);
    }
    
    [TestMethod]
    public void Arithmetic_LowerLimitSubtraction_ArgumentOutOfRangeException()
    {
        var roman1 = new Roman(1);
        var roman2 = new Roman(1);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => roman1 - roman2);
        Assert.AreEqual("Roman numerals cannot represent zero or negative values.", ex.Message);
    }

    #endregion
}