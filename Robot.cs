public enum RobotState {Idle, Picking, Placing}

public class Robot
{
    public const int MoveTime = 3;

    public RobotState State {get; private set; } = RobotState.Idle;
    public Wafer? Carrying {get; private set; }
    public IStation? Source {get; private set; }
    public IStation? Destination {get; private set; }

    private int _secondsLeft;
    public bool IsFree => State == RobotState.Idle;
    public void StartTransfer(IStation from, IStation to, int now)
    {
        if (!IsFree)
            throw new InvalidOperationException("Robot is busy");
        Source = from;
        Destination = to;
        State = RobotState.Picking;
        _secondsLeft = MoveTime;
        Logger.Log(now, $"Robot start {from.Name} -> {to.Name}, picking");
    }

    public void Tick(int now)
    {
        if (State == RobotState.Idle)
            return;
        
        _secondsLeft--;
        if (_secondsLeft > 0)
            return;

        if (State == RobotState.Picking)
        {
            Carrying = Source!.Pick(now);
            Logger.Log(now, $"Robot holding {Carrying}, moving to {Destination!.Name}");
            State = RobotState.Placing;
            _secondsLeft = MoveTime;
        }
        else if (State == RobotState.Placing)
        {
            Destination!.Place(Carrying!, now);
            Logger.Log(now, $"Robot delivered {Carrying} to {Destination.Name}, now free");
            Carrying = null;
            Source = null;
            Destination = null;
            State = RobotState.Idle;
        }
    }
}