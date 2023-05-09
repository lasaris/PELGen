using EventLogGenerator.GenerationLogic;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;
using EventLogGenerator.Services;

namespace EventLogGenerator;

public static class StudentGenerator
{
    public static void GenerateLogs(int studentsCount = 1)
    {
        if (studentsCount < 1)
        {
            throw new ArgumentException($"Invalid number of actors: students {studentsCount}");
        }

        // Setup necessary services
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp,OwnerId", "student.csv");
        IdService.ResetService();
        SprinkleService.ResetService();
        Collector.CreateCollectorMap();

        // Prepare Actors (time offset for )
        List<Actor> students = Enumerable.Range(0, studentsCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();

        // FIXME: This logic should be implemented somewhere else, not the scenario itself
        // Setup seminar offsets
        var seminarActivites = new HashSet<EActivityType>()
        {
            EActivityType.OpenRopot,
            EActivityType.SaveRopot,
            EActivityType.SubmitRopot,
            EActivityType.ReceiveAttendance,
            EActivityType.ReceiveAbsence,
        };
        var firstOffset = TimeSpan.Zero;
        var secondOffset = TimeSpan.FromHours(2);
        var thirdOffset = TimeSpan.FromHours(4);

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
        var materialsWeek1 = new Resource("/um/slides-week01.pdf");
        var materialsWeek2 = new Resource("/um/slides-week02.pdf");
        var materialsWeek3 = new Resource("/um/slides-week03.pdf");
        var materialsWeek4 = new Resource("/um/slides-week04.pdf");
        var materialsWeek5 = new Resource("/um/slides-week05.pdf");
        var materialsWeek6 = new Resource("/um/slides-week06.pdf");
        var hw1 = new Resource("/ode/homework-1.zip");
        var hw2 = new Resource("/ode/homework-2.zip");
        var hw3 = new Resource("/ode/homework-3.zip");
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
        var semesterEnd = new DateTime(2023, 3, 1);
        var examRegistrationEndTerm3 = new DateTime(2023, 2, 24);

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
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31)),
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, firstOffset);
            }
        );

        var registerSeminarGroup2 = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup2,
            new StateRules(true, 1, 0, null, new HashSet<ProcessState>() { enrollCourse }),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31)),
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, secondOffset);
            }
        );

        var registerSeminarGroup3 = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup3,
            new StateRules(true, 1, 0, null, new HashSet<ProcessState>() { enrollCourse }),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31)),
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, thirdOffset);
            }
        );

        var materialRules =
            new StateRules(false, -1, -1,
                new Dictionary<EActivityType, float>() { { EActivityType.ReceiveAttendance, 0.8f } },
                enrolledCourseSet);


        var submitHomeworkRules = new StateRules(true, -1, 0, null, enrolledCourseSet);
        var deadlineHomework1 = new DateTime(2023, 1, 15);
        var deadlineHomework2 = new DateTime(2023, 1, 29);
        var deadlineHomework3 = new DateTime(2023, 2, 12);

        var submitHomework1 = new ProcessState(
            EActivityType.CreateFile,
            hw1,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 9), deadlineHomework1, ETimeFrameDistribution.Exponential)
        );

        var submitHomework2 = new ProcessState(
            EActivityType.CreateFile,
            hw2,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 23), deadlineHomework2, ETimeFrameDistribution.Exponential)
        );

        var submitHomework3 = new ProcessState(
            EActivityType.CreateFile,
            hw3,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 2, 6), deadlineHomework3, ETimeFrameDistribution.Exponential)
        );

        var removeHomework1 = new ProcessState(
            EActivityType.DeleteFile,
            hw1,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 9), new DateTime(2023, 1, 15))
        );

        var removeHomework2 = new ProcessState(
            EActivityType.DeleteFile,
            hw2,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 23), new DateTime(2023, 1, 29))
        );

        var removeHomework3 = new ProcessState(
            EActivityType.DeleteFile,
            hw3,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 2, 6), new DateTime(2023, 2, 12))
        );

        var readHomework1 = new ProcessState(
            EActivityType.ReadFile,
            hw1,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 9), new DateTime(2023, 1, 15))
        );

        var readHomework2 = new ProcessState(
            EActivityType.ReadFile,
            hw2,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 1, 23), new DateTime(2023, 1, 29))
        );

        var readHomework3 = new ProcessState(
            EActivityType.ReadFile,
            hw3,
            submitHomeworkRules,
            new TimeFrame(new DateTime(2023, 2, 6), new DateTime(2023, 2, 12))
        );

        var timeFrameRopot1 =
            new TimeFrame(new DateTime(2023, 1, 03, 12, 4, 00), new DateTime(2023, 1, 03, 12, 5, 00));
        var timeFrameRopot2 =
            new TimeFrame(new DateTime(2023, 1, 10, 12, 4, 00), new DateTime(2023, 1, 10, 12, 5, 00));
        var timeFrameRopot3 =
            new TimeFrame(new DateTime(2023, 1, 17, 12, 4, 00), new DateTime(2023, 1, 17, 12, 5, 00));
        var timeFrameRopot4 =
            new TimeFrame(new DateTime(2023, 1, 24, 12, 4, 00), new DateTime(2023, 1, 24, 12, 5, 00));
        var timeFrameRopot5 =
            new TimeFrame(new DateTime(2023, 1, 31, 12, 4, 00), new DateTime(2023, 1, 31, 12, 5, 00));
        var timeFrameRopot6 =
            new TimeFrame(new DateTime(2023, 2, 07, 12, 4, 00), new DateTime(2023, 2, 07, 12, 5, 00));

        var openRopotFollowing = new Dictionary<EActivityType, float>()
            { { EActivityType.SubmitRopot, 0.7f }, { EActivityType.SaveRopot, 0.3f } };
        var saveRopotFollowing = new Dictionary<EActivityType, float>()
            { { EActivityType.SubmitRopot, 0.6f }, { EActivityType.SaveRopot, 0.4f } };
        var submitRopotFollowing = new Dictionary<EActivityType, float>() { { EActivityType.ViewRopot, 0.9f } };

        var openRopotRulesSeminar1 =
            new StateRules(true, 1, 0, openRopotFollowing);
        var openRopotRulesSeminar2 =
            new StateRules(true, 1, 0, openRopotFollowing);
        var openRopotRulesSeminar3 =
            new StateRules(true, 1, 0, openRopotFollowing);
        var openRopotRulesSeminar4 =
            new StateRules(true, 1, 0, openRopotFollowing);
        var openRopotRulesSeminar5 =
            new StateRules(true, 1, 0, openRopotFollowing);
        var openRopotRulesSeminar6 =
            new StateRules(true, 1, 0, openRopotFollowing);

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
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot1 });
        var saveRopotRulesSeminar2 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot2 });
        var saveRopotRulesSeminar3 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot3 });
        var saveRopotRulesSeminar4 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot4 });
        var saveRopotRulesSeminar5 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot5 });
        var saveRopotRulesSeminar6 =
            new StateRules(false, 1, 3, saveRopotFollowing, new HashSet<ProcessState>() { openRopot6 });

        var submitRopotRulesSeminar1 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot1 });
        var submitRopotRulesSeminar2 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot2 });
        var submitRopotRulesSeminar3 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot3 });
        var submitRopotRulesSeminar4 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot4 });
        var submitRopotRulesSeminar5 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot5 });
        var submitRopotRulesSeminar6 =
            new StateRules(true, 1, 0, submitRopotFollowing,
                new HashSet<ProcessState>() { openRopot6 });

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
            timeFrameRopot1.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );

        var submitRopot2 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot2,
            submitRopotRulesSeminar2,
            timeFrameRopot2.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );

        var submitRopot3 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot3,
            submitRopotRulesSeminar3,
            timeFrameRopot3.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );

        var submitRopot4 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot4,
            submitRopotRulesSeminar4,
            timeFrameRopot4.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );

        var submitRopot5 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot5,
            submitRopotRulesSeminar5,
            timeFrameRopot5.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );

        var submitRopot6 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot6,
            submitRopotRulesSeminar6,
            timeFrameRopot6.GetTimeFrameWithOffset(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        );
        
        // Implement auto save of ropots
        var autoSavePeriod = TimeSpan.FromMinutes(1);
        
        var autoSaveRopot1 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot1,
            new HashSet<ProcessState>(){openRopot1},
            new HashSet<ProcessState>(){submitRopot1},
            autoSavePeriod
        );
        
        var autoSaveRopot2 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot2,
            new HashSet<ProcessState>(){openRopot2},
            new HashSet<ProcessState>(){submitRopot2},
            autoSavePeriod
        );
        
        var autoSaveRopot3 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot3,
            new HashSet<ProcessState>(){openRopot3},
            new HashSet<ProcessState>(){submitRopot3},
            autoSavePeriod
        );
        
        var autoSaveRopot4 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot4,
            new HashSet<ProcessState>(){openRopot4},
            new HashSet<ProcessState>(){submitRopot4},
            autoSavePeriod
        );
        
        var autoSaveRopot5 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot5,
            new HashSet<ProcessState>(){openRopot5},
            new HashSet<ProcessState>(){submitRopot5},
            autoSavePeriod
        );
        
        var autoSaveRopot6 = new PeriodicSprinkleState(
            EActivityType.SaveRopot,
            ropot6,
            new HashSet<ProcessState>(){openRopot6},
            new HashSet<ProcessState>(){submitRopot6},
            autoSavePeriod
        );

        var examRules = new StateRules(false, 1, 0,
            new Dictionary<EActivityType, float>()
                { { EActivityType.PassCourse, 0.5f }, { EActivityType.RegisterExamTerm, 0.5f } });

        var registerTerm1 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam1,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 19))
        );

        var registerTerm2 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam2,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 22))
        );

        var registerTerm3 = new ProcessState(
            EActivityType.RegisterExamTerm,
            exam3,
            examRules,
            new TimeFrame(new DateTime(2023, 2, 11), new DateTime(2023, 2, 26))
        );

        var failExam1 = new ProcessState(
            EActivityType.FailExam,
            exam1,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm1 }),
            new TimeFrame(new DateTime(2023, 2, 21, 12, 0, 0), new DateTime(2023, 2, 21, 12, 5, 0)));

        var failExam2 = new ProcessState(
            EActivityType.FailExam,
            exam2,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm2 }),
            new TimeFrame(new DateTime(2023, 2, 24, 12, 0, 0), new DateTime(2023, 2, 24, 12, 5, 0)));

        var failExam3 = new ProcessState(
            EActivityType.FailExam,
            exam3,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm3 }),
            new TimeFrame(new DateTime(2023, 2, 28, 12, 0, 0), new DateTime(2023, 2, 28, 12, 5, 0)));
        
        var passExam1 = new ProcessState(
            EActivityType.PassExam,
            exam1,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm1 }),
            new TimeFrame(new DateTime(2023, 2, 21, 12, 0, 0), new DateTime(2023, 2, 21, 12, 5, 0)));

        var passExam2 = new ProcessState(
            EActivityType.PassExam,
            exam2,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm2 }),
            new TimeFrame(new DateTime(2023, 2, 24, 12, 0, 0), new DateTime(2023, 2, 24, 12, 5, 0)));

        var passExam3 = new ProcessState(
            EActivityType.PassExam,
            exam3,
            new StateRules(false, 1, 0, null, new HashSet<ProcessState>() { registerTerm3 }),
            new TimeFrame(new DateTime(2023, 2, 28, 12, 0, 0), new DateTime(2023, 2, 28, 12, 5, 0)));

        // Finishing processes
        var passCourse = new ProcessState(
            EActivityType.PassCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new TimeFrame(examRegistrationEndTerm3, semesterEnd),
            true
        );

        var failCourse = new ProcessState(
            EActivityType.FailCourse,
            course,
            new StateRules(true, 1, 0, null, enrolledCourseSet,
                new HashSet<EActivityType>() { EActivityType.RegisterExamTerm }),
            new TimeFrame(examRegistrationEndTerm3, semesterEnd),
            true
        );

        // Setup following states map
        enrollCourse.AddFollowingStates((registerSeminarGroup1, 1 / 3f), (registerSeminarGroup2, 1 / 3f),
            (registerSeminarGroup3, 1 / 3f));
        registerSeminarGroup1.AddFollowingStates((registerSeminarGroup2, 0.04f), (registerSeminarGroup3, 0.04f),
            (openRopot1, 0.85f), (openRopot2, 0.05f));
        registerSeminarGroup2.AddFollowingStates((registerSeminarGroup1, 0.04f), (registerSeminarGroup3, 0.04f),
            (openRopot1, 0.85f), (openRopot2, 0.05f));
        registerSeminarGroup3.AddFollowingStates((registerSeminarGroup1, 0.04f), (registerSeminarGroup2, 0.04f),
            (openRopot1, 0.85f), (openRopot2, 0.05f));
        // ropot 1
        openRopot1.AddFollowingStates((saveRopot1, 0.8f), (submitRopot1, 0.2f));
        saveRopot1.AddFollowingStates((submitRopot1, 0.85f), (saveRopot1, 0.15f));
        submitRopot1.AddFollowingStates((openRopot2, 0.90f), (submitHomework1, 0.05f), (openRopot3, 0.05f));
        // ropot 2
        openRopot2.AddFollowingStates((saveRopot2, 0.8f), (submitRopot2, 0.2f));
        saveRopot2.AddFollowingStates((submitRopot2, 0.85f), (saveRopot2, 0.15f));
        submitRopot2.AddFollowingStates((submitHomework1, 0.90f), (openRopot3, 0.05f), (openRopot4, 0.05f));
        // hw 1
        submitHomework1.AddFollowingStates((openRopot3, 0.75f), (readHomework1, 0.2f), (openRopot4, 0.05f));
        readHomework1.AddFollowingStates((openRopot3, 0.8f), (removeHomework1, 0.2f));
        removeHomework1.AddFollowingStates((submitHomework1, 1f));
        // ropot 3
        openRopot3.AddFollowingStates((saveRopot3, 0.8f), (submitRopot3, 0.2f));
        saveRopot3.AddFollowingStates((submitRopot3, 0.85f), (saveRopot3, 0.15f));
        submitRopot3.AddFollowingStates((openRopot4, 0.90f), (submitHomework2, 0.05f), (openRopot5, 0.05f));
        // ropot 4
        openRopot4.AddFollowingStates((saveRopot4, 0.8f), (submitRopot4, 0.2f));
        saveRopot4.AddFollowingStates((submitRopot4, 0.85f), (saveRopot4, 0.15f));
        submitRopot4.AddFollowingStates((submitHomework2, 0.90f), (openRopot5, 0.05f), (openRopot6, 0.05f));
        // hw 2
        submitHomework2.AddFollowingStates((openRopot5, 0.75f), (readHomework2, 0.2f), (openRopot6, 0.05f));
        readHomework2.AddFollowingStates((openRopot5, 0.8f), (removeHomework2, 0.2f));
        removeHomework2.AddFollowingStates((submitHomework2, 1f));
        // ropot 5
        openRopot5.AddFollowingStates((saveRopot5, 0.8f), (submitRopot5, 0.2f));
        saveRopot5.AddFollowingStates((submitRopot5, 0.85f), (saveRopot5, 0.15f));
        submitRopot5.AddFollowingStates((openRopot6, 0.90f), (submitHomework3, 0.05f), (registerTerm1, 0.05f));
        // ropot 6
        openRopot6.AddFollowingStates((saveRopot6, 0.8f), (submitRopot6, 0.2f));
        saveRopot6.AddFollowingStates((submitRopot6, 0.85f), (saveRopot6, 0.15f));
        submitRopot6.AddFollowingStates((submitHomework3, 0.95f), (registerTerm1, 0.05f));
        // hw 3
        submitHomework3.AddFollowingStates((registerTerm1, 0.65f), (registerTerm2, 0.05f), (registerTerm3, 0.05f), (readHomework3, 0.2f));
        readHomework3.AddFollowingStates((registerTerm1, 0.65f), (registerTerm2, 0.05f), (registerTerm3, 0.05f), (removeHomework3, 0.2f));
        removeHomework3.AddFollowingStates((submitHomework3, 1f));

        // exams
        registerTerm1.AddFollowingStates((passExam1, 0.65f), (failExam1, 0.35f));
        registerTerm2.AddFollowingStates((passExam2, 0.65f), (failExam2, 0.35f));
        registerTerm3.AddFollowingStates((passExam3, 0.65f), (failExam3, 0.35f));
        failExam1.AddFollowingStates((registerTerm2, 0.9f), (failCourse, 0.1f));
        failExam2.AddFollowingStates((registerTerm3, 0.9f), (failCourse, 0.1f));
        failExam3.AddFollowingStates((failCourse, 1f));
        passExam1.AddFollowingStates((passCourse, 1f));
        passExam2.AddFollowingStates((passCourse, 1f));
        passExam3.AddFollowingStates((passCourse, 1f));

        var endCourseSet = new HashSet<ProcessState>() { passCourse, failCourse };

        var openRopotSet = new HashSet<ProcessState>()
            { openRopot1, openRopot2, openRopot3, openRopot4, openRopot5, openRopot6 };
        var submitRopotSet = new HashSet<ProcessState>()
            { submitRopot1, submitRopot2, submitRopot3, submitRopot4, submitRopot5, submitRopot6 };

        // Create sprinkles
        var readStudyMaterials1 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek1,
            new TimeFrame(new DateTime(2023, 1, 2), examRegistrationEndTerm3)
        );
        
        var readStudyMaterials2 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek2,
            new TimeFrame(new DateTime(2023, 1, 9), examRegistrationEndTerm3)
        );
        
        var readStudyMaterials3 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek3,
            new TimeFrame(new DateTime(2023, 1, 16), examRegistrationEndTerm3)
        );
        
        var readStudyMaterials4 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek4,
            new TimeFrame(new DateTime(2023, 1, 23), examRegistrationEndTerm3)
        );
        
        var readStudyMaterials5 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek5,
            new TimeFrame(new DateTime(2023, 1, 30), examRegistrationEndTerm3)
        );
        
        var readStudyMaterials6 = new IntervalSprinkleState(
            EActivityType.ReadFile,
            materialsWeek6,
            new TimeFrame(new DateTime(2023, 2, 6), examRegistrationEndTerm3)
        );
        
        var viewRopot1 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot1,
            new HashSet<ProcessState>() { submitRopot1 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        var viewRopot2 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot2,
            new HashSet<ProcessState>() { submitRopot2 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        var viewRopot3 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot3,
            new HashSet<ProcessState>() { submitRopot3 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        var viewRopot4 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot4,
            new HashSet<ProcessState>() { submitRopot4 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        var viewRopot5 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot5,
            new HashSet<ProcessState>() { submitRopot5 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        var viewRopot6 = new SprinkleState(
            EActivityType.ViewRopot,
            ropot6,
            new HashSet<ProcessState>() { submitRopot6 },
            endCourseSet,
            openRopotSet,
            submitRopotSet
        );

        // TODO: Register offset for each seminar group. Each student in seminar group should receive homework at the same time.
        var receivePointsHomework1 = new FixedTimeState(
            EActivityType.ReceivePoints,
            hw1,
            deadlineHomework1 + TimeSpan.FromDays(5),
            submitHomework1
            );

        var receivePointsHomework2 = new FixedTimeState(
            EActivityType.ReceivePoints,
            hw2,
            deadlineHomework2 + TimeSpan.FromDays(5),
            submitHomework2
        );

        var receivePointsHomework3 = new FixedTimeState(
            EActivityType.ReceivePoints,
            hw3,
            deadlineHomework3 + TimeSpan.FromDays(5),
            submitHomework3
        );
        
        // Scenario sprinkles
        var internetFailureRopot1 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot1 },
            new HashSet<ProcessState>(){ submitRopot1 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot1, TimeSpan.FromMinutes(1)),
                (openRopot1, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        var internetFailureRopot2 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot2 },
            new HashSet<ProcessState>(){ submitRopot2 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot2, TimeSpan.FromMinutes(1)),
                (openRopot2, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        var internetFailureRopot3 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot3 },
            new HashSet<ProcessState>(){ submitRopot3 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot3, TimeSpan.FromMinutes(1)),
                (openRopot3, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        var internetFailureRopot4 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot4 },
            new HashSet<ProcessState>(){ submitRopot4 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot4, TimeSpan.FromMinutes(1)),
                (openRopot4, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        var internetFailureRopot5 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot5 },
            new HashSet<ProcessState>(){ submitRopot5 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot5, TimeSpan.FromMinutes(1)),
                (openRopot5, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        var internetFailureRopot6 = new ScenarioSetSprinkle(
            new HashSet<ProcessState>(){ openRopot6 },
            new HashSet<ProcessState>(){ submitRopot6 },
            new List<(ABaseState, TimeSpan)>()
            {
                (saveRopot6, TimeSpan.FromMinutes(1)),
                (openRopot6, TimeSpan.FromMinutes(1)),
            },
            0.04f
        );
        
        // Conditional sprinkles for attendance marking
        var attendanceSeminar1 = new ConditionalSprinkle(
            openRopot1,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek1),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek1),
            TimeSpan.FromHours(1)
        );
        
        var attendanceSeminar2 = new ConditionalSprinkle(
            openRopot2,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek2),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek2),
            TimeSpan.FromHours(1)
        );
        
        var attendanceSeminar3 = new ConditionalSprinkle(
            openRopot3,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek3),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek3),
            TimeSpan.FromHours(1)
        );
        
        var attendanceSeminar4 = new ConditionalSprinkle(
            openRopot4,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek4),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek4),
            TimeSpan.FromHours(1)
        );
        
        var attendanceSeminar5 = new ConditionalSprinkle(
            openRopot5,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek5),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek5),
            TimeSpan.FromHours(1)
        );
        
        var attendanceSeminar6 = new ConditionalSprinkle(
            openRopot6,
            new DummyState(EActivityType.ReceiveAttendance, seminarWeek6),
            new DummyState(EActivityType.ReceiveAbsence, seminarWeek6),
            TimeSpan.FromHours(1)
        );

        foreach (var student in students)
        {
            var actorFrame = new ActorFrame(student, enrollCourse);
            // FIXME: StateEvaluator.RunProcess() can take these parameters
            StateEvaluator.InitializeEvaluator(actorFrame, new DateTime(2023, 4, 1));
            var filledActorFrame = StateEvaluator.RunProcess(enrollCourse);
            SprinkleService.RunSprinkling(filledActorFrame);
            FixedTimeStateService.RunFixedStatesWithPreceding(filledActorFrame);
        }

        // TODO: Implement rules for the whole scenarios, if the rules apply, process finishes? (like student missing more than 2 seminars)
    }
}