using EventLogGenerationLibrary;

namespace UnitTests;

/// <summary>
/// Tests that all necessary objects are accessible and have the expected parameters.
/// </summary>
[TestFixture]
public class ObjectsFixture
{
    [Test]
    public void CreateConfiguration()
    {
        Assert.Pass();
    }
    
    [Test]
    public void CreateProcessStates()
    {
        Assert.Pass();
    }
    
    [Test]
    public void CreateSprinkleStates()
    {
        Assert.Pass();
    }
    
    [Test]
    public void CreateOtherStates()
    {
        Assert.Pass();
    }
}

/// <summary>
/// Tests very simple process runs only using ProcessStates.
/// </summary>
[TestFixture]
public class SimpleProcessFixture
{
    [Test]
    public void SampleTest()
    {
        Assert.Pass();
    }
}

/// <summary>
/// Tests simple process while using SprinkleService and its features with different SprinkleStates.
/// </summary>
[TestFixture]
public class ProcessWithSprinklesFixture
{
    [Test]
    public void SampleTest()
    {
        Assert.Pass();
    }
}

/// <summary>
/// Takes all the examples from the Examples project and checks whether they are correctly generated. 
/// </summary>
[TestFixture]
public class ExampleCasesFixture
{
    [Test]
    public void SampleTest()
    {
        Assert.Pass();
    }
}

/// <summary>
/// This is a complex fixture that uses most of the features from the PELGen.
/// It tests that the InformationSystemGeneration example project was generated correctly.
/// </summary>
[TestFixture]
public class InformationSystemGenerationFixture
{
    [Test]
    public void SampleTest()
    {
        Assert.Pass();
    }
}