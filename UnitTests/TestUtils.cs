using EventLogGenerator.Models;
using Newtonsoft.Json;

namespace UnitTests;

public static class TestUtils
{
    public static void CompareWithProcessFixture(Process process, string fileName)
    {
        string generated = JsonConvert.SerializeObject(process, Formatting.Indented);
        string fixture = File.ReadAllText("../../../../UnitTests/Fixtures/Examples/" + fileName);
        Assert.True(String.Equals(generated, fixture));
    }

    public static void CompareFiles(string path1, string path2)
    {
        string content1 = File.ReadAllText(path1);
        string content2 = File.ReadAllText(path2);
        Assert.True(string.Equals(content1, content2, StringComparison.OrdinalIgnoreCase));
    }
}