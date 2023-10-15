using EventLogGenerationLibrary;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerator.Models;

namespace Examples;

/// <summary>
/// This project contains multiple examples of the library usage.
/// More complex example is in InformationSystemGeneration project.
/// </summary>
internal class Program
{
    /// <summary>
    /// Firstly we try to model a process with 3 single activities following each other in a strict path and exact time.
    /// There is no factor of randomness in this process.
    /// </summary>
    private static void SingleTraceProcess()
    {
        // Define common resources
        var resource = "Assembly line 42";

        // Create states with exact time of execution
        var state1 = new ProcessState("Component assembly", resource, 1, new DateTime(2023, 10, 13, 12, 00, 00));
        var state2 = new ProcessState("Quality inspection", resource, 1, new DateTime(2023, 10, 13, 12, 00, 10));
        var state3 = new ProcessState("Product packing", resource, 1, new DateTime(2023, 10, 13, 13, 30, 40), true);

        // Add following states to model the process
        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);

        // Create configuration
        var config = new Configuration(1, state1, 1, null, "manufacture.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Worker");

        // Create generator and run generation
        var generator = new EventGenerator(config);
        generator.RunGeneration();
    }

    /// <summary>
    /// Here we are passing a TimeFrame object to ProcessState which allows us to randomly pick between certain times.
    /// We can additionally specify time distribution over the TimeFrame for better control.
    /// </summary>
    private static void SingleTraceProcessRandomTimes()
    {
        // Define common resources
        var resource = "Morning routine";

        // Create state with fixed start
        var state1 = new ProcessState("Wake up", resource, 1, new DateTime(2023, 10, 27, 7, 00, 00));
        // Create state with start at random time within the TimeFrame
        var state2 = new ProcessState("Take shower", resource, 1,
            new TimeFrame(new DateTime(2023, 10, 27, 7, 00, 00), new DateTime(2023, 10, 27, 7, 30, 00)));
        // Create state with start at random time within the TimeFrame based on given distribution
        // ReverseExponential means that it is exponentially more likely to occur at the start of the timeframe.
        var state3 = new ProcessState("Prepare breakfast", resource, 1,
            new TimeFrame(new DateTime(2023, 10, 27, 7, 30, 00), new DateTime(2023, 10, 27, 8, 00, 00),
                ETimeFrameDistribution.ReverseExponential), true);

        // Add following states to model the process
        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);

        // Create configuration
        var config = new Configuration(1, state1, 1, null, "routine.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Human");

        // Create generator and run generation
        var generator = new EventGenerator(config);
        generator.RunGeneration();
    }

    /// <summary>
    /// Now we can add more activities and additional branching of the traces based on given probabilities.
    /// </summary>
    private static void ProcessWithBranching()
    {
        // Define common resources
        var quiz = "Test quiz";
        var question1 = "Question 1";
        var question2 = "Question 2";

        // Create states
        var openQuiz = new ProcessState("Open quiz", quiz, 1, new DateTime(2023, 9, 23, 12, 00, 00));
        var answerYes = new ProcessState("Yes answer", question1, 1,
            new TimeFrame(new DateTime(2023, 9, 23, 12, 00, 00), new DateTime(2023, 9, 23, 12, 05, 00)));
        var answerNo = new ProcessState("No answer", question1, 1,
            new TimeFrame(new DateTime(2023, 9, 23, 12, 00, 00), new DateTime(2023, 9, 23, 12, 05, 00)));
        var answerTrue = new ProcessState("True answer", question2, 1,
            new TimeFrame(new DateTime(2023, 9, 23, 12, 05, 00), new DateTime(2023, 9, 23, 12, 10, 00)));
        var answerFalse = new ProcessState("False answer", question2, 1,
            new TimeFrame(new DateTime(2023, 9, 23, 12, 05, 00), new DateTime(2023, 9, 23, 12, 10, 00)));
        var submitQuiz = new ProcessState("Submit quiz", quiz, 1,
            new TimeFrame(new DateTime(2023, 9, 23, 12, 10, 00), new DateTime(2023, 9, 23, 12, 12, 00)), true);

        // Add following states to model the process with specified probabilities and branching
        openQuiz.AddFollowingStates((answerYes, 0.8f), (answerNo, 0.2f));
        answerYes.AddFollowingStates((answerTrue, 0.3f), (answerFalse, 0.7f));
        answerNo.AddFollowingStates((answerTrue, 0.5f), (answerFalse, 0.5f));
        answerTrue.AddFollowingState(submitQuiz);
        answerFalse.AddFollowingState(submitQuiz);

        // Create configuration and add more actors, to see results better
        var config = new Configuration(30, openQuiz, 1, null, "quiz.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Student");

        // Create generator and run generation
        var generator = new EventGenerator(config);
        generator.RunGeneration();
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
        SingleTraceProcessRandomTimes();
        ProcessWithBranching();
        ProcessWithSprinkles();
        ProcessWithAllSprinkles();
    }
}