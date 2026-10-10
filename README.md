# Wafer Scheduling Task

**MindOx Task — done by S. Nathiskar**

A C# console simulation of a wafer-handling machine. One robot moves each wafer from
Load Port 1 → Pre-Heat Buffer (S1/S2/S3) → Chamber (CH1/CH2) → Load Port 2, with
zero damaged wafers, and prints an event log and run statistics.

## Assumptions

- **Robot:** each move takes 3 seconds. A pick is one move and a place is one move,
  so moving a wafer from one station to another takes 6 seconds.
- **Robot:** carries one wafer at a time and only moves a wafer when the destination is free.
- **Pre-heat slots:** each slot heats its wafer for 10 seconds. The task does not say that
  waiting in a slot damages the wafer, so a heated wafer can wait in its slot safely
  until the robot collects it. Only the chamber can damage a wafer.
- **Chambers:** processing takes 20 seconds. The robot must finish picking the wafer
  within 6 seconds after processing ends, otherwise the wafer is damaged.
- **Load Port 2** starts empty and collects the finished wafers.
- **Time** is simulated with a 1-second clock (it does not run in real time).

## How to run

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or newer).
2. Open a terminal in the project folder.
3. Run:

```
   dotnet run
```

4. Enter the number of wafers (1–25) when asked.