# Process Event Log Generator
Simple C# class library designed with the purpose of enabling the generation of realistic event logs for processes.
In research and software development, having the ability to generate such event logs can be incredibly valuable for various purposes, including testing, debugging, and analysis.
The library is created to offer a certain level of customization and control over the process generation.

# Contents

Description of repository projects:

- [EventLogGenerationLibrary](https://github.com/lasaris/PELGen/tree/main/EventLogGenerationLibrary) - PELGen library for process generation.
- [EventLogGenerator](https://github.com/lasaris/PELGen/tree/main/EventLogGenerator) - Old (deprecated) application for Information system generation. Contains many hacks and probably bugs as well.
- [Examples](https://github.com/lasaris/PELGen/tree/main/Examples) - Contains multiple documented examples to help better undrestand the library and its capabilities.
- [InformationSystemGeneration](https://github.com/lasaris/PELGen/tree/main/InformationSystemGeneration) - Example project for advanced and complex library usage for real-life scenario.
- [UnitTests](https://github.com/lasaris/PELGen/tree/main/UnitTests) - Contains tests to ensure the functionality and correctness of the library.

# Usage

To use library within your domain, create your own project and import EventLogGenerationLibrary into it as a Reference.
Currently built on .NET 7.

# Examples

Here is a simple example of how this library can be used in your code.
For more inspiration you can have a look into [Examples](https://github.com/lasaris/PELGen/blob/main/Examples/Program.cs).

```cs
// Define common resources
var resource = "Assembly line 42";

// Create states with exact time of execution
var state1 = new ProcessState("Component assembly", resource, 1, new DateTime(2023, 10, 13, 12, 00, 00));
var state2 = new ProcessState("Quality inspection", resource, 1, new DateTime(2023, 10, 13, 12, 00, 10));
var state3 = new ProcessState("Product packing", resource, 1, new DateTime(2023, 10, 13, 13, 30, 40), true);

// Add following states to model the process
state1.AddFollowingState(state2);
state2.AddFollowingState(state3);

// Create configuration
var config = new Configuration(1, state1, 1, null, "manufacture.csv",
  "ActorId,ActorType,Activity,Resource,Timestamp",
  "Worker");

// Create generator and run generation
var generator = new EventGenerator(config);
generator.RunGeneration();
```