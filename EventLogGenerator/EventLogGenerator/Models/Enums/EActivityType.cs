namespace EventLogGenerator.Models.Enums;

public enum EActivityType
{
    // For IS student activities
    EnrollCourse,
    RegisterSeminarGroup,
    ReceiveAttendance,
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
    CreateHomeworkVault,
    GivePoints,
    MarkAttendance,
    CreateExamTerm,
    GiveFinalGrade,
    // File operations
    ReadFile,
    DeleteFile,
    CreateFile,
    ReceivePoints,
}