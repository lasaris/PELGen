using EventLogGenerator.GenerationLogic;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator;

public static class IsEventsGenerator
{
    public static void GenerateIsLogs(int studentsCount = 0)
    {
        if (studentsCount < 1)
        {
            throw new ArgumentException($"Invalid number of actors: students {studentsCount}");
        }

        // Setup FileManager output CSV file
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp");

        // Prepare Actors (time offset for )
        List<Actor> students = Enumerable.Range(0, studentsCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();

        // Prepare Resources
        var course = new Resource("Course X");
        var seminarGroup = new Resource("Seminar group"); // TODO: Should we differentiate between seminar groups? (YES)
        var seminarWeek1 = new Resource("Seminar week 1");
        var seminarWeek2 = new Resource("Seminar week 2");
        var seminarWeek3 = new Resource("Seminar week 3");
        var seminarWeek4 = new Resource("Seminar week 4");
        var seminarWeek5 = new Resource("Seminar week 5");
        var seminarWeek6 = new Resource("Seminar week 6");
        var materialsWeek1 = new Resource("Study materials week 1");
        var materialsWeek2 = new Resource("Study materials week 2");
        var materialsWeek3 = new Resource("Study materials week 3");
        var materialsWeek4 = new Resource("Study materials week 4");
        var materialsWeek5 = new Resource("Study materials week 5");
        var materialsWeek6 = new Resource("Study materials week 6");
        var hw1 = new Resource("Homework 1");
        var hw2 = new Resource("Homework 2");
        var hw3 = new Resource("Homework 3");
        var ropot1 = new Resource("Ropot week 1");
        var ropot2 = new Resource("Ropot week 2");
        var ropot3 = new Resource("Ropot week 3");
        var ropot4 = new Resource("Ropot week 4");
        var ropot5 = new Resource("Ropot week 5");
        var ropot6 = new Resource("Ropot week 6");
        var exam1 = new Resource("Exam term 1");
        var exam2 = new Resource("Exam term 2");
        var exam3 = new Resource("Exam term 3");

        var seminarResources = new HashSet<Resource>()
        {
            seminarWeek1, seminarWeek2, seminarWeek3, seminarWeek4, seminarWeek5, seminarWeek6,
            ropot1, ropot2, ropot3, ropot4, ropot5, ropot6
        };

        // TODO: Which data structure should I use here???
        // TODO: Offset given by seminar group
        Dictionary<HashSet<Resource>, TimeSpan> offsetSeminar1 =
            new Dictionary<HashSet<Resource>, TimeSpan>() { { seminarResources, TimeSpan.FromDays(0) } };
        Dictionary<HashSet<Resource>, TimeSpan> offsetSeminar2 =
            new Dictionary<HashSet<Resource>, TimeSpan>() { { seminarResources, TimeSpan.FromDays(1) } };
        Dictionary<HashSet<Resource>, TimeSpan> offsetSeminar3 =
            new Dictionary<HashSet<Resource>, TimeSpan>() { { seminarResources, TimeSpan.FromDays(2) } };


        // Useful properties
        var defaultChances = new StateChances();
        var seminarAttendanceChances = new StateChances(0.95f);
        var defaultStudyMaterialsChances = new StateChances(0.8f, 0.05f);
        var semesterStart = new DateTime(2023, 1, 1);
        var semesterEnd = new DateTime(2023, 3, 1);

        // Prepare states
        var enrollCourse = new ProcessState(
            EActivityType.EnrollCourse,
            course,
            new StateRules(true, 1, 0),
            defaultChances,
            new TimeFrame(new DateTime(2022, 12, 14), new DateTime(2022, 12, 24)),
            true
        );

        var enrolledCourseSet = new HashSet<ProcessState>() { enrollCourse };

        var registerSeminarGroup = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup,
            new StateRules(true, 1, 4, null, new HashSet<ProcessState>() { enrollCourse }),
            new StateChances(1, 0.05f),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );

        // var materialRules =
        //     new StateRules(false, -1, -1, new Dictionary<EActivityType, float>() { {EActivityType.AttendSeminar, 0.8f} }, enrolledCourseSet);
        //
        // var readStudyMaterials1 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek1,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 1, 2), semesterEnd)
        // );
        //
        // var readStudyMaterials2 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek2,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 1, 9), semesterEnd)
        // );
        //
        // var readStudyMaterials3 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek3,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 1, 16), semesterEnd)
        // );
        //
        // var readStudyMaterials4 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek4,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 1, 23), semesterEnd)
        // );
        //
        // var readStudyMaterials5 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek5,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 1, 30), semesterEnd)
        // );
        //
        // var readStudyMaterials6 = new ProcessState(
        //     EActivityType.ReadStudyMaterials,
        //     materialsWeek6,
        //     materialRules,
        //     defaultStudyMaterialsChances,
        //     new TimeFrame(new DateTime(2023, 2, 6), semesterEnd)
        // );

        var submitHomeworkRules = new StateRules(true, 1, 0, null, enrolledCourseSet);

        var submitHomework1 = new ProcessState(
            EActivityType.SubmitHomework,
            hw1,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 9), new DateTime(2023, 1, 15), ETimeFrameDistribution.Exponential)
        );

        var submitHomework2 = new ProcessState(
            EActivityType.SubmitHomework,
            hw2,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 23), new DateTime(2023, 1, 29), ETimeFrameDistribution.Exponential)
        );

        var submitHomework3 = new ProcessState(
            EActivityType.SubmitHomework,
            hw3,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 2, 6), new DateTime(2023, 2, 12), ETimeFrameDistribution.Exponential)
        );

        var attendSeminarRules = new StateRules(true, 1, 0, new Dictionary<EActivityType, float>()
            { { EActivityType.OpenRopot, 1f } }, enrolledCourseSet);

        var attendSeminar1 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek1,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 3, 12, 00, 00), new DateTime(2023, 1, 3, 12, 20, 00))
        );

        var attendSeminar2 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek2,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 10, 12, 00, 00), new DateTime(2023, 1, 10, 12, 20, 00))
        );

        var attendSeminar3 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek3,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 17, 12, 00, 00), new DateTime(2023, 1, 17, 12, 20, 00))
        );

        var attendSeminar4 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek4,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 24, 12, 00, 00), new DateTime(2023, 1, 24, 12, 20, 00))
        );

        var attendSeminar5 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek5,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 31, 12, 00, 00), new DateTime(2023, 1, 31, 12, 20, 00))
        );

        var attendSeminar6 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek6,
            attendSeminarRules,
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 2, 7, 12, 00, 00), new DateTime(2023, 2, 7, 12, 20, 00))
        );

        HashSet<ProcessState> attendingSeminar1Set = new HashSet<ProcessState>();
        attendingSeminar1Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar1 });

        HashSet<ProcessState> attendingSeminar2Set = new HashSet<ProcessState>();
        attendingSeminar2Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar2 });

        HashSet<ProcessState> attendingSeminar3Set = new HashSet<ProcessState>();
        attendingSeminar3Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar3 });

        HashSet<ProcessState> attendingSeminar4Set = new HashSet<ProcessState>();
        attendingSeminar4Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar4 });

        HashSet<ProcessState> attendingSeminar5Set = new HashSet<ProcessState>();
        attendingSeminar5Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar5 });

        HashSet<ProcessState> attendingSeminar6Set = new HashSet<ProcessState>();
        attendingSeminar6Set.UnionWith(new HashSet<ProcessState>() { enrollCourse, attendSeminar6 });

        var timeFrameRopot1 =
            new TimeFrame(new DateTime(2023, 1, 03, 12, 00, 00), new DateTime(2023, 1, 03, 12, 15, 00));
        var timeFrameRopot2 =
            new TimeFrame(new DateTime(2023, 1, 10, 12, 00, 00), new DateTime(2023, 1, 10, 12, 15, 00));
        var timeFrameRopot3 =
            new TimeFrame(new DateTime(2023, 1, 17, 12, 00, 00), new DateTime(2023, 1, 17, 12, 15, 00));
        var timeFrameRopot4 =
            new TimeFrame(new DateTime(2023, 1, 24, 12, 00, 00), new DateTime(2023, 1, 24, 12, 15, 00));
        var timeFrameRopot5 =
            new TimeFrame(new DateTime(2023, 1, 31, 12, 00, 00), new DateTime(2023, 1, 31, 12, 15, 00));
        var timeFrameRopot6 =
            new TimeFrame(new DateTime(2023, 2, 07, 12, 00, 00), new DateTime(2023, 2, 07, 12, 15, 00));

        var openRopotFollowing = new Dictionary<EActivityType, float>()
            { { EActivityType.SubmitRopot, 0.7f }, { EActivityType.SaveRopot, 0.3f } };
        var saveRopotFollowing = new Dictionary<EActivityType, float>()
            { { EActivityType.SubmitRopot, 0.7f }, { EActivityType.SaveRopot, 0.3f } };
        var submitRopotFollowing = new Dictionary<EActivityType, float>() { { EActivityType.ViewRopot, 0.9f } };
        var viewRopotFollowing = new Dictionary<EActivityType, float>() { { EActivityType.ReadStudyMaterials, 0.5f } };

        var openRopotChances = new StateChances(1f);
        var saveRopotChances = new StateChances(0.8f);
        var submitRopotChances = new StateChances(1f);
        var viewRopotChances = new StateChances(0.9f);

        var openRopotRulesSeminar1 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar1 });
        var openRopotRulesSeminar2 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar2 });
        var openRopotRulesSeminar3 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar3 });
        var openRopotRulesSeminar4 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar4 });
        var openRopotRulesSeminar5 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar5 });
        var openRopotRulesSeminar6 =
            new StateRules(true, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar6 });

        var openRopot1 = new ProcessState(
            EActivityType.OpenRopot,
            ropot1,
            openRopotRulesSeminar1,
            openRopotChances,
            timeFrameRopot1
        );

        var openRopot2 = new ProcessState(
            EActivityType.OpenRopot,
            ropot2,
            openRopotRulesSeminar2,
            openRopotChances,
            timeFrameRopot2
        );

        var openRopot3 = new ProcessState(
            EActivityType.OpenRopot,
            ropot3,
            openRopotRulesSeminar3,
            openRopotChances,
            timeFrameRopot3
        );

        var openRopot4 = new ProcessState(
            EActivityType.OpenRopot,
            ropot4,
            openRopotRulesSeminar4,
            openRopotChances,
            timeFrameRopot4
        );

        var openRopot5 = new ProcessState(
            EActivityType.OpenRopot,
            ropot5,
            openRopotRulesSeminar5,
            openRopotChances,
            timeFrameRopot5
        );

        var openRopot6 = new ProcessState(
            EActivityType.OpenRopot,
            ropot6,
            openRopotRulesSeminar6,
            openRopotChances,
            timeFrameRopot6
        );

        var saveRopotRulesSeminar1 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar1, openRopot1 });
        var saveRopotRulesSeminar2 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar2, openRopot2 });
        var saveRopotRulesSeminar3 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar3, openRopot3 });
        var saveRopotRulesSeminar4 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar3, openRopot4 });
        var saveRopotRulesSeminar5 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar5, openRopot5 });
        var saveRopotRulesSeminar6 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { attendSeminar6, openRopot6 });

        var submitRopotRulesSeminar1 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar1, openRopot1 });
        var submitRopotRulesSeminar2 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar2, openRopot2 });
        var submitRopotRulesSeminar3 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar3, openRopot3 });
        var submitRopotRulesSeminar4 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar4, openRopot4 });
        var submitRopotRulesSeminar5 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar5, openRopot5 });
        var submitRopotRulesSeminar6 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar6, openRopot6 });

        // TODO: FIX ropot saving after view (add states/activites that cannot be before/after each other?)
        var saveRopot1 = new ProcessState(
            EActivityType.SaveRopot,
            ropot1,
            saveRopotRulesSeminar1,
            saveRopotChances,
            timeFrameRopot1
        );

        var saveRopot2 = new ProcessState(
            EActivityType.SaveRopot,
            ropot2,
            saveRopotRulesSeminar2,
            saveRopotChances,
            timeFrameRopot2
        );

        var saveRopot3 = new ProcessState(
            EActivityType.SaveRopot,
            ropot3,
            saveRopotRulesSeminar3,
            saveRopotChances,
            timeFrameRopot3
        );

        var saveRopot4 = new ProcessState(
            EActivityType.SaveRopot,
            ropot4,
            saveRopotRulesSeminar4,
            saveRopotChances,
            timeFrameRopot4
        );

        var saveRopot5 = new ProcessState(
            EActivityType.SaveRopot,
            ropot5,
            saveRopotRulesSeminar5,
            saveRopotChances,
            timeFrameRopot5
        );

        var saveRopot6 = new ProcessState(
            EActivityType.SaveRopot,
            ropot6,
            saveRopotRulesSeminar6,
            saveRopotChances,
            timeFrameRopot6
        );

        var submitRopot1 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot1,
            submitRopotRulesSeminar1,
            submitRopotChances,
            timeFrameRopot1
        );

        var submitRopot2 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot2,
            submitRopotRulesSeminar2,
            submitRopotChances,
            timeFrameRopot2
        );

        var submitRopot3 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot3,
            submitRopotRulesSeminar3,
            submitRopotChances,
            timeFrameRopot3
        );

        var submitRopot4 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot4,
            submitRopotRulesSeminar4,
            submitRopotChances,
            timeFrameRopot4
        );

        var submitRopot5 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot5,
            submitRopotRulesSeminar5,
            submitRopotChances,
            timeFrameRopot5
        );

        var submitRopot6 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot6,
            submitRopotRulesSeminar6,
            submitRopotChances,
            timeFrameRopot6
        );

        var viewRopotRulesSeminar1 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar1, submitRopot1 });
        var viewRopotRulesSeminar2 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar2, submitRopot2 });
        var viewRopotRulesSeminar3 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar3, submitRopot3 });
        var viewRopotRulesSeminar4 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar4, submitRopot4 });
        var viewRopotRulesSeminar5 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar5, submitRopot5 });
        var viewRopotRulesSeminar6 =
            new StateRules(false, 1, 0, viewRopotFollowing,
                new HashSet<ProcessState>() { attendSeminar6, submitRopot6 });

        // TODO: FIX view ropot time frames (should be anytime after it was submitted)
        // Should this also be a random event throughout the semester?
        var viewRopot1 = new ProcessState(
            EActivityType.ViewRopot,
            ropot1,
            viewRopotRulesSeminar1,
            viewRopotChances,
            timeFrameRopot1
        );

        var viewRopot2 = new ProcessState(
            EActivityType.ViewRopot,
            ropot2,
            viewRopotRulesSeminar2,
            viewRopotChances,
            timeFrameRopot2
        );

        var viewRopot3 = new ProcessState(
            EActivityType.ViewRopot,
            ropot3,
            viewRopotRulesSeminar3,
            viewRopotChances,
            timeFrameRopot3
        );

        var viewRopot4 = new ProcessState(
            EActivityType.ViewRopot,
            ropot4,
            viewRopotRulesSeminar4,
            viewRopotChances,
            timeFrameRopot4
        );

        var viewRopot5 = new ProcessState(
            EActivityType.ViewRopot,
            ropot5,
            viewRopotRulesSeminar5,
            viewRopotChances,
            timeFrameRopot5
        );

        var viewRopot6 = new ProcessState(
            EActivityType.ViewRopot,
            ropot6,
            viewRopotRulesSeminar6,
            viewRopotChances,
            timeFrameRopot6
        );

        var examRules = new StateRules(false, 1, 0,
            new Dictionary<EActivityType, float>()
                { { EActivityType.PassCourse, 0.5f }, { EActivityType.RegisterExamTerm, 0.5f } });
        var examChances = new StateChances(0.9f);

        var registerTerm1 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam1,
            examRules,
            examChances,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 15))
        );

        var registerTerm2 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam2,
            examRules,
            examChances,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 20))
        );

        var registerTerm3 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam3,
            examRules,
            examChances,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 24))
        );

        // Finishing processes
        var passCourse = new ProcessState(
            EActivityType.PassCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new StateChances(),
            new TimeFrame(new DateTime(2023, 2, 8), semesterEnd),
            false,
            true
        );

        var failCourse = new ProcessState(
            EActivityType.FailCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new StateChances(),
            new TimeFrame(new DateTime(2023, 2, 8), semesterEnd),
            false,
            true
        );

        foreach (var student in students)
        {
            var actorFrame = new ActorFrame(student, enrollCourse);
            StateEvaluator.OnStateEnter(actorFrame.Actor, enrollCourse, actorFrame.CurrentTime);
            StateEvaluator.InitializeEvaluator(actorFrame, new DateTime(2023, 4, 1));
            StateEvaluator.RunProcess();
        }

        // TODO: Implement Good vs. Bad student Actor (it naturally happens, is it really needed?)

        // TODO: Implement variable offset for attending seminar for each student Actor

        // TODO: Implement rules for the whole scenarios, if the rules apply, process finishes? (like student missing more than 2 seminars)

        // TODO: Create process for teacher Actor

        // TODO: Implement FailExam activity
    }
}