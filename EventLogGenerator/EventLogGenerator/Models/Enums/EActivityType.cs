namespace EventLogGenerator.Models.Enums;

public enum EActivityType
{
    // For IS student activities
    EnrollCourse,
    RegisterSeminarGroup,
    ReceiveAttendance,
    ReadFile,
    DeleteFile,
    CreateFile,
    ReceivePoints,
    // Ropot activities
    OpenRopot,
    SaveRopot,
    SubmitRopot,
    ViewRopot,
    // Exam activities
    RegisterExamTerm,
    FailExam,
    // Finishing activities
    PassCourse,
    FailCourse,
}