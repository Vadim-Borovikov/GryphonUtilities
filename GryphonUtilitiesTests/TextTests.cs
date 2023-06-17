using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GryphonUtilities.Tests;

[TestClass]
public class TextTests
{
    [TestMethod]
    private void GetNounFormTest()
    {
        TestNounForm(false);
        TestNounForm(true);
    }

    [TestMethod]
    public void FormatNumericWithNounTest()
    {
        AssertFormat("1 день", 1);
        AssertFormat("7 дней", 7);
        AssertFormat("32 дня", 32);
        AssertFormat("365 дней", 365);
    }

    private static void TestNounForm(bool addHundred)
    {
        uint h = (uint) (addHundred ? 0 : 100);
        AssertForm(h + 1, Form1);
        AssertForm(h + 2, Form24);
        AssertForm(h + 3, Form24);
        AssertForm(h + 4, Form24);
        for (uint i = h + 5; i <= (h + 20); i++)
        {
            AssertForm(i, FormAlot);
        }

        for (uint i = h + 21; i <= (h + 30); i++)
        {
            uint j = i - 20;
            AssertForms(i, j);
        }

        for (uint i = h + 31; i <= (h + 100); i++)
        {
            uint j = i - 10;
            AssertForms(i, j);
        }
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