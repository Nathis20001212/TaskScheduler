// State Machine 

public enum ChamberState {Idle, Processing, Done, Damaged}

public class Chamber : IStation
{
    public const int ProcessTime = 20;
    public const int PickupWindow = 6;

    public string Name {get;}
    public ChamberState State {get; private set;} = ChamberState.Idle;
    public Wafer? Wafer {get; private set;}

    private int _seconds;

    public Chamber(string name)
    {
        Name = name;
    }

    public bool IsFree => State == ChamberState.Idle;

    public int SecondsUntilDone =>
        State == ChamberState.Processing ? ProcessTime - _seconds : int.MaxValue;

    public void Load(Wafer wafer, int now)
    {
        if (State != ChamberState.Idle)
            throw new InvalidOperationException($"{Name} is not free");
        
        Wafer = wafer;
        State = ChamberState.Processing;
        _seconds = 0;
        Logger.Log(now, $"{Name} : {wafer} loaded, processing started");
    }

    public void Tick(int now)
    {
        if (State == ChamberState.Processing)
        {
            _seconds++;
            if (_seconds == ProcessTime)
            {
                State = ChamberState.Done;
                _seconds = 0;
                Logger.Log(now, $"{Name} : {Wafer} processing done, {PickupWindow}s pickup window started");
            }
        }
        else if (State == ChamberState.Done)
        {
            _seconds++;
            if (_seconds > PickupWindow)
            {
                State = ChamberState.Damaged;
                Logger.Log(now, $"{Name} : {Wafer} DAMAGED, pickup window missed");
            }
        }
    }

    public Wafer Unload(int now)
    {
        if (State != ChamberState.Done)
            throw new InvalidOperationException($"{Name} has nothing ready to collect (state: {State})");
        
        Wafer wafer = Wafer!;
        Logger.Log(now, $"{Name} : {wafer} collected {_seconds}s after done");
        Wafer = null;
        State = ChamberState.Idle;
        _seconds = 0;
        return wafer;
    }

    public Wafer Pick(int now) => Unload(now);
    public void Place(Wafer wafer, int now) => Load(wafer, now);
}
