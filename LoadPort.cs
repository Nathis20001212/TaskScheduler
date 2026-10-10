public class LoadPort: IStation{
    public string Name {get;}
    private readonly Queue<Wafer> _wafers = new();

    public LoadPort(string name, int waferCount = 0)
    {
        Name = name;
        for (int i=1; i<=waferCount; i++)
            _wafers.Enqueue(new Wafer(i));
    }

    public int Count => _wafers.Count;
    public bool HasWafers => _wafers.Count > 0;

    public Wafer TakeNext(int now)
    {
        if (!HasWafers)
            throw new InvalidOperationException($"{Name} is empty");
        
        Wafer wafer = _wafers.Dequeue();
        Logger.Log(now, $"{Name} : {wafer} released ({Count} left)");
        return wafer;
    }

    public void Receive(Wafer wafer, int now)
    {
        _wafers.Enqueue(wafer);
        Logger.Log(now, $"{Name} : {wafer} received ({Count} total)");
    }

    public Wafer Pick(int now) => TakeNext(now);
    public void Place(Wafer wafer, int now) => Receive(wafer, now);
}