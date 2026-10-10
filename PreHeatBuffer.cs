public class PreHeatBuffer
{
    public List<Slot> Slots {get;} = new()
    {
        new Slot("S1"),
        new Slot("S2"),
        new Slot("S3")
    };

    public void Tick(int now)
    {
        foreach (Slot slot in Slots)
            slot.Tick(now);
    }

    public Slot? GetFreeSlot()
    {
        return Slots.FirstOrDefault(s => s.IsFree);
    }

    public Slot? GetReadySlot()
    {
        return Slots
            .Where(s => s.IsReady)
            .OrderByDescending(s => s.SecondsInSlot)
            .FirstOrDefault();
    }
}
