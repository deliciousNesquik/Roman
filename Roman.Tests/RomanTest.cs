using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roman;

namespace Roman.Tests;

[TestClass]
[TestSubject(typeof(Roman))]
public class RomanTest
{

    [TestMethod]
    public void ConstructorInt()
    {
        var test = new Roman(10);
        Assert.AreEqual("X", test.ToString());
    }
    
    [TestMethod]
    public void ConstructorString()
    {
        var test = new Roman("X");
        Assert.AreEqual("X", test.ToString());
    }
    
    [TestMethod]
    public void ConstructorRoman()
    {
        var test = new Roman("X");
        var test2 = new Roman("X");
        Assert.AreEqual(test2.ToString(), test.ToString());
    }
}