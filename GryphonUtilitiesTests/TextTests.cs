using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GryphonUtilities.Tests;

[TestClass]
public class TextTests
{
    [TestMethod]
    public void GetNounFormTest()
    {
        AssertForm(1, Form1);
        AssertForm(2, Form24);
        AssertForm(3, Form24);
        AssertForm(4, Form24);
        for (uint i = 5; i <= 20; i++)
        {
            AssertForm(i, FormAlot);
        }

        for (uint i = 21; i <= 30; i++)
        {
            uint j = i - 20;
            AssertForms(i, j);
        }

        for (uint i = 31; i <= 100; i++)
        {
            uint j = i - 10;
            AssertForms(i, j);
        }
    }

    [TestMethod]
    public void FormatNumericWithNounTest()
    {
        AssertFormat("1 день", 1);
        AssertFormat("7 дней", 7);
        AssertFormat("32 дня", 32);
        AssertFormat("365 дней", 365);
    }

    private static void AssertFormat(string expected, uint number)
    {
        Assert.AreEqual(expected, Text.FormatNumericWithNoun(Format, number, Form1, Form24, FormAlot));
    }

    private static void AssertForms(uint number1, uint number2)
    {
        Assert.AreEqual(Text.GetNounForm(number1, Form1, Form24, FormAlot),
            Text.GetNounForm(number2, Form1, Form24, FormAlot));
    }

    private static void AssertForm(uint number, string expected)
    {
        Assert.AreEqual(expected, Text.GetNounForm(number, Form1, Form24, FormAlot));
    }

    private const string Form1 = "день";
    private const string Form24 = "дня";
    private const string FormAlot = "дней";
    private const string Format = "{0} {1}";
}