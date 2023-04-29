using EventLogGenerator.InputOutput;
using EventLogGenerator.Services;

namespace EventLogGenerator;

public static class TeacherGenerator
{
    public static void GenerateLogs(int teacherCount = 0)
    {
        // Setup services
        FileManager.SetupNewCsvFile("ActorId,ActorType,Activity,Resource,StartTimestamp", "student.csv");
        IdService.ResetService();
        
        
    }
}