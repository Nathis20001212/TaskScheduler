public interface IStation
{
    string Name { get;}
    Wafer Pick(int now);
    void Place(Wafer wafer, int now);
}