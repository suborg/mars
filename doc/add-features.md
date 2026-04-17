# Adding New Robot Commands

## Overview

The system is designed for extending the command set. Each command is a `Command` enum value, recognized by the parser and executed by the simulator.

## Steps to Add a New Command

### 1. `Robots/Models/Command.cs` — declare the command

Add a new value to the enum. The name must match the character used in the script.

```csharp
public enum Command
{
    L, R, F,
    B  // ← new command: move backward
}
```

> Parsing in `ScriptReader` uses `Enum.TryParse<Command>(ch.ToString())`, so the enum value name = the command character in the script. A single-letter name is required.

### 2. `Robots/Services/Simulator.cs` — implement the logic

Add a case to the `Execute` method:

```csharp
private void Execute(Robot robot, Command command)
{
    switch (command)
    {
        case Command.L:
        case Command.R:
            Rotate(robot, command);
            break;
        case Command.F:
            Move(robot);
            break;
        case Command.B:       // ← new branch
            MoveBack(robot);
            break;
    }
}
```

Implement the method itself (e.g. `MoveBack`) following the pattern of `Move` / `Rotate`.

### 3. `Robots.Tests/SimulatorTests.cs` — add tests

At minimum:
- A test for correct command execution (happy path)
- A test for behavior at the grid boundary (loss / scent)

## Example: Command B (move backward)

Full example — add backward movement without turning.

**Command.cs** — add `B` to the enum.

**Simulator.cs** — add the method:

```csharp
private void MoveBack(Robot robot)
{
    int nextX = robot.X;
    int nextY = robot.Y;

    switch (robot.Orientation)
    {
        case Orientation.N: nextY -= 1; break;
        case Orientation.E: nextX -= 1; break;
        case Orientation.S: nextY += 1; break;
        case Orientation.W: nextX += 1; break;
    }

    // Out-of-bounds logic — same as Move
    if (IsOutOfBounds(nextX, nextY))
    {
        if (!world.DangerousCells[robot.X, robot.Y])
        {
            world.DangerousCells[robot.X, robot.Y] = true;
            robot.IsLost = true;
        }
    }
    else
    {
        robot.X = nextX;
        robot.Y = nextY;
    }
}
```

**SimulatorTests.cs** — test:

```csharp
[Fact]
public void Run_BackCommand_MovesOppositeDirection()
{
    var world = Read("5 5\n2 2 N\nB");
    new Simulator(world, logger).Run();

    world.Robots[0].X.Should().Be(2);
    world.Robots[0].Y.Should().Be(1);
    world.Robots[0].Orientation.Should().Be(Orientation.N);
}
```
