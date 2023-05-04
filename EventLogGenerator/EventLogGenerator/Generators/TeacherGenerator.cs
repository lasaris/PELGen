using EventLogGenerator.GenerationLogic;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;
using EventLogGenerator.Services;

namespace EventLogGenerator;

public static class TeacherGenerator
{
    public static void GenerateLogs(int teacherCount = 1)
    {
        // Setup services
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp,StudentId", "teacher.csv");
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

        var materialsWeek1 = new Resource("slides-week01.pdf");
        var materialsWeek2 = new Resource("slides-week02.pdf");
        var materialsWeek3 = new Resource("slides-week03.pdf");
        var materialsWeek4 = new Resource("slides-week04.pdf");
        var materialsWeek5 = new Resource("slides-week05.pdf");
        var materialsWeek6 = new Resource("slides-week06.pdf");

        var studentCourseRecord = new Resource("Student course record");

        var placeholder = new Resource("Fill me please");

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

        var createStudyMaterials1 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek1,
            new DateTime(2022, 12, 24)
        );

        var createStudyMaterials2 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek2,
            new DateTime(2023, 1, 03)
        );

        var createStudyMaterials3 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek3,
            new DateTime(2023, 1, 10)
        );

        var createStudyMaterials4 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek4,
            new DateTime(2023, 1, 17)
        );

        var createStudyMaterials5 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek5,
            new DateTime(2023, 1, 24)
        );

        var createStudyMaterials6 = new FixedTimeState(
            EActivityType.CreateFile,
            materialsWeek6,
            new DateTime(2023, 1, 31)
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

        // Teacher sprinkles

        // TODO: This state should also have an student ID
        var viewStudentRecord = new IntervalSprinkleState(
            EActivityType.VisitStudentRecord,
            studentCourseRecord,
            new TimeFrame(new DateTime(2022, 12, 14), new DateTime(2023, 3, 1))
        );

        // FIXME: Instead of parsing teachers[0], there should be some general strategy implemented for each fixed state/all of them
        // to tell, which actors should participate (i.e. parsing all actors and then EStrategy.First,
        // which would suggest all fixed states are run by the fist teacher only)
        FixedTimeStateService.RunFixedStates(teachers[0]);
        ReactiveStateService.RunReactiveStates(Collector.GetPreviousCollection(), teachers);
        
        foreach (var actor in teachers)
        {
            SprinkleService.RunIntervalSprinkles(actor);
        }
        
        // TODO: For all states, we should also log to which ID the reactions and interval sprinkles map to (something like "StudentId" column)
    }
}