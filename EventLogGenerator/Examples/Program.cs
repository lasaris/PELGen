using EventLogGenerationLibrary;

namespace Examples;

/// <summary>
/// This project contains multiple examples of the library usage.
/// More complex example is in InformationSystemGeneration project.
/// </summary>
internal class Program
{
    /// <summary>
    /// Firstly we try to model a process with 3 single activities following each other in a strict path.
    /// </summary>
    private static void SingleTraceProcess()
    {
        // TODO
    }
    
    /// <summary>
    /// Now we can add more activities and additional branching of the traces based on given probabilities.
    /// </summary>
    private static void ProcessWithBranching()
    {
        // TODO
    }
    
    /// <summary>
    /// Here we add sprinkles, which mimic random events and are added randomly for a given timeframe and distribution.
    /// </summary>
    private static void ProcessWithSprinkles()
    {
        // TODO
    }
    
    /// <summary>
    /// This example shows all currently available kinds of sprinkle states.
    /// </summary>
    private static void ProcessWithAllSprinkles()
    {
        // TODO
    }
    
    /// <summary>
    /// Finally we can add another process that reacts to the previous process run.
    /// </summary>
    private static void DependantProcess()
    {
        // TODO
    }

    /// <summary>
    /// In main function we can run all the examples declared above.
    /// </summary>
    public static void Main()
    {
        SingleTraceProcess();
        ProcessWithBranching();
        ProcessWithSprinkles();
        ProcessWithAllSprinkles();
    }
}