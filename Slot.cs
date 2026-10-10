// asumption: not mentioned about damage in the documets so thought no damage to the wafer
public enum SlotState {Empty, Heating, Ready}

public class Slot : IStation
{
    public const int HeatTime = 10;

    public string Name { get; }
    public SlotState State { get; private set; } = SlotState.Empty;
    public Wafer? Wafer { get; private set; }
    public int SecondsInSlot {get; private set;}

    public Slot(string name)
    {
        Name = name;
    }

    public bool IsFree => State == SlotState.Empty;
    public bool IsReady => State == SlotState.Ready;

    public void Place(Wafer wafer, int now)
    {
        if (!IsFree)
            throw new InvalidOperationException($"{Name} is not free");

        Wafer = wafer;
        State = SlotState.Heating;
        SecondsInSlot = 0;
        Logger.Log(now, $"{Name} : {wafer} placed, heating started");
    }

    public void Tick(int now)
    {
        if (State == SlotState.Empty)
            return;

        SecondsInSlot++;

        if (State == SlotState.Heating && SecondsInSlot == HeatTime)
        {
            State = SlotState.Ready;
            Logger.Log(now, $"{Name} : {Wafer} heating done, ready to pick");
        }
    }

    public Wafer Take(int now)
    {
        if (!IsReady)
            throw new InvalidOperationException($"{Name} has no ready wafer (state: {State})");

        Wafer wafer = Wafer!;
        Logger.Log(now, $"{Name} : {wafer} taken ({SecondsInSlot - HeatTime}s after ready)");
        Wafer = null;
        State = SlotState.Empty;
        SecondsInSlot = 0;
        return wafer;
    }

    public Wafer Pick(int now) => Take(now); 
}