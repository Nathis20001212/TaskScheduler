public class Wafer
{
    public int Id {get; }

    public Wafer(int id)
    {
        Id = id;
    }

    public override string ToString() => $"Wafer{Id}";
}