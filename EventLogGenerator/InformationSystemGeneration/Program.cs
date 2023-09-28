using EventLogGenerationLibrary;
using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;

namespace InformationSystemGeneration;

public class Program
{
    public static void Main(string[] args)
    {
        RunStudentProcess();
    }

    public static void RunStudentProcess()
    {
        // Setup seminar offsets
        var seminarActivites = new HashSet<string>()
        {
            "OpenRopot",
            "SaveRopot",
            "SubmitRopot",
            "ReceiveAttendance",
            "ReceiveAbsence",
        };
        var firstOffset = TimeSpan.Zero;
        var secondOffset = TimeSpan.FromDays(1) + TimeSpan.FromHours(4);
        var thirdOffset = TimeSpan.FromDays(2) + TimeSpan.FromHours(6);

        // Prepare Resources
        var course = "Process Mining Seminar";
        var seminarGroup1 = "Seminar group 1";
        var seminarGroup2 = "Seminar group 2";
        var seminarGroup3 = "Seminar group 3";
        var seminarWeek1 = "Seminar week 1";
        var seminarWeek2 = "Seminar week 2";
        var seminarWeek3 = "Seminar week 3";
        var seminarWeek4 = "Seminar week 4";
        var seminarWeek5 = "Seminar week 5";
        var seminarWeek6 = "Seminar week 6";
        var materialsWeek1 = "/um/slides-week01.pdf";
        var materialsWeek2 = "/um/slides-week02.pdf";
        var materialsWeek3 = "/um/slides-week03.pdf";
        var materialsWeek4 = "/um/slides-week04.pdf";
        var materialsWeek5 = "/um/slides-week05.pdf";
        var materialsWeek6 = "/um/slides-week06.pdf";
        var hw1 = "/ode/homework-1.zip";
        var hw2 = "/ode/homework-2.zip";
        var hw3 = "/ode/homework-3.zip";
        var notebookhw1 = "Homework 1";
        var notebookhw2 = "Homework 2";
        var notebookhw3 = "Homework 3";
        var ropot1 = "Ropot week 1";
        var ropot2 = "Ropot week 2";
        var ropot3 = "Ropot week 3";
        var ropot4 = "Ropot week 4";
        var ropot5 = "Ropot week 5";
        var ropot6 = "Ropot week 6";
        var exam1 = "Exam term 1";
        var exam2 = "Exam term 2";
        var exam3 = "Exam term 3";
        var exam4 = "Exam term 4";

        // CREATE TIMEFRAMES

        // Prepare times
        var courseRegistrationPeriod = new TimeFrame((2023, 1, 31, 17, 0, 0), (2023, 2, 12));
        var seminarRegistrationPeriod = new TimeFrame((2023, 2, 1, 17, 0, 0), (2023, 2, 12));

        var openRopotPeriod1 = new TimeFrame((2023, 2, 21, 10, 4, 0), (2023, 2, 21, 10, 5, 0));
        var openRopotPeriod2 = new TimeFrame((2023, 3, 7, 10, 4, 0), (2023, 3, 7, 10, 5, 0));
        var openRopotPeriod3 = new TimeFrame((2023, 3, 21, 10, 4, 0), (2023, 3, 21, 10, 5, 0));
        var openRopotPeriod4 = new TimeFrame((2023, 4, 4, 10, 4, 0), (2023, 4, 4, 10, 5, 0));
        var openRopotPeriod5 = new TimeFrame((2023, 4, 18, 10, 4, 0), (2023, 4, 18, 10, 5, 0));
        var openRopotPeriod6 = new TimeFrame((2023, 5, 2, 10, 4, 0), (2023, 5, 2, 10, 5, 0));

        var saveRopotPeriod1 = new TimeFrame((2023, 2, 21, 10, 5, 15), (2023, 2, 21, 10, 10, 0));
        var saveRopotPeriod2 = new TimeFrame((2023, 3, 7, 10, 5, 15), (2023, 3, 7, 10, 10, 0));
        var saveRopotPeriod3 = new TimeFrame((2023, 3, 21, 10, 5, 15), (2023, 3, 21, 10, 10, 0));
        var saveRopotPeriod4 = new TimeFrame((2023, 4, 4, 10, 5, 15), (2023, 4, 4, 10, 10, 0));
        var saveRopotPeriod5 = new TimeFrame((2023, 4, 18, 10, 5, 15), (2023, 4, 18, 10, 10, 0));
        var saveRopotPeriod6 = new TimeFrame((2023, 5, 2, 10, 5, 15), (2023, 5, 2, 10, 10, 0));

        var submitRopotPeriod1 = new TimeFrame((2023, 2, 21, 10, 9, 0), (2023, 2, 21, 10, 10, 0),
            ETimeFrameDistribution.Exponential);
        var submitRopotPeriod2 = new TimeFrame((2023, 3, 7, 10, 9, 0), (2023, 3, 7, 10, 10, 0),
            ETimeFrameDistribution.Exponential);
        var submitRopotPeriod3 = new TimeFrame((2023, 3, 21, 10, 9, 0), (2023, 3, 21, 10, 10, 0),
            ETimeFrameDistribution.Exponential);
        var submitRopotPeriod4 = new TimeFrame((2023, 4, 4, 10, 9, 0), (2023, 4, 4, 10, 10, 0),
            ETimeFrameDistribution.Exponential);
        var submitRopotPeriod5 = new TimeFrame((2023, 4, 18, 10, 9, 0), (2023, 4, 18, 10, 10, 0),
            ETimeFrameDistribution.Exponential);
        var submitRopotPeriod6 = new TimeFrame((2023, 5, 2, 10, 9, 0), (2023, 5, 2, 10, 10, 0),
            ETimeFrameDistribution.Exponential);

        var receivePointsTime1 = new DateTime(2023, 3, 24, 16, 45, 10);
        var receivePointsTime2 = new DateTime(2023, 4, 21, 19, 21, 35);
        var receivePointsTime3 = new DateTime(2023, 5, 19, 17, 37, 55);

        var examRegistrationStart = new DateTime(2023, 5, 8, 17, 0, 0);
        var examTime1 = new DateTime(2023, 5, 22, 9, 0, 0);
        var examTime2 = new DateTime(2023, 5, 31, 14, 0, 0);
        var examTime3 = new DateTime(2023, 6, 12, 10, 0, 0);
        var examTime4 = new DateTime(2023, 6, 22, 14, 0, 0);

        var examRegistrationPeriod1 =
            new TimeFrame(examRegistrationStart, examTime1, ETimeFrameDistribution.ReverseExponential);
        var examRegistrationPeriod2 =
            new TimeFrame(examRegistrationStart, examTime2, ETimeFrameDistribution.ReverseExponential);
        var examRegistrationPeriod3 =
            new TimeFrame(examRegistrationStart, examTime3, ETimeFrameDistribution.ReverseExponential);
        var examRegistrationPeriod4 =
            new TimeFrame(examRegistrationStart, examTime4, ETimeFrameDistribution.ReverseExponential);

        var createMaterialsPeriod1 = new TimeFrame((2023, 2, 13, 8, 0, 0), (2023, 2, 13, 9, 0, 0));
        var createMaterialsPeriod2 = new TimeFrame((2023, 2, 27, 8, 0, 0), (2023, 2, 27, 9, 0, 0));
        var createMaterialsPeriod3 = new TimeFrame((2023, 3, 13, 8, 0, 0), (2023, 3, 13, 9, 0, 0));
        var createMaterialsPeriod4 = new TimeFrame((2023, 3, 27, 8, 0, 0), (2023, 3, 27, 9, 0, 0));
        var createMaterialsPeriod5 = new TimeFrame((2023, 4, 10, 8, 0, 0), (2023, 4, 10, 9, 0, 0));
        var createMaterialsPeriod6 = new TimeFrame((2023, 4, 24, 8, 0, 0), (2023, 4, 24, 9, 0, 0));

        var createExamScanTime1 = new DateTime(2023, 5, 29, 10, 6, 10);
        var createExamScanTime2 = new DateTime(2023, 6, 7, 9, 53, 28);
        var createExamScanTime3 = new DateTime(2023, 6, 19, 14, 15, 54);
        var createExamScanTime4 = new DateTime(2023, 6, 26, 8, 42, 49);

        // FIXME: This probably cannot be generalized at all
        var allRopotPeriods = new HashSet<TimeFrame>()
        {
            new TimeFrame(openRopotPeriod1.Start + firstOffset, submitRopotPeriod1.End + firstOffset),
            new TimeFrame(openRopotPeriod1.Start + secondOffset, submitRopotPeriod1.End + secondOffset),
            new TimeFrame(openRopotPeriod1.Start + thirdOffset, submitRopotPeriod1.End + thirdOffset),
            new TimeFrame(openRopotPeriod2.Start + firstOffset, submitRopotPeriod2.End + firstOffset),
            new TimeFrame(openRopotPeriod2.Start + secondOffset, submitRopotPeriod2.End + secondOffset),
            new TimeFrame(openRopotPeriod2.Start + thirdOffset, submitRopotPeriod2.End + thirdOffset),
            new TimeFrame(openRopotPeriod3.Start + firstOffset, submitRopotPeriod3.End + firstOffset),
            new TimeFrame(openRopotPeriod3.Start + secondOffset, submitRopotPeriod3.End + secondOffset),
            new TimeFrame(openRopotPeriod3.Start + thirdOffset, submitRopotPeriod3.End + thirdOffset),
            new TimeFrame(openRopotPeriod4.Start + firstOffset, submitRopotPeriod4.End + firstOffset),
            new TimeFrame(openRopotPeriod4.Start + secondOffset, submitRopotPeriod4.End + secondOffset),
            new TimeFrame(openRopotPeriod4.Start + thirdOffset, submitRopotPeriod4.End + thirdOffset),
            new TimeFrame(openRopotPeriod5.Start + firstOffset, submitRopotPeriod5.End + firstOffset),
            new TimeFrame(openRopotPeriod5.Start + secondOffset, submitRopotPeriod5.End + secondOffset),
            new TimeFrame(openRopotPeriod5.Start + thirdOffset, submitRopotPeriod5.End + thirdOffset),
            new TimeFrame(openRopotPeriod6.Start + firstOffset, submitRopotPeriod6.End + firstOffset),
            new TimeFrame(openRopotPeriod6.Start + secondOffset, submitRopotPeriod6.End + secondOffset),
            new TimeFrame(openRopotPeriod6.Start + thirdOffset, submitRopotPeriod6.End + thirdOffset),
        };

        var submitHomeworkPeriod1 =
            new TimeFrame((2023, 3, 5), (2023, 3, 19), ETimeFrameDistribution.Exponential, allRopotPeriods);
        var submitHomeworkPeriod2 =
            new TimeFrame((2023, 4, 2), (2023, 4, 16), ETimeFrameDistribution.Exponential, allRopotPeriods);
        var submitHomeworkPeriod3 = new TimeFrame((2023, 4, 30), (2023, 5, 14), ETimeFrameDistribution.Exponential,
            allRopotPeriods);

        var viewRopotPeriod1 = new TimeFrame((2023, 2, 24), (2023, 3, 3), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);
        var viewRopotPeriod2 = new TimeFrame((2023, 3, 10), (2023, 3, 17), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);
        var viewRopotPeriod3 = new TimeFrame((2023, 3, 24), (2023, 3, 31), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);
        var viewRopotPeriod4 = new TimeFrame((2023, 4, 7), (2023, 4, 14), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);
        var viewRopotPeriod5 = new TimeFrame((2023, 4, 21), (2023, 4, 28), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);
        var viewRopotPeriod6 = new TimeFrame((2023, 5, 5), (2023, 5, 12), ETimeFrameDistribution.ReverseExponential,
            allRopotPeriods);

        var readStudyMaterialsPeriod1 = new TimeFrame(createMaterialsPeriod1.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);
        var readStudyMaterialsPeriod2 = new TimeFrame(createMaterialsPeriod2.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);
        var readStudyMaterialsPeriod3 = new TimeFrame(createMaterialsPeriod3.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);
        var readStudyMaterialsPeriod4 = new TimeFrame(createMaterialsPeriod4.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);
        var readStudyMaterialsPeriod5 = new TimeFrame(createMaterialsPeriod5.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);
        var readStudyMaterialsPeriod6 = new TimeFrame(createMaterialsPeriod6.End, examTime4,
            ETimeFrameDistribution.Uniform, allRopotPeriods);

        // Useful properties
        var semesterEnd = new DateTime(2023, 7, 1);

        // CREATE STATES

        var enrollCourse = new ProcessState(
            "EnrollCourse",
            course,
            1,
            courseRegistrationPeriod
        );

        var registerSeminarGroup1 = new ProcessState(
            "RegisterSeminarGroup",
            seminarGroup1,
            1,
            seminarRegistrationPeriod,
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, firstOffset);
                ActorService.SetActivitiesOffset(actor, new HashSet<string>() { "ReceivePoints" }, TimeSpan.Zero);
            }
        );

        var registerSeminarGroup2 = new ProcessState(
            "RegisterSeminarGroup",
            seminarGroup2,
            1,
            seminarRegistrationPeriod,
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, secondOffset);
                ActorService.SetActivitiesOffset(actor, new HashSet<string>() { "ReceivePoints" },
                    TimeSpan.FromDays(1));
            }
        );

        var registerSeminarGroup3 = new ProcessState(
            "RegisterSeminarGroup",
            seminarGroup3,
            1,
            seminarRegistrationPeriod,
            false,
            (actor) =>
            {
                ActorService.SetActivitiesOffset(actor, seminarActivites, thirdOffset);
                ActorService.SetActivitiesOffset(actor, new HashSet<string>() { "ReceivePoints" },
                    TimeSpan.FromDays(2));
            }
        );

        var submitHomework1 = new ProcessState(
            "CreateFile",
            hw1,
            -1,
            submitHomeworkPeriod1
        );

        var submitHomework2 = new ProcessState(
            "CreateFile",
            hw2,
            -1,
            submitHomeworkPeriod2
        );

        var submitHomework3 = new ProcessState(
            "CreateFile",
            hw3,
            -1,
            submitHomeworkPeriod3
        );

        var removeHomework1 = new ProcessState(
            "DeleteFile",
            hw1,
            -1,
            submitHomeworkPeriod1
        );

        var removeHomework2 = new ProcessState(
            "DeleteFile",
            hw2,
            -1,
            submitHomeworkPeriod2
        );

        var removeHomework3 = new ProcessState(
            "DeleteFile",
            hw3,
            -1,
            submitHomeworkPeriod3
        );

        var readHomework1 = new ProcessState(
            "ReadFile",
            hw1,
            -1,
            submitHomeworkPeriod1
        );

        var readHomework2 = new ProcessState(
            "ReadFile",
            hw2,
            -1,
            submitHomeworkPeriod2
        );

        var readHomework3 = new ProcessState(
            "ReadFile",
            hw3,
            -1,
            submitHomeworkPeriod3
        );

        var openRopot1 = new ProcessState(
            "OpenRopot",
            ropot1,
            2,
            openRopotPeriod1
        );

        var openRopot2 = new ProcessState(
            "OpenRopot",
            ropot2,
            2,
            openRopotPeriod2
        );

        var openRopot3 = new ProcessState(
            "OpenRopot",
            ropot3,
            2,
            openRopotPeriod3
        );

        var openRopot4 = new ProcessState(
            "OpenRopot",
            ropot4,
            2,
            openRopotPeriod4
        );

        var openRopot5 = new ProcessState(
            "OpenRopot",
            ropot5,
            2,
            openRopotPeriod5
        );

        var openRopot6 = new ProcessState(
            "OpenRopot",
            ropot6,
            2,
            openRopotPeriod6
        );

        var saveRopot1 = new ProcessState(
            "SaveRopot",
            ropot1,
            1,
            saveRopotPeriod1
        );

        var saveRopot2 = new ProcessState(
            "SaveRopot",
            ropot2,
            1,
            saveRopotPeriod2
        );

        var saveRopot3 = new ProcessState(
            "SaveRopot",
            ropot3,
            1,
            saveRopotPeriod3
        );

        var saveRopot4 = new ProcessState(
            "SaveRopot",
            ropot4,
            1,
            saveRopotPeriod4
        );

        var saveRopot5 = new ProcessState(
            "SaveRopot",
            ropot5,
            1,
            saveRopotPeriod5
        );

        var saveRopot6 = new ProcessState(
            "SaveRopot",
            ropot6,
            1,
            saveRopotPeriod6
        );

        var submitRopot1 = new ProcessState(
            "SubmitRopot",
            ropot1,
            1,
            submitRopotPeriod1
        );

        var submitRopot2 = new ProcessState(
            "SubmitRopot",
            ropot2,
            1,
            submitRopotPeriod2
        );

        var submitRopot3 = new ProcessState(
            "SubmitRopot",
            ropot3,
            1,
            submitRopotPeriod3
        );

        var submitRopot4 = new ProcessState(
            "SubmitRopot",
            ropot4,
            1,
            submitRopotPeriod4
        );

        var submitRopot5 = new ProcessState(
            "SubmitRopot",
            ropot5,
            1,
            submitRopotPeriod5
        );

        var submitRopot6 = new ProcessState(
            "SubmitRopot",
            ropot6,
            1,
            submitRopotPeriod6
        );

        // Implement auto save of ropots
        var autoSavePeriod = TimeSpan.FromMinutes(1);

        // Deliberately left out option to fail internet on first ropot
        // var reopenRopot1 = new DummyState("OpenRopot", ropot1);
        var reopenRopot2 = new DummyState("OpenRopot", ropot2);
        var reopenRopot3 = new DummyState("OpenRopot", ropot3);
        var reopenRopot4 = new DummyState("OpenRopot", ropot4);
        var reopenRopot5 = new DummyState("OpenRopot", ropot5);
        var reopenRopot6 = new DummyState("OpenRopot", ropot6);

        var autoSaveRopot1 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot1,
            new HashSet<ProcessState>() { openRopot1 },
            new HashSet<ProcessState>() { submitRopot1 },
            autoSavePeriod
        );

        var autoSaveRopot2 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot2,
            new HashSet<ProcessState>() { openRopot2 },
            new HashSet<ProcessState>() { submitRopot2 },
            autoSavePeriod,
            (reopenRopot2, 0.013f, 1)
        );

        var autoSaveRopot3 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot3,
            new HashSet<ProcessState>() { openRopot3 },
            new HashSet<ProcessState>() { submitRopot3 },
            autoSavePeriod,
            (reopenRopot3, 0.013f, 1)
        );

        var autoSaveRopot4 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot4,
            new HashSet<ProcessState>() { openRopot4 },
            new HashSet<ProcessState>() { submitRopot4 },
            autoSavePeriod,
            (reopenRopot4, 0.013f, 1)
        );

        var autoSaveRopot5 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot5,
            new HashSet<ProcessState>() { openRopot5 },
            new HashSet<ProcessState>() { submitRopot5 },
            autoSavePeriod,
            (reopenRopot5, 0.013f, 1)
        );

        var autoSaveRopot6 = new PeriodicSprinkleState(
            "SaveRopot",
            ropot6,
            new HashSet<ProcessState>() { openRopot6 },
            new HashSet<ProcessState>() { submitRopot6 },
            autoSavePeriod,
            (reopenRopot6, 0.013f, 1)
        );

        var registerTerm1 = new ProcessState(
            "RegisterExamTerm",
            exam1,
            1,
            examRegistrationPeriod1
        );

        var registerTerm2 = new ProcessState(
            "RegisterExamTerm",
            exam2,
            1,
            examRegistrationPeriod2
        );

        var registerTerm3 = new ProcessState(
            "RegisterExamTerm",
            exam3,
            1,
            examRegistrationPeriod3
        );

        var registerTerm4 = new ProcessState(
            "RegisterExamTerm",
            exam4,
            1,
            examRegistrationPeriod4
        );

        var failExam1 = new ProcessState(
            "FailExam",
            exam1,
            1,
            createExamScanTime1
        );

        var failExam2 = new ProcessState(
            "FailExam",
            exam2,
            1,
            createExamScanTime2
        );

        var failExam3 = new ProcessState(
            "FailExam",
            exam3,
            1,
            createExamScanTime3
        );

        var failExam4 = new ProcessState(
            "FailExam",
            exam4,
            1,
            createExamScanTime4
        );

        var passExam1 = new ProcessState(
            "PassExam",
            exam1,
            1,
            createExamScanTime1
        );

        var passExam2 = new ProcessState(
            "PassExam",
            exam2,
            1,
            createExamScanTime2
        );

        var passExam3 = new ProcessState(
            "PassExam",
            exam3,
            1,
            createExamScanTime3
        );

        var passExam4 = new ProcessState(
            "PassExam",
            exam4,
            1,
            createExamScanTime4
        );

        // Finishing processes
        var passCourse = new ProcessState(
            "PassCourse",
            course,
            1,
            new TimeFrame(examRegistrationStart, semesterEnd),
            true
        );

        var failCourse = new ProcessState(
            "FailCourse",
            course,
            1,
            new TimeFrame(examRegistrationStart, semesterEnd),
            true
        );

        // Create sprinkles
        var readStudyMaterials1 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek1,
            readStudyMaterialsPeriod1
        );

        var readStudyMaterials2 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek2,
            readStudyMaterialsPeriod2
        );

        var readStudyMaterials3 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek3,
            readStudyMaterialsPeriod3
        );

        var readStudyMaterials4 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek4,
            readStudyMaterialsPeriod4
        );

        var readStudyMaterials5 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek5,
            readStudyMaterialsPeriod5
        );

        var readStudyMaterials6 = new IntervalSprinkleState(
            "ReadFile",
            materialsWeek6,
            readStudyMaterialsPeriod6
        );

        var viewRopot1 = new IntervalSprinkleState(
            "ViewRopot",
            ropot1,
            viewRopotPeriod1
        );

        var viewRopot2 = new IntervalSprinkleState(
            "ViewRopot",
            ropot2,
            viewRopotPeriod2
        );

        var viewRopot3 = new IntervalSprinkleState(
            "ViewRopot",
            ropot3,
            viewRopotPeriod3
        );

        var viewRopot4 = new IntervalSprinkleState(
            "ViewRopot",
            ropot4,
            viewRopotPeriod4
        );

        var viewRopot5 = new IntervalSprinkleState(
            "ViewRopot",
            ropot5,
            viewRopotPeriod5
        );

        var viewRopot6 = new IntervalSprinkleState(
            "ViewRopot",
            ropot6,
            viewRopotPeriod6
        );

        var receivePointsHomework1 = new FixedTimeState(
            "ReceivePoints",
            notebookhw1,
            receivePointsTime1,
            submitHomework1
        );

        var receivePointsHomework2 = new FixedTimeState(
            "ReceivePoints",
            notebookhw2,
            receivePointsTime2,
            submitHomework2
        );

        var receivePointsHomework3 = new FixedTimeState(
            "ReceivePoints",
            notebookhw3,
            receivePointsTime3,
            submitHomework3
        );

        // Conditional sprinkles for attendance marking
        var attendanceSeminar1 = new ConditionalSprinkle(
            openRopot1,
            new DummyState("ReceiveAttendance", seminarWeek1),
            new DummyState("ReceiveAbsence", seminarWeek1),
            TimeSpan.FromHours(1)
        );

        var attendanceSeminar2 = new ConditionalSprinkle(
            openRopot2,
            new DummyState("ReceiveAttendance", seminarWeek2),
            new DummyState("ReceiveAbsence", seminarWeek2),
            TimeSpan.FromHours(1)
        );

        var attendanceSeminar3 = new ConditionalSprinkle(
            openRopot3,
            new DummyState("ReceiveAttendance", seminarWeek3),
            new DummyState("ReceiveAbsence", seminarWeek3),
            TimeSpan.FromHours(1)
        );

        var attendanceSeminar4 = new ConditionalSprinkle(
            openRopot4,
            new DummyState("ReceiveAttendance", seminarWeek4),
            new DummyState("ReceiveAbsence", seminarWeek4),
            TimeSpan.FromHours(1)
        );

        var attendanceSeminar5 = new ConditionalSprinkle(
            openRopot5,
            new DummyState("ReceiveAttendance", seminarWeek5),
            new DummyState("ReceiveAbsence", seminarWeek5),
            TimeSpan.FromHours(1)
        );

        var attendanceSeminar6 = new ConditionalSprinkle(
            openRopot6,
            new DummyState("ReceiveAttendance", seminarWeek6),
            new DummyState("ReceiveAbsence", seminarWeek6),
            TimeSpan.FromHours(1)
        );

        var minAttendanceRule = new MinimumActivityCountRule(
            registerTerm1,
            (failCourse, semesterEnd),
            "ReceiveAttendance",
            5
        );

        var minHomeworks = new MinimumActivityCountRule(
            registerTerm1,
            (failCourse, semesterEnd),
            "CreateFile",
            2
        );

        // SETUP FOLLOWING STATES

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
        submitHomework3.AddFollowingStates((registerTerm1, 0.65f), (registerTerm2, 0.05f), (registerTerm3, 0.05f),
            (readHomework3, 0.2f));
        readHomework3.AddFollowingStates((registerTerm1, 0.65f), (registerTerm2, 0.05f), (registerTerm3, 0.05f),
            (removeHomework3, 0.2f));
        removeHomework3.AddFollowingStates((submitHomework3, 1f));

        // exams
        registerTerm1.AddFollowingStates((passExam1, 0.60f), (failExam1, 0.40f));
        registerTerm2.AddFollowingStates((passExam2, 0.55f), (failExam2, 0.45f));
        registerTerm3.AddFollowingStates((passExam3, 0.50f), (failExam3, 0.50f));
        registerTerm4.AddFollowingStates((passExam4, 0.45f), (failExam4, 0.55f));
        failExam1.AddFollowingStates((registerTerm2, 0.8f), (registerTerm3, 0.1f), (failCourse, 0.1f));
        failExam2.AddFollowingStates((registerTerm3, 0.7f), (registerTerm4, 0.1f), (failCourse, 0.1f));
        failExam3.AddFollowingStates((registerTerm4, 0.7f), (failCourse, 0.3f));
        failExam4.AddFollowingStates((failCourse, 1f));
        passExam1.AddFollowingStates((passCourse, 1f));
        passExam2.AddFollowingStates((passCourse, 1f));
        passExam3.AddFollowingStates((passCourse, 1f));
        passExam4.AddFollowingStates((passCourse, 1f));


        // RUNNING THE SIMULATION
        var studentConfig = new Configuration(
            200,
            enrollCourse,
            600000,
            new Dictionary<string, int>()
            {
                { "RegisterExamTerm", 3 }
            },
            "student.csv",
            "ActorId,ActorType,Activity,Resource,StartTimestamp,OwnerId",
            "Student"
        );

        var studentGenerator = new EventGenerator(studentConfig);
        studentGenerator.RunGeneration();
    }
}