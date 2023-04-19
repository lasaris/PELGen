using EventLogGenerator.GenerationLogic;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator;

public static class IsEventsGenerator
{
    public static void GenerateIsLogs(int studentsCount = 0)
    {
        if (studentsCount < 1)
        {
            throw new ArgumentException($"Invalid number of actors: students {studentsCount}");
        }

        // Setup necessary services
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp");
        SprinkleService.ResetSprinklerState();

        // Prepare Actors (time offset for )
        List<Actor> students = Enumerable.Range(0, studentsCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();
        
        // Setup seminar offsets
        var seminarActivites = new HashSet<EActivityType>()
        {
            EActivityType.AttendSeminar,
            EActivityType.OpenRopot,
            EActivityType.SaveRopot,
            EActivityType.SubmitRopot,
            EActivityType.ViewRopot
        };
        int third = students.Count / 3;
        var firstOffset = TimeSpan.Zero;
        List<Actor> firstThird = students.GetRange(0, third);
        var secondOffset = TimeSpan.FromDays(1);
        List<Actor> secondThird = students.GetRange(third, third);
        var thirdOffset = TimeSpan.FromDays(2);
        List<Actor> thirdThird = students.GetRange(third * 2, students.Count - third * 2);
        
        // Set the offset on each group of students
        foreach (Actor student in firstThird)
        {
            ActorService.RegisterActivitiesOffset(student, seminarActivites, firstOffset);
        }

        foreach (Actor student in secondThird)
        {
            ActorService.RegisterActivitiesOffset(student, seminarActivites, secondOffset);
        }

        foreach (Actor student in thirdThird)
        {
            ActorService.RegisterActivitiesOffset(student, seminarActivites, thirdOffset);
        }
        
        // Prepare Resources
        var course = new Resource("Our Course");
        var seminarGroup1 = new Resource("Seminar group 1");
        var seminarGroup2 = new Resource("Seminar group 2");
        var seminarGroup3 = new Resource("Seminar group 3");
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

        // Useful properties
        var semesterStart = new DateTime(2023, 1, 1);
        var semesterEnd = new DateTime(2023, 3, 1);

        // Prepare states
        var enrollCourse = new ProcessState(
            EActivityType.EnrollCourse,
            course,
            new StateRules(true, 1, 0),
            new TimeFrame(new DateTime(2022, 12, 14), new DateTime(2022, 12, 24))
        );

        var enrolledCourseSet = new HashSet<ProcessState>() { enrollCourse };

        var registerSeminarGroup1 = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup1,
            new StateRules(true, 1, 0, null, new HashSet<ProcessState>() { enrollCourse }),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );
        
        var registerSeminarGroup2 = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup2,
            new StateRules(true, 1, 0, null, new HashSet<ProcessState>() { enrollCourse }),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );
        
