namespace EventLogGenerator.Models.Enums;

public enum EActivityType
{
    // For IS student activities
    EnrollCourse,
    RegisterSeminarGroup,
    ReceiveAttendance,
    ReceiveAbsence,
    // Ropot activities
    OpenRopot,
    SaveRopot,
    SubmitRopot,
    ViewRopot,
    // Exam activities
    RegisterExamTerm,
    PassExam,
    FailExam,
    // Finishing activities
    PassCourse,
    FailCourse,
    // Teacher activities
    VisitStudentRecord,
    GivePoints,
    MarkAttendance,
    MarkAbsence,
    GiveFinalGrade,
    DeleteRopotSession,
    // File operations
    ReadFile,
    DeleteFile,
    CreateFile,
    ReceivePoints,
}