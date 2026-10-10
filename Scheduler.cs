public class Scheduler
{
    private const int TransferTime = 2 * Robot.MoveTime;

    private int _robotIdleSeconds;
    private int _safeHoldSeconds;
    private int _destinationBlockeSeconds;
    private int _destinationBlockeWaits;
    private bool _wasBlocked;

    private readonly int _totalWafers;
    private readonly LoadPort _lp1;
    private readonly LoadPort _lp2;
    private readonly PreHeatBuffer _buffer = new();
    private readonly List<Chamber> _chambers = new()
    {
        new Chamber("CH1"),
        new Chamber("CH2")
    };
    private readonly Robot _robot = new();

    public Scheduler(int waferCount)
    {
        _totalWafers = waferCount;
        _lp1 = new LoadPort("LP1", waferCount);
        _lp2 = new LoadPort("LP2");
    }

    private void DecideNext(int now)
    {
        // priority -> 1 
        // fininshed chamber (ch1/ch2) has a deadline so coniser it first
        Chamber? done = _chambers.FirstOrDefault(c => c.State == ChamberState.Done);
        if (done != null)
        {
            StartJob(done, _lp2, now);
            return;
        }
        
       
        // cofirm that the chabember heating time would be less or not , for making robot busy
        if (_chambers.Any(c => c.SecondsUntilDone < TransferTime))
        {
            RecordIdle(safetyHold: true, blocked: false);
            return;
        }
            

         // priority -> 2 heated wafer and does chamber (ch1/ch2) is empty make the robot busy
        Slot? ready = _buffer.GetReadySlot();
        Chamber? empty = _chambers.FirstOrDefault(c => c.IsFree);
        if (ready != null && empty != null)
        {
            StartJob(ready, empty, now);
            return;
        }

        // p3 wafer in Lp1 and free slot then go on
        Slot? free = _buffer.GetFreeSlot();
        if (_lp1.HasWafers && free != null)
        {
            StartJob(_lp1, free, now);
            return;
        }

        bool heatedWaferBlocked = ready != null && empty == null;   
        bool newWaferBlocked    = _lp1.HasWafers && free == null;
        RecordIdle(safetyHold: false, blocked: heatedWaferBlocked || newWaferBlocked);

    }

    private void StartJob(IStation from, IStation to, int now)
    {
        _robot.StartTransfer(from, to, now);
        _wasBlocked = false;
    }

    private void RecordIdle(bool safetyHold, bool blocked)
    {
        _robotIdleSeconds++;

        if (safetyHold)
            _safeHoldSeconds++;

        if (blocked)
        {
            _destinationBlockeSeconds++;
            if (!_wasBlocked)
                _destinationBlockeWaits++;
        }

        _wasBlocked = blocked;
    }

    private void PrintEnd(int totalTime)
    {
        int busySeconds = totalTime - _robotIdleSeconds;
        Console.WriteLine();
        Console.WriteLine("Total time");
        Console.WriteLine("------------------------------------------------------------------------------------");
        Console.WriteLine($"Wafer proceed : {_lp2.Count}");
        Console.WriteLine($"Total time    : {totalTime} s");
        Console.WriteLine($"Waits (destination full)  : {_destinationBlockeWaits} times {_destinationBlockeSeconds} s");
        Console.WriteLine($"Robot idle time : {_robotIdleSeconds} s");
        Console.WriteLine($" of which safety holds : {_safeHoldSeconds} s");
        Console.WriteLine($"Robot busy time : {busySeconds} s ({100.0 * busySeconds / totalTime:F1}%)");
        Console.WriteLine("Damaged wafers : 0");
        Console.WriteLine("------------------------------------------------------------------------------------");
    }


    public void Run()
    {
        int now = 0;
        DecideNext(now);

        while (_lp2.Count < _totalWafers)
        {
            now++;

            _buffer.Tick(now);
            foreach (Chamber chamber in _chambers)
                chamber.Tick(now);
            _robot.Tick(now);

            if (_chambers.Any(c => c.State == ChamberState.Damaged))
                throw new Exception($"A wafer was damaged at t={now}s. Scheduler bug!");

            if (_robot.IsFree && _lp2.Count < _totalWafers)
                DecideNext(now);
            
        }
        Logger.Log(now, $"All {_totalWafers} wafers finished");
        PrintEnd(now);

    }
}