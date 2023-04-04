﻿using EventLogGenerator.InputOutput;
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
        FileManager.SetupNewCsvFile("CaseId,Activity,ActorId,ActorType,Resource,StartTimestamp,EndTimestamp");

        // Prepare Actors
        List<Actor> students = Enumerable.Range(0, studentsCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();

        // Prepare Resources
        var course = new Resource("Course X");
        var seminarGroup = new Resource("Seminar group 1");
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

        // Useful properties
        var defaultChances = new StateChances();
        var seminarAttendanceChances = new StateChances(0.95f);
        var defaultStudyMaterialsChances = new StateChances(0.8f, 0.05f);
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

        var materialRules =
            new StateRules(false, -1, -1, new[] { (EActivityType.AttendSeminar, 0.8f) }, enrolledCourseSet);

        var readStudyMaterials1 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek1,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 1, 2), semesterEnd)
        );

        var readStudyMaterials2 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek2,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 1, 9), semesterEnd)
        );

        var readStudyMaterials3 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek3,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 1, 16), semesterEnd)
        );

        var readStudyMaterials4 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek4,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 1, 23), semesterEnd)
        );

        var readStudyMaterials5 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek5,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 1, 30), semesterEnd)
        );

        var readStudyMaterials6 = new ProcessState(
            EActivityType.ReadStudyMaterials,
            materialsWeek6,
            materialRules,
            defaultStudyMaterialsChances,
            new TimeFrame(new DateTime(2023, 2, 6), semesterEnd)
        );

        var submitHomeworkRules = new StateRules(false, 1, 0, null, enrolledCourseSet);

        var submitHomework1 = new ProcessState(
            EActivityType.SubmitHomework,
            hw1,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 2), new DateTime(2023, 1, 15), ETimeFrameDistribution.Exponential)
        );

        var submitHomework2 = new ProcessState(
            EActivityType.SubmitHomework,
            hw2,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 16), new DateTime(2023, 1, 29), ETimeFrameDistribution.Exponential)
        );

        var submitHomework3 = new ProcessState(
            EActivityType.SubmitHomework,
            hw3,
            submitHomeworkRules,
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 30), new DateTime(2023, 2, 12), ETimeFrameDistribution.Exponential)
        );

        var attendSeminarRules = new StateRules(false, 1, 0, null, enrolledCourseSet);

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

        var timeFrameRopot1 = new TimeFrame(new DateTime(2023, 1, 3, 12, 00, 00), new DateTime(2023, 1, 3, 12, 15, 00));
        var timeFrameRopot2 =
            new TimeFrame(new DateTime(2023, 1, 10, 12, 00, 00), new DateTime(2023, 1, 10, 12, 15, 00));
        var timeFrameRopot3 =
            new TimeFrame(new DateTime(2023, 1, 17, 12, 00, 00), new DateTime(2023, 1, 17, 12, 15, 00));
        var timeFrameRopot4 =
            new TimeFrame(new DateTime(2023, 1, 24, 12, 00, 00), new DateTime(2023, 1, 24, 12, 15, 00));
        var timeFrameRopot5 =
            new TimeFrame(new DateTime(2023, 1, 31, 12, 00, 00), new DateTime(2023, 1, 31, 12, 15, 00));
        var timeFrameRopot6 = new TimeFrame(new DateTime(2023, 2, 7, 12, 00, 00), new DateTime(2023, 2, 7, 12, 15, 00));

        var openRopotFollowing = new[] { (EActivityType.SubmitRopot, 0.7f), (EActivityType.SaveRopot, 0.3f) };
        var saveRopotFollowing = new[] { (EActivityType.SubmitRopot, 0.7f), (EActivityType.SaveRopot, 0.3f) };
        var submitRopotFollowing = new[] { (EActivityType.ViewRopot, 0.9f) };
        var viewRopotFollowing = new[] { (EActivityType.ReadStudyMaterials, 0.5f) };

        var openRopotChances = new StateChances(0.9f);
        var saveRopotChances = new StateChances(1f);
        var submitRopotChances = new StateChances(0.9f);
        var viewRopotChances = new StateChances(0.9f);

        var attendSeminar1Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar1 });
        var attendSeminar2Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar2 });
        var attendSeminar3Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar3 });
        var attendSeminar4Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar4 });
        var attendSeminar5Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar5 });
        var attendSeminar6Rules =
            new StateRules(false, 1, 0, openRopotFollowing, new HashSet<ProcessState>() { attendSeminar6 });


        var openRopot1 = new ProcessState(
            EActivityType.OpenRopot,
            ropot1,
            attendSeminar1Rules,
            openRopotChances,
            timeFrameRopot1
        );

        var openRopot2 = new ProcessState(
            EActivityType.OpenRopot,
            ropot2,
            attendSeminar2Rules,
            openRopotChances,
            timeFrameRopot2
        );

        var openRopot3 = new ProcessState(
            EActivityType.OpenRopot,
            ropot3,
            attendSeminar3Rules,
            openRopotChances,
            timeFrameRopot3
        );

        var openRopot4 = new ProcessState(
            EActivityType.OpenRopot,
            ropot4,
            attendSeminar4Rules,
            openRopotChances,
            timeFrameRopot4
        );

        var openRopot5 = new ProcessState(
            EActivityType.OpenRopot,
            ropot5,
            attendSeminar5Rules,
            openRopotChances,
            timeFrameRopot5
        );

        var openRopot6 = new ProcessState(
            EActivityType.OpenRopot,
            ropot6,
            attendSeminar6Rules,
            openRopotChances,
            timeFrameRopot6
        );

        var saveRopot1 = new ProcessState(
            EActivityType.SaveRopot,
            ropot1,
            attendSeminar1Rules,
            saveRopotChances,
            timeFrameRopot1
        );

        var saveRopot2 = new ProcessState(
            EActivityType.SaveRopot,
            ropot2,
            attendSeminar2Rules,
            saveRopotChances,
            timeFrameRopot2
        );

        var saveRopot3 = new ProcessState(
            EActivityType.SaveRopot,
            ropot3,
            attendSeminar3Rules,
            saveRopotChances,
            timeFrameRopot3
        );

        var saveRopot4 = new ProcessState(
            EActivityType.SaveRopot,
            ropot4,
            attendSeminar4Rules,
            saveRopotChances,
            timeFrameRopot4
        );

        var saveRopot5 = new ProcessState(
            EActivityType.SaveRopot,
            ropot5,
            attendSeminar5Rules,
            saveRopotChances,
            timeFrameRopot5
        );

        var saveRopot6 = new ProcessState(
            EActivityType.SaveRopot,
            ropot6,
            attendSeminar6Rules,
            saveRopotChances,
            timeFrameRopot6
        );
        
        var submitRopot1 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot1,
            attendSeminar1Rules,
            submitRopotChances,
            timeFrameRopot1
        );

        var submitRopot2 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot2,
            attendSeminar2Rules,
            submitRopotChances,
            timeFrameRopot2
        );

        var submitRopot3 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot3,
            attendSeminar3Rules,
            submitRopotChances,
            timeFrameRopot3
        );

        var submitRopot4 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot4,
            attendSeminar4Rules,
            submitRopotChances,
            timeFrameRopot4
        );

        var submitRopot5 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot5,
            attendSeminar5Rules,
            submitRopotChances,
            timeFrameRopot5
        );

        var submitRopot6 = new ProcessState(
            EActivityType.SubmitRopot,
            ropot6,
            attendSeminar6Rules,
            submitRopotChances,
            timeFrameRopot6
        );
        
        var viewRopot1 = new ProcessState(
            EActivityType.ViewRopot,
            ropot1,
            attendSeminar1Rules,
            viewRopotChances,
            timeFrameRopot1
        );

        var viewRopot2 = new ProcessState(
            EActivityType.ViewRopot,
            ropot2,
            attendSeminar2Rules,
            viewRopotChances,
            timeFrameRopot2
        );

        var viewRopot3 = new ProcessState(
            EActivityType.ViewRopot,
            ropot3,
            attendSeminar3Rules,
            viewRopotChances,
            timeFrameRopot3
        );

        var viewRopot4 = new ProcessState(
            EActivityType.ViewRopot,
            ropot4,
            attendSeminar4Rules,
            viewRopotChances,
            timeFrameRopot4
        );

        var viewRopot5 = new ProcessState(
            EActivityType.ViewRopot,
            ropot5,
            attendSeminar5Rules,
            viewRopotChances,
            timeFrameRopot5
        );

        var viewRopot6 = new ProcessState(
            EActivityType.ViewRopot,
            ropot6,
            attendSeminar6Rules,
            viewRopotChances,
            timeFrameRopot6
        );
        
        // TODO: register exam states
        
        // TODO: Create ActorFrame for each student and run evaluator with it
        
        // TODO: run evaluator
    }
}