using EventLogGenerator.GenerationLogic;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator;

public static class TeacherGenerator
{
    public static void GenerateLogs(int teacherCount = 1)
    {
        // Setup services
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp", "teacher.csv");
        IdService.ResetService();
        SprinkleService.ResetService();
        ReactiveStateService.ResetService();
        Collector.CreateCollectorMap();

        // Prepare Actors
        List<Actor> teachers = Enumerable.Range(0, teacherCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();

        // Define resources
        var vaultHomework1 = new Resource("Homework vault 1");
        var vaultHomework2 = new Resource("Homework vault 2");
        var vaultHomework3 = new Resource("Homework vault 3");

        var placeholder = new Resource("Fill me please");


        // TODO: Sprinkling afterwards

        // Fixed time states

        var createVaultHomework1 = new FixedTimeState(
            EActivityType.CreateHomeworkVault,
            vaultHomework1,
            new DateTime(2023, 1, 9)
        );

        var createVaultHomework2 = new FixedTimeState(
            EActivityType.CreateHomeworkVault,
            vaultHomework2,
            new DateTime(2023, 1, 23)
        );

        var createVaultHomework3 = new FixedTimeState(
            EActivityType.CreateHomeworkVault,
            vaultHomework3,
            new DateTime(2023, 2, 6)
        );

        // Reactive states

        var givePointsHomework = new ReactiveState(
            EActivityType.GivePoints,
            placeholder,
            EActivityType.ReceivePoints
        );

        var markSeminarAttendance = new ReactiveState(
            EActivityType.MarkAttendance,
            placeholder,
            EActivityType.ReceiveAttendance
        );

        var giveFinalGrade1 = new ReactiveState(
            EActivityType.GiveFinalGrade,
            placeholder,
            EActivityType.FailExam
        );
        
        var giveFinalGrade2 = new ReactiveState(
            EActivityType.GiveFinalGrade,
            placeholder,
            EActivityType.PassExam
        );

        FixedTimeStateService.RunFixedStates(teachers[0]);
        ReactiveStateService.RunReactiveStates(Collector.GetPreviousCollection(), teachers);
    }
}