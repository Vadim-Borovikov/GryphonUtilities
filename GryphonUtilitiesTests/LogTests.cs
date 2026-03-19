using GryphonUtilities.Logging;
using GryphonUtilities.Time;
using JetBrains.Annotations;

namespace GryphonUtilities.Tests;

[TestClass]
[UsedImplicitly]
public class LogTests
{
    [TestMethod]
    [UsedImplicitly]
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