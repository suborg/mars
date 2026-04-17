# Robots Simulator — Usage

## Overview

A console application that simulates robots moving on a bounded rectangular grid on Mars. Robots follow instruction strings (L, R, F) and may fall off the grid. Lost robots leave a "scent" that prevents future robots from falling from the same point.

## Running the Program

```bash
# Default: reads script.txt in the current directory
dotnet run

# Specify a script file
dotnet run -- --script path/to/input.txt

# Verbose mode: shows step-by-step execution with ASCII grid visualization
dotnet run -- --script input.txt --verbose

# Custom limits
dotnet run -- --script input.txt --max-coord 100 --max-commands 200

# Show help
dotnet run -- --help
```

## Options

| Option                 | Short | Default      | Description                       |
|------------------------|-------|--------------|-----------------------------------|
| `--script <file>`      | `-s`  | `script.txt` | Path to the script file           | 
|                        |       |              | with robot instructions           |
|                        |       |              |                                   |
| `--verbose`            | `-v`  | `false`      | Enable detailed step-by-step      |
|                        |       |              | logging with ASCII grid           |
|                        |       |              |                                   |
| `--max-commands <int>` | `-m`  | `100`        | Max. number of commands per robot |
|                        |       |              |                                   |
| `--max-coord <int>`    | `-c`  | `50`         | Max. allowed coordinate value for |
|                        |       |              | surface and robots                |
|                        |       |              |                                   |
| `--help`               | `-h`  | —            | Show help and exit                |

## Script File Format

```
<width> <height>
<x> <y> <orientation>
<commands>
<x> <y> <orientation>
<commands>
...
```

- **Line 1** — surface dimensions (upper-right corner; lower-left is 0,0)
- **Odd lines** — robot starting position: X Y and orientation (N/S/E/W)
- **Even lines** — command string: L (turn left), R (turn right), F (move forward)

### Example Input

```
5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFLFLFL
```

### Example Output

```
1 1 E
3 3 N LOST
2 3 S
```

## Exit Codes

| Code | Meaning                                                 |
|------|---------------------------------------------------------|
| `0`  | Success                                                 |
| `1`  | Error (file not found, validation failure, parse error) |
