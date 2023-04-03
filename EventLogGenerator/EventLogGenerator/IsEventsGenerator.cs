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
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 14)),
            true
        );

        var enrolledCourseSet = new HashSet<ProcessState>() { enrollCourse };

        var registerSeminarGroup = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup,
            new StateRules(true, 1, 4, new HashSet<ProcessState>() { enrollCourse }),
            new StateChances(1, 0.05f),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );

        var materialRules = new StateRules(false, -1, -1, enrolledCourseSet);

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

        var submitHomework1 = new ProcessState(
            EActivityType.SubmitHomework,
            hw1,
            new StateRules(false, 1, 0, enrolledCourseSet),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 2), new DateTime(2023, 1, 15), ETimeFrameDistribution.Exponential)
        );

        var submitHomework2 = new ProcessState(
            EActivityType.SubmitHomework,
            hw2,
            new StateRules(false, 1, 0, enrolledCourseSet),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 16), new DateTime(2023, 1, 29), ETimeFrameDistribution.Exponential)
        );

        var submitHomework3 = new ProcessState(
            EActivityType.SubmitHomework,
            hw3,
            new StateRules(false, 1, 0, enrolledCourseSet),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 30), new DateTime(2023, 2, 12), ETimeFrameDistribution.Exponential)
        );

        var attendSeminar1 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek1,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 3, 12, 00, 00), new DateTime(2023, 1, 3, 12, 20, 00))
        );
        
        var attendSeminar2 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek2,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 10, 12, 00, 00), new DateTime(2023, 1, 10, 12, 20, 00))
        );
        
        var attendSeminar3 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek3,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 17, 12, 00, 00), new DateTime(2023, 1, 17, 12, 20, 00))
        );
        
        var attendSeminar4 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek4,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 24, 12, 00, 00), new DateTime(2023, 1, 24, 12, 20, 00))
        );
        
        var attendSeminar5 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek5,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 1, 31, 12, 00, 00), new DateTime(2023, 1, 31, 12, 20, 00))
        );
        
        var attendSeminar6 = new ProcessState(
            EActivityType.AttendSeminar,
            seminarWeek6,
            new StateRules(false, 1, 0, enrolledCourseSet),
            seminarAttendanceChances,
            new TimeFrame(new DateTime(2023, 2, 7, 12, 00, 00), new DateTime(2023, 2, 7, 12, 20, 00))
        );

        HashSet<ProcessState> attendingSeminar1Set = new HashSet<ProcessState>();
        attendingSeminar1Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar1});
        
        HashSet<ProcessState> attendingSeminar2Set = new HashSet<ProcessState>();
        attendingSeminar2Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar2});
        
        HashSet<ProcessState> attendingSeminar3Set = new HashSet<ProcessState>();
        attendingSeminar3Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar3});
        
        HashSet<ProcessState> attendingSeminar4Set = new HashSet<ProcessState>();
        attendingSeminar4Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar4});
        
        HashSet<ProcessState> attendingSeminar5Set = new HashSet<ProcessState>();
        attendingSeminar5Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar5});
        
        HashSet<ProcessState> attendingSeminar6Set = new HashSet<ProcessState>();
        attendingSeminar6Set.UnionWith(new HashSet<ProcessState>(){enrollCourse, attendSeminar6});
        
        // TODO: add states for each ropot and each activity (6x ropot and 4x activity => 24 states :') )
    }
}