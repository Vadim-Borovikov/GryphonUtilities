using GryphonUtilities.Logging;
using GryphonUtilities.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GryphonUtilities.Tests;

[TestClass]
public class LogTests
{
    [TestMethod]
    public void ExceptionTest()
    {
        Clock clock = new();
        Logger logger = new(clock);

        try
        {
            throw new Exception("Test exception");
        }
        catch (Exception ex)
        {
            logger.Errors.Log(ex);
            string errors = File.ReadAllText(logger.Errors.FilePath);
            Console.WriteLine(errors);
        }
    }
}