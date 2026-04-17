# Technical Overview

## Summary

Mars Robots simulator on a rectangular grid. Reads a script describing the surface and robots, executes commands, outputs final positions.

## Project Structure

```
Robots/
  Program.cs          — entry point, CLI parsing, DI, launch
  Models/             — data models
  Services/           — business logic
  Validators/         — input validation (FluentValidation)
  script.txt          — sample input script
Robots.Tests/         — unit tests (xUnit + FluentAssertions)
```

## Models (`Models/`)

| Class.         | Purpose                                                                         |
|----------------|---------------------------------------------------------------------------------|
| `World`        | Root object: contains `Surface` and a list of `Robot`                           |
| `Surface`      | Grid dimensions (`Width × Height`) and `DangerousCells`                         |
|                |  — coordinates of cells where a robot has previously fallen off                 |
| `Robot`        | Position (`X`, `Y`), direction (`Orientation`), command list, and `IsLost` flag |
| `Orientation`  | Enum: `N`, `E`, `S`, `W`                                                        |
| `Command`      | Enum: `L` (turn left), `R` (turn right), `F` (move forward)                     |
| `CommandsList` | Inherits `List<Command>` — a robot's command sequence                           |

## Services (`Services/`)

### `ScriptReader` (implements `IScriptReader`)

Parses a text script from a `TextReader`:
1. First line — surface dimensions (`"5 3"`)
2. Then pairs of lines: robot position (`"1 1 E"`) and its commands (`"RFRFRFRF"`)

Returns a populated `World`.

### `Simulator`

Takes `World` and `IAppLogger`. Method `Run()`:
- Iterates over robots in order
- Executes each robot's commands sequentially
- `L` / `R` — 90° rotation
- `F` — move forward; if the next position is out of bounds:
  - If the cell is **not** in `DangerousCells` — the robot is lost (`IsLost = true`), the cell is added to the list
  - If the cell is **already** dangerous — the command is ignored (the robot stays in place)
- When `Verbose = true`, logs step-by-step output with an ASCII map

### `FormatLog`

- `Format(World)` — final output: `"1 1 E"` or `"3 3 N LOST"` for each robot
- `ASCII(World)` — debug ASCII map showing robots and dangerous cells

### `IAppLogger` / `ConsoleLogger`

Logging interface and its `Console`-based implementation.

## Validators (`Validators/`)

Built with FluentValidation. `WorldValidator` is the root validator and invokes:
- `SurfaceValidator` — width/height within `[0, maxCoord]`
- `RobotValidator` — coordinates and command list length
- Check that each robot's starting position is inside the surface

Limits (`maxCoord`, `maxCommands`) are configurable via CLI options.

## CLI (`Program.cs`)

Uses `System.CommandLine`. Options:

| Flag                   | Description                    | Default      |
|------------------------|--------------------------------|--------------|
| `--script`, `-s`       | Path to the script file        | `script.txt` |
| `--verbose`, `-v`      | Detailed step-by-step logging. | `false`      |
| `--max-commands`, `-m` | Max commands per robot         | `100`        |
| `--max-coord`, `-c`    | Max allowed coordinate value   | `50`         |

## Execution Flow

```
CLI parsing → ScriptReader.Read() → WorldValidator.Validate()
  → Simulator.Run() → Console.Write(FormatLog.Format())
```

## Tests (`Robots.Tests/`)

| File                   | Coverage                                     |
|------------------------|----------------------------------------------|
| `ValidatorTests.cs`    | Coordinate, command, and boundary validation |
| `SimulatorTests.cs`    | Movement logic, robot loss, dangerous cells  |
| `ScriptReaderTests.cs` | Script parsing, format errors                |
| `FormatLogTests.cs`    | Output formatting                            |
| `NullLogger.cs`        | `IAppLogger` stub for tests                  |