        var registerSeminarGroup3 = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup3,
            new StateRules(true, 1, 0, null, new HashSet<ProcessState>() { enrollCourse }),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );

        var materialRules =
            new StateRules(false, -1, -1, new Dictionary<EActivityType, float>() { {EActivityType.AttendSeminar, 0.8f} }, enrolledCourseSet);
        
        

        var submitHomeworkRules = new StateRules(true, 1, 0, null, enrolledCourseSet);

        var submitHomework1 = new ProcessState(
            EActivityType.SubmitHomework,
            hw1,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 9), new DateTime(2023, 1, 15), ETimeFrameDistribution.Exponential)
        );

        var submitHomework2 = new ProcessState(
            EActivityType.SubmitHomework,
            hw2,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 23), new DateTime(2023, 1, 29), ETimeFrameDistribution.Exponential)
        );

        var submitHomework3 = new ProcessState(
            EActivityType.SubmitHomework,
            hw3,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 2, 6), new DateTime(2023, 2, 12), ETimeFrameDistribution.Exponential)
        );

        var attendSeminarRules = new StateRules(true, 1, 0, new Dictionary<EActivityType, float>()
            { { EActivityType.OpenRopot, 1f } }, enrolledCourseSet);

        var attendSeminar1 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek1,
            attendSeminarRules,
            new TimeFrame(new DateTime(2023, 1, 3, 12, 00, 00), new DateTime(2023, 1, 3, 12, 20, 00))
        );

        var attendSeminar2 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek2,
            attendSeminarRules,
            new TimeFrame(new DateTime(2023, 1, 10, 12, 00, 00), new DateTime(2023, 1, 10, 12, 20, 00))
        );

        var attendSeminar3 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek3,
            attendSeminarRules,
            new TimeFrame(new DateTime(2023, 1, 17, 12, 00, 00), new DateTime(2023, 1, 17, 12, 20, 00))
        );

        var attendSeminar4 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek4,
            attendSeminarRules,
            new TimeFrame(new DateTime(2023, 1, 24, 12, 00, 00), new DateTime(2023, 1, 24, 12, 20, 00))
        );

        var attendSeminar5 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek5,
            attendSeminarRules,
            new TimeFrame(new DateTime(2023, 1, 31, 12, 00, 00), new DateTime(2023, 1, 31, 12, 20, 00))
        );

        var attendSeminar6 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek6,
            attendSeminarRules,
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
            timeFrameRopot1
        );

        var openRopot2 = new ProcessState(
            EActivityType.OpenRopot,
            ropot2,
            openRopotRulesSeminar2,
            timeFrameRopot2
        );

        var openRopot3 = new ProcessState(
            EActivityType.OpenRopot,
            ropot3,
            openRopotRulesSeminar3,
            timeFrameRopot3
        );

        var openRopot4 = new ProcessState(
            EActivityType.OpenRopot,
            ropot4,
            openRopotRulesSeminar4,
            timeFrameRopot4
        );

        var openRopot5 = new ProcessState(
            EActivityType.OpenRopot,
            ropot5,
            openRopotRulesSeminar5,
            timeFrameRopot5
        );

        var openRopot6 = new ProcessState(
            EActivityType.OpenRopot,
            ropot6,
            openRopotRulesSeminar6,
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
        
        var saveRopot1 = new ProcessState(
            EActivityType.SaveRopot,
            ropot1,
            saveRopotRulesSeminar1,
            timeFrameRopot1
        );

        var saveRopot2 = new ProcessState(
            EActivityType.SaveRopot,
            ropot2,
            saveRopotRulesSeminar2,
            timeFrameRopot2
        );

        var saveRopot3 = new ProcessState(
            EActivityType.SaveRopot,
            ropot3,
            saveRopotRulesSeminar3,
            timeFrameRopot3
        );

        var saveRopot4 = new ProcessState(
            EActivityType.SaveRopot,
            ropot4,
            saveRopotRulesSeminar4,
            timeFrameRopot4
        );

        var saveRopot5 = new ProcessState(
            EActivityType.SaveRopot,
            ropot5,
            saveRopotRulesSeminar5,
            timeFrameRopot5
        );

        var saveRopot6 = new ProcessState(
            EActivityType.SaveRopot,
            ropot6,
            saveRopotRulesSeminar6,
            timeFrameRopot6
        );

        var submitRopot1 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot1,
            submitRopotRulesSeminar1,
            timeFrameRopot1
        );

        var submitRopot2 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot2,
            submitRopotRulesSeminar2,
            timeFrameRopot2
        );

        var submitRopot3 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot3,
            submitRopotRulesSeminar3,
            timeFrameRopot3
        );

        var submitRopot4 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot4,
            submitRopotRulesSeminar4,
            timeFrameRopot4
        );

        var submitRopot5 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot5,
            submitRopotRulesSeminar5,
            timeFrameRopot5
        );

        var submitRopot6 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot6,
            submitRopotRulesSeminar6,
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

        var examRules = new StateRules(false, 1, 0,
            new Dictionary<EActivityType, float>()
                { { EActivityType.PassCourse, 0.5f }, { EActivityType.RegisterExamTerm, 0.5f } });

        var registerTerm1 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam1,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 15))
        );

        var registerTerm2 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam2,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 20))
        );

        var registerTerm3 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam3,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 24))
        );
        
        var failExam1 = new ProcessState(
            EActivityType.FailExam,
            exam1,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm1 }),
            new TimeFrame(new DateTime(2023, 2, 16), new DateTime(2023, 2, 20)));
        
        var failExam2 = new ProcessState(
            EActivityType.FailExam,
            exam2,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm2 }),
            new TimeFrame(new DateTime(2023, 2, 21), new DateTime(2023, 2, 24)));
        
        var failExam3 = new ProcessState(
            EActivityType.FailExam,
            exam3,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm3 }),
            new TimeFrame(new DateTime(2023, 2, 25), new DateTime(2023, 2, 28)));
        
        // Finishing processes
        var passCourse = new ProcessState(
            EActivityType.PassCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new TimeFrame(new DateTime(2023, 2, 8), semesterEnd),
            true
        );

        var failCourse = new ProcessState(
            EActivityType.FailCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new TimeFrame(new DateTime(2023, 2, 8), semesterEnd),
            true
        );
        
        // Setup following states map
        enrollCourse.AddFollowingStates((registerSeminarGroup1, 1/3f), (registerSeminarGroup2, 1/3f), (registerSeminarGroup3, 1/3f));
        registerSeminarGroup1.AddFollowingStates((registerSeminarGroup2, 0.04f), (registerSeminarGroup3, 0.04f), (openRopot1, 0.92f));
        registerSeminarGroup2.AddFollowingStates((registerSeminarGroup1, 0.04f), (registerSeminarGroup3, 0.04f), (openRopot1, 0.92f));
        registerSeminarGroup3.AddFollowingStates((registerSeminarGroup1, 0.04f), (registerSeminarGroup2, 0.04f), (openRopot1, 0.92f));
        // ropot 1
        openRopot1.AddFollowingStates((saveRopot1, 0.8f), (submitRopot1, 0.2f));
        saveRopot1.AddFollowingStates((submitRopot1, 0.85f),(saveRopot1, 0.15f));
        submitRopot1.AddFollowingStates((openRopot2, 1f));
        // ropot 2
        openRopot2.AddFollowingStates((saveRopot2, 0.8f), (submitRopot2, 0.2f));
        saveRopot2.AddFollowingStates((submitRopot2, 0.85f),(saveRopot2, 0.15f));
        submitRopot2.AddFollowingStates((submitHomework1, 1f));
        // hw 1
        submitHomework1.AddFollowingStates((openRopot3, 1f));
        // ropot 3
        openRopot3.AddFollowingStates((saveRopot3, 0.8f), (submitRopot3, 0.2f));
        saveRopot3.AddFollowingStates((submitRopot3, 0.85f),(saveRopot3, 0.15f));
        submitRopot3.AddFollowingStates((openRopot4, 1f));
        // ropot 4
        openRopot4.AddFollowingStates((saveRopot4, 0.8f), (submitRopot4, 0.2f));
        saveRopot4.AddFollowingStates((submitRopot4, 0.85f),(saveRopot4, 0.15f));
        submitRopot4.AddFollowingStates((submitHomework2, 1f));
        // hw 2
        submitHomework2.AddFollowingStates((openRopot5, 1f));
        // ropot 5
        openRopot5.AddFollowingStates((saveRopot5, 0.8f), (submitRopot5, 0.2f));
        saveRopot5.AddFollowingStates((submitRopot5, 0.85f),(saveRopot5, 0.15f));
        submitRopot5.AddFollowingStates((openRopot6, 1f));
        // ropot 6
        openRopot6.AddFollowingStates((saveRopot6, 0.8f), (submitRopot6, 0.2f));
        saveRopot6.AddFollowingStates((submitRopot6, 0.85f),(saveRopot6, 0.15f));
        submitRopot6.AddFollowingStates((submitHomework3, 1f));
        // hw 3
        submitHomework3.AddFollowingStates((registerTerm1, 0.9f), (registerTerm2, 0.05f), (registerTerm3, 0.05f));
        // exams
        registerTerm1.AddFollowingStates((passCourse, 0.65f), (failExam1, 0.30f), (failCourse, 0.05f));
        registerTerm2.AddFollowingStates((passCourse, 0.65f), (failExam2, 0.30f), (failCourse, 0.05f));
        registerTerm3.AddFollowingStates((passCourse, 0.65f), (failExam3, 0.35f));
        failExam1.AddFollowingStates((registerTerm2, 0.9f), (failCourse, 0.1f));
        failExam2.AddFollowingStates((registerTerm3, 0.9f), (failCourse, 0.1f));
        failExam3.AddFollowingStates((failCourse, 1f));
        
        // TODO: Register sprinkles here
        var readStudyMaterials1 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek1,
            materialRules,
            new TimeFrame(new DateTime(2023, 1, 2), semesterEnd)
        );
        
        var readStudyMaterials2 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek2,
            materialRules,
            new TimeFrame(new DateTime(2023, 1, 9), semesterEnd)
        );
        
        var readStudyMaterials3 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek3,
            materialRules,
            new TimeFrame(new DateTime(2023, 1, 16), semesterEnd)
        );
        
        var readStudyMaterials4 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek4,
            materialRules,
            new TimeFrame(new DateTime(2023, 1, 23), semesterEnd)
        );
        
        var readStudyMaterials5 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek5,
            materialRules,
            new TimeFrame(new DateTime(2023, 1, 30), semesterEnd)
        );
        
        var readStudyMaterials6 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek6,
            materialRules,
            new TimeFrame(new DateTime(2023, 2, 6), semesterEnd)
        );
        
        // TODO: FIX view ropot time frames (should be anytime after it was submitted)
        // Should this also be a random event throughout the semester?
        var viewRopot1 = new ProcessState(
            EActivityType.ViewRopot,
            ropot1,
            viewRopotRulesSeminar1,
            timeFrameRopot1
        );
        
        var viewRopot2 = new ProcessState(
            EActivityType.ViewRopot,
            ropot2,
            viewRopotRulesSeminar2,
            timeFrameRopot2
        );
        
        var viewRopot3 = new ProcessState(
            EActivityType.ViewRopot,
            ropot3,
            viewRopotRulesSeminar3,
            timeFrameRopot3
        );
        
        var viewRopot4 = new ProcessState(
            EActivityType.ViewRopot,
            ropot4,
            viewRopotRulesSeminar4,
            timeFrameRopot4
        );
        
        var viewRopot5 = new ProcessState(
            EActivityType.ViewRopot,
            ropot5,
            viewRopotRulesSeminar5,
            timeFrameRopot5
        );
        
        var viewRopot6 = new ProcessState(
            EActivityType.ViewRopot,
            ropot6,
            viewRopotRulesSeminar6,
            timeFrameRopot6
        );
        
        foreach (var student in students)
        {
            var actorFrame = new ActorFrame(student, enrollCourse);
            StateEvaluator.OnStateEnter(actorFrame.Actor, enrollCourse, actorFrame.CurrentTime);
            StateEvaluator.InitializeEvaluator(actorFrame, new DateTime(2023, 4, 1));
            StateEvaluator.RunProcess();
        }

        // TODO: Implement Good vs. Bad student Actor

        // TODO: Implement rules for the whole scenarios, if the rules apply, process finishes? (like student missing more than 2 seminars)

        // TODO: Create process for teacher Actor
    }
}