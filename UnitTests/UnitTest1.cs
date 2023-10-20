using System.Reflection;
using EventLogGenerationLibrary;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerator.Models;
using EventLogGenerator.Services;
using Newtonsoft.Json;

namespace UnitTests;

/// <summary>
/// Test that examples are generated correctly. The logic behind generation should not change over time.
/// </summary>
[TestFixture]
public class TestGeneratedProcessesFixture
{
    public string PathToExamples = "../../../../UnitTests/Fixtures/Examples/";

    [Test]
    public void SingleTraceProcessTest()
    {
        RandomService.SetSeed(4269123);

        var resource = "Assembly line 42";
        var state1 = new ProcessState("Component assembly", resource, 1, new DateTime(2023, 10, 13, 12, 00, 00));
        var state2 = new ProcessState("Quality inspection", resource, 1, new DateTime(2023, 10, 13, 12, 00, 10));
        var state3 = new ProcessState("Product packing", resource, 1, new DateTime(2023, 10, 13, 13, 30, 40), true);

        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);

        var config = new Configuration(1, state1, 1, null, "manufacture.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Worker");

        var generator = new EventGenerator(config);
        var process = generator.RunGeneration();

        TestUtils.CompareWithProcessFixture(process, "single-trace.json");
        TestUtils.CompareFiles(PathToExamples + "manufacture.csv", "./generated/manufacture.csv");
    }

    [Test]
    public void SingleTraceProcessRandomTimesTest()
    {
        RandomService.SetSeed(4269123);

        var resource = "Morning routine";
        var state1 = new ProcessState("Wake up", resource, 1, new DateTime(2023, 10, 27, 7, 00, 00));
        var state2 = new ProcessState("Take shower", resource, 1,
            new TimeFrame(new DateTime(2023, 10, 27, 7, 00, 00), new DateTime(2023, 10, 27, 7, 30, 00)));
        var state3 = new ProcessState("Prepare breakfast", resource, 1,
            new TimeFrame(new DateTime(2023, 10, 27, 7, 30, 00), new DateTime(2023, 10, 27, 8, 00, 00),
                ETimeFrameDistribution.ReverseExponential), true);

        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);

        var config = new Configuration(1, state1, 1, null, "routine.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Human");

        var generator = new EventGenerator(config);
        var process = generator.RunGeneration();
        
        TestUtils.CompareWithProcessFixture(process, "single-trace-random.json");
        TestUtils.CompareFiles(PathToExamples + "routine.csv", "./generated/routine.csv");
    }
    
    [Test]
    public void ProcessWithBranchingTest()
    {
        RandomService.SetSeed(4269123);

        var quiz = "Test quiz";
        var question1 = "Question 1";
        var question2 = "Question 2";
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

        openQuiz.AddFollowingStates((answerYes, 0.8f), (answerNo, 0.2f));
        answerYes.AddFollowingStates((answerTrue, 0.3f), (answerFalse, 0.7f));
        answerNo.AddFollowingStates((answerTrue, 0.5f), (answerFalse, 0.5f));
        answerTrue.AddFollowingState(submitQuiz);
        answerFalse.AddFollowingState(submitQuiz);

        var config = new Configuration(30, openQuiz, 1, null, "quiz.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Student");

        var generator = new EventGenerator(config);
        var process = generator.RunGeneration();
        
        TestUtils.CompareWithProcessFixture(process, "branching.json");
        TestUtils.CompareFiles(PathToExamples + "quiz.csv", "./generated/quiz.csv");
    }
    
    [Test]
    public void ProcessWithSprinklesTest()
    {
        RandomService.SetSeed(4269123);

        var resource = "IT Project";
        var state1 = new ProcessState("Requirement gathering", resource, 1, new DateTime(2023, 8, 10, 10, 00, 00));
        var state2 = new ProcessState("Development start", resource, 1, new DateTime(2023, 8, 14, 08, 00, 00));
        var state3 = new ProcessState("Acceptance testing", resource, 1, new DateTime(2023, 10, 10, 13, 00, 00), true);

        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);

        var planningMeeeting = new SprinkleState("Planning team meet", "Google Meet",
            new HashSet<ProcessState>() { state1 },
            new HashSet<ProcessState>() { state2 });
        var qualityMeeting = new SprinkleState("QA team meeting", "Zoom",
            new HashSet<ProcessState>() { state2 },
            new HashSet<ProcessState>() { state3 }, null, null, null, 4);

        var config = new Configuration(1, state1, 1, null, "project.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Company");

        var generator = new EventGenerator(config);
        var process = generator.RunGeneration();
        
        TestUtils.CompareWithProcessFixture(process, "sprinkles.json");
        TestUtils.CompareFiles(PathToExamples + "project.csv", "./generated/project.csv");
    }
    
    [Test]
    public void ProcessWithMoreStatesTest()
    {
        RandomService.SetSeed(4269123);

        var resource = "Support Software";
        var state1 = new ProcessState("Issue reported", resource, 1, new DateTime(2023, 8, 10, 22, 00, 00));
        var state2 = new ProcessState("Analysis performed", resource, 1, new DateTime(2023, 8, 14, 08, 00, 00));
        var state3 = new ProcessState("Resolution added", resource, 1, new DateTime(2023, 8, 29, 16, 00, 00));
        var state4 = new ProcessState("Feedback gathered", resource, 1, new DateTime(2023, 9, 2, 13, 30, 00), true);

        state1.AddFollowingStates((state2, 0.6f), (state3, 0.4f));
        state2.AddFollowingState(state3);
        state3.AddFollowingState(state4);

        var dynamicSprinkle = new DynamicSprinkleState("Notify analyst", "Emailing service",
            new HashSet<ProcessState>() { state1 }, TimeSpan.FromHours(1), ETimeFrameDistribution.Linear);

        var conditionalSprinkle = new ConditionalSprinkle(state2, new DummyState("Review analysis", "Jira"),
            null,
            TimeSpan.FromDays(2));

        var fixedState =
            new FixedTimeState("Update due date", "Time cron", new DateTime(2023, 9, 01));

        var intervalSprinkle = new IntervalSprinkleState("Quality assurance", "Testing SW",
            new TimeFrame(new DateTime(2023, 8, 15, 08, 00, 00), new DateTime(2023, 9, 2, 13, 30, 00)));

        var periodicSprinkle = new PeriodicSprinkleState("Tickets backup", "Database service",
            new HashSet<ProcessState>() { state1 }, new HashSet<ProcessState>() { state4 }, TimeSpan.FromDays(7));

        var config = new Configuration(30, state1, 1, null, "support.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Ticket");

        var generator = new EventGenerator(config);
        var process = generator.RunGeneration();
        
        TestUtils.CompareWithProcessFixture(process, "more-states.json");
        TestUtils.CompareFiles(PathToExamples + "support.csv", "./generated/support.csv");
    }
    
    [Test]
    public void DependantProcessTest()
    {
        RandomService.SetSeed(4269123);

        var resource = "Assembly line 42";
        var state1 = new ProcessState("Component assembly", resource, 1, new DateTime(2023, 10, 13, 12, 00, 00));
        var state2 = new ProcessState("Quality inspection", resource, 1, new DateTime(2023, 10, 13, 12, 00, 10));
        var state3 = new ProcessState("Product packing", resource, 1, new DateTime(2023, 10, 13, 13, 30, 40), true);
        state1.AddFollowingState(state2);
        state2.AddFollowingState(state3);
        var config = new Configuration(1, state1, 1, null, "reactive-manufacture.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Suspected worker");
        var generator = new EventGenerator(config);
        var previousProcess = generator.RunGeneration();
        var newState1 = new ProcessState("Begin daily inspection", "Management System", 1, new DateTime(2023, 10, 13, 8, 0, 0),
            true);
        
        var reactiveState1 = new ReactiveState("Check assembly", "placeholder", "Component assembly");
        var reactiveState2 = new ReactiveState("Check quality inspection", "placeholder", "Quality inspection", null,
            "Quality management system", TimeSpan.FromMinutes(5));
        var patternReaction = new PatternReaction(new List<string>(){ "Quality inspection", "Product packing" },
            "Product packing", "Check packed product");

        var newConfig = new Configuration(1, newState1, 1, null, "reactive-manager.csv",
            "ActorId,ActorType,Activity,Resource,Timestamp",
            "Manager", previousProcess);
        
        // Generate process with reactive states
        var newGenerator = new EventGenerator(newConfig);
        var process =newGenerator.RunGeneration();
        
        TestUtils.CompareWithProcessFixture(process, "reactive-process.json");
        TestUtils.CompareFiles(PathToExamples + "reactive-manager.csv", "./generated/reactive-manager.csv");
    }
}