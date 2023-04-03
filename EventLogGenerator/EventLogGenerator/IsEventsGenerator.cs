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
        FileManager.SetupNewCsvFile("CaseId,Activity,ActorId,ActorType,StartTimestamp,EndTimestamp");
        
        // Prepare Actors
        List<Actor> students = Enumerable.Range(0, studentsCount)
            .Select(_ => new Actor(EActorType.Student))
            .ToList();

        // Prepare Resources
        var course = new Resource("Course X");
        var seminarGroup = new Resource("Seminar group 1");
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

        var enrolledCourseRule = new HashSet<ProcessState>() { enrollCourse };

        var registerSeminarGroup = new ProcessState(
            EActivityType.RegisterSeminarGroup,
            seminarGroup,
            new StateRules(true, 1, 4, new HashSet<ProcessState>() { enrollCourse }),
            new StateChances(1, 0.05f),
            new TimeFrame(new DateTime(2022, 12, 24), new DateTime(2022, 12, 31))
        );

        var materialRules = new StateRules(false, -1, -1, enrolledCourseRule);

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
            new StateRules(false, 1, 0, enrolledCourseRule),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 2), new DateTime(2023, 1, 15), ETimeFrameDistribution.Exponential)
        );
        
        var submitHomework2 = new ProcessState(
            EActivityType.SubmitHomework,
            hw2,
            new StateRules(false, 1, 0, enrolledCourseRule),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 16), new DateTime(2023, 1, 29), ETimeFrameDistribution.Exponential)
        );
        
        var submitHomework3 = new ProcessState(
            EActivityType.SubmitHomework,
            hw3,
            new StateRules(false, 1, 0, enrolledCourseRule),
            defaultChances,
            new TimeFrame(new DateTime(2023, 1, 30), new DateTime(2023, 2, 12), ETimeFrameDistribution.Exponential)
        );
    }
}