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
        
        // FIXME: Hard coded IDs of teachers
        teachers[0].Id = 514184;
        teachers[1].Id = 515163;
        teachers[2].Id = 410452;

        // Define resources
        var materialsWeek1 = new Resource("/um/slides-week01.pdf");
        var materialsWeek2 = new Resource("/um/slides-week02.pdf");
        var materialsWeek3 = new Resource("/um/slides-week03.pdf");
        var materialsWeek4 = new Resource("/um/slides-week04.pdf");
        var materialsWeek5 = new Resource("/um/slides-week05.pdf");
        var materialsWeek6 = new Resource("/um/slides-week06.pdf");

        var homeworkAssignment1 = new Resource("/um/homework1.pdf");
        var homeworkAssignment2 = new Resource("/um/homework2.pdf");
        var homeworkAssignment3 = new Resource("/um/homework3.pdf");

        var examScan1 = new Resource("/re/scan-exam1.png");
        var examScan2 = new Resource("/re/scan-exam2.png");
        var examScan3 = new Resource("/re/scan-exam3.png");
        var examScan4 = new Resource("/re/scan-exam4.png");
        
        var studentCourseRecord = new Resource("Student course record");

        var placeholder = new Resource("Fill me please");
        
        // Prepare times
        var createMaterialsPeriod1 = new TimeFrame((2023, 2, 13, 8, 0, 0), (2023, 2, 13, 9, 0, 0));
        var createMaterialsPeriod2 = new TimeFrame((2023, 2, 27, 8, 0, 0), (2023, 2, 27, 9, 0, 0));
        var createMaterialsPeriod3 = new TimeFrame((2023, 3, 13, 8, 0, 0), (2023, 3, 13, 9, 0, 0));
        var createMaterialsPeriod4 = new TimeFrame((2023, 3, 27, 8, 0, 0), (2023, 3, 27, 9, 0, 0));
        var createMaterialsPeriod5 = new TimeFrame((2023, 4, 10, 8, 0, 0), (2023, 4, 10, 9, 0, 0));
        var createMaterialsPeriod6 = new TimeFrame((2023, 4, 24, 8, 0, 0), (2023, 4, 24, 9, 0, 0));
        
        var submitHomeworkPeriod1 = new TimeFrame((2023, 3, 5), (2023, 3, 19));
        var submitHomeworkPeriod2 = new TimeFrame((2023, 4, 2), (2023, 4, 16));
        var submitHomeworkPeriod3 = new TimeFrame((2023, 4, 30), (2023, 5, 14));

        // Fixed time states
        var publishHomework1 = new FixedTimeState(
            EActivityType.CreateFile,
            homeworkAssignment1,
            submitHomeworkPeriod1.Start
        );
        
        var publishHomework2 = new FixedTimeState(
            EActivityType.CreateFile,
            homeworkAssignment2,
            submitHomeworkPeriod2.Start
        );
        
        var publishHomework3 = new FixedTimeState(
            EActivityType.CreateFile,
            homeworkAssignment3,
            submitHomeworkPeriod3.Start
        );

        // Create study materials process

        var defaultCompulsaryRules = new StateRules(true, -1);
        var defaultOptionalRules = new StateRules();

        var createStudyMaterials1 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek1,
            defaultCompulsaryRules,
            createMaterialsPeriod1
        );

        var createStudyMaterials2 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek2,
            defaultCompulsaryRules,
            createMaterialsPeriod2
        );

        var createStudyMaterials3 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek3,
            defaultCompulsaryRules,
            createMaterialsPeriod3
        );

        var createStudyMaterials4 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek4,
            defaultCompulsaryRules,
            createMaterialsPeriod4
        );

        var createStudyMaterials5 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek5,
            defaultCompulsaryRules,
            createMaterialsPeriod5
        );

        var createStudyMaterials6 = new ProcessState(
            EActivityType.CreateFile,
            materialsWeek6,
            defaultCompulsaryRules,
            createMaterialsPeriod6,
            true
        );
        
        var removeStudyMaterials1 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek1,
            defaultOptionalRules,
            createMaterialsPeriod1.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        var removeStudyMaterials2 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek2,
            defaultOptionalRules,
            createMaterialsPeriod2.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        var removeStudyMaterials3 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek3,
            defaultOptionalRules,
            createMaterialsPeriod3.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        var removeStudyMaterials4 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek4,
            defaultOptionalRules,
            createMaterialsPeriod4.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        var removeStudyMaterials5 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek5,
            defaultOptionalRules,
            createMaterialsPeriod5.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        var removeStudyMaterials6 = new ProcessState(
            EActivityType.DeleteFile,
            materialsWeek6,
            defaultOptionalRules,
            createMaterialsPeriod6.GetTimeFrameWithOffset(null, -TimeSpan.FromMinutes(15))
        );
        
        createStudyMaterials1.AddFollowingStates((createStudyMaterials2, 0.6f), (removeStudyMaterials1, 0.4f));
        createStudyMaterials2.AddFollowingStates((createStudyMaterials3, 0.6f), (removeStudyMaterials2, 0.4f));
        createStudyMaterials3.AddFollowingStates((createStudyMaterials4, 0.6f), (removeStudyMaterials3, 0.4f));
        createStudyMaterials4.AddFollowingStates((createStudyMaterials5, 0.6f), (removeStudyMaterials4, 0.4f));
        createStudyMaterials5.AddFollowingStates((createStudyMaterials6, 0.6f), (removeStudyMaterials5, 0.4f));
        
        removeStudyMaterials1.AddFollowingStates((createStudyMaterials1, 1f));
        removeStudyMaterials2.AddFollowingStates((createStudyMaterials2, 1f));
        removeStudyMaterials3.AddFollowingStates((createStudyMaterials3, 1f));
        removeStudyMaterials4.AddFollowingStates((createStudyMaterials4, 1f));
        removeStudyMaterials5.AddFollowingStates((createStudyMaterials5, 1f));
        removeStudyMaterials6.AddFollowingStates((createStudyMaterials6, 1f));

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

        var addScansPositive1 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.PassExam,
            "Exam term 1",
            examScan1,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansPositive2 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.PassExam,
            "Exam term 2",
            examScan2,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansPositive3 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.PassExam,
            "Exam term 3",
            examScan3,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansPositive4 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.PassExam,
            "Exam term 4",
            examScan4,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansNegative1 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.FailExam,
            "Exam term 1",
            examScan1,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansNegative2 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.FailExam,
            "Exam term 2",
            examScan2,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansNegative3 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.FailExam,
            "Exam term 3",
            examScan3,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );
        
        var addScansNegative4 = new ReactiveState(
            EActivityType.CreateFile,
            placeholder,
            EActivityType.FailExam,
            "Exam term 4",
            examScan4,
            - TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(2)
        );

        // Teacher sprinkles
        var viewStudentRecord = new IntervalSprinkleState(
            EActivityType.VisitStudentRecord,
            studentCourseRecord,
            new TimeFrame(new DateTime(2023, 2, 12), new DateTime(2023, 6, 30))
        );

        var deleteIncompleteRopotSession = new ReactiveScenario(
            new List<EActivityType>()
            {
                EActivityType.SaveRopot, EActivityType.OpenRopot
            },
            EActivityType.SaveRopot,
            EActivityType.DeleteRopotSession
        );

        // FIXME: Instead of parsing teachers[0], there should be some general strategy
        // to tell, which actors should participate (i.e. parsing all actors and then EStrategy.First)
        var actorFrame = new ActorFrame(teachers[0], createStudyMaterials1);
        // FIXME: StateEvaluator.RunProcess() can take these parameters
        StateEvaluator.InitializeEvaluator(actorFrame, new DateTime(2023, 2, 1));
        var filledActorFrame = StateEvaluator.RunProcess(createStudyMaterials1);
        
        ReactiveStateService.RunReactiveStates(Collector.GetPreviousCollection(), teachers);
        FixedTimeStateService.RunFixedStates(teachers[0]);

        foreach (var actor in teachers)
        {
            SprinkleService.RunIntervalSprinkles(actor);
        }
        
        // TODO: Create resource for homework assignments

        // TODO: Create random 6 digit number for student IDs (start at 600000) and teacher IDs (us researchers)
        
        // TODO: When attacking, view more homeworks (10) (manual job)
    }
}