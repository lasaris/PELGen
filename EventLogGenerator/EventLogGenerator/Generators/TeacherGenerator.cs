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
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp,StudentId,OwnerId", "teacher.csv");
        IdService.ResetService();
        SprinkleService.ResetService();
        ReactiveStateService.ResetService();
        FixedTimeStateService.ResetService();
        Collector.CreateCollectorMap();

        // Prepare Actors
        List<Actor> teachers = Enumerable.Range(0, teacherCount)
            .Select(_ => new Actor(EActorType.Teacher))
            .ToList();

        // Define resources
        var vaultHomework1 = new Resource("Homework vault 1");
        var vaultHomework2 = new Resource("Homework vault 2");
        var vaultHomework3 = new Resource("Homework vault 3");

        var materialsWeek1 = new Resource("/um/slides-week01.pdf");
        var materialsWeek2 = new Resource("/um/slides-week02.pdf");
        var materialsWeek3 = new Resource("/um/slides-week03.pdf");
        var materialsWeek4 = new Resource("/um/slides-week04.pdf");
        var materialsWeek5 = new Resource("/um/slides-week05.pdf");
        var materialsWeek6 = new Resource("/um/slides-week06.pdf");

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
        
        // Create study materials process

        var defaultRules = new StateRules(true);

        var createStudyMaterials1 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek1,
            defaultRules,
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 24, 0, 1, 0))
        );

        var createStudyMaterials2 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek2,
            defaultRules,
            new TimeFrame(new DateTime(2023, 1, 03), new DateTime(2023, 1, 03, 0, 1, 0))
        );

        var createStudyMaterials3 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek3,
            defaultRules,
            new TimeFrame(new DateTime(2023, 1, 10), new DateTime(2023, 1, 10, 0, 1, 0))
        );

        var createStudyMaterials4 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek4,
            defaultRules,
            new TimeFrame(new DateTime(2023, 1, 17), new DateTime(2023, 1, 17, 0, 1, 0))
        );

        var createStudyMaterials5 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek5,
            defaultRules,
            new TimeFrame(new DateTime(2023, 1, 24), new DateTime(2023, 1, 24, 0, 1, 0))
        );

        var createStudyMaterials6 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek6,
            defaultRules,
            new TimeFrame(new DateTime(2023, 1, 31), new DateTime(2023, 1, 31, 0, 1, 0)),
            true
        );
        
        createStudyMaterials1.AddFollowingStates((createStudyMaterials2, 1f));
        createStudyMaterials2.AddFollowingStates((createStudyMaterials3, 1f));
        createStudyMaterials3.AddFollowingStates((createStudyMaterials4, 1f));
        createStudyMaterials4.AddFollowingStates((createStudyMaterials5, 1f));
        createStudyMaterials5.AddFollowingStates((createStudyMaterials6, 1f));

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
        
        var markSeminarAbsence = new ReactiveState(
            EActivityType.MarkAbsence,
            placeholder,
            EActivityType.ReceiveAbsence
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
        var viewStudentRecord = new IntervalSprinkleState(
            EActivityType.VisitStudentRecord,
            studentCourseRecord,
            new TimeFrame(new DateTime(2022, 12, 14), new DateTime(2023, 3, 1))
        );

        // FIXME: Instead of parsing teachers[0], there should be some general strategy
        // to tell, which actors should participate (i.e. parsing all actors and then EStrategy.First)
        var actorFrame = new ActorFrame(teachers[0], createStudyMaterials1);
        // FIXME: StateEvaluator.RunProcess() can take these parameters
        StateEvaluator.InitializeEvaluator(actorFrame, new DateTime(2023, 2, 1));
        var filledActorFrame = StateEvaluator.RunProcess(createStudyMaterials1);
        
        ReactiveStateService.RunReactiveStates(Collector.GetPreviousCollection(), teachers);
        
        foreach (var actor in teachers)
        {
            SprinkleService.RunIntervalSprinkles(actor);
        }
        
        // TODO: At fixed time create scan files for student exams
        
        // TODO: Activity of deleting ropot sessions
    }
}