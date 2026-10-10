const int num_wafers = 25;

int waferCount;
while (true)
{
    Console.Write("Enter number of wafers : ");
    string? input = Console.ReadLine();
    
    if (int.TryParse(input, out waferCount) && waferCount >=1 && waferCount <= num_wafers)
        break;
    Console.WriteLine($"Please enter a whole number between 1 and {num_wafers}.");
}

Console.WriteLine();
new Scheduler(waferCount).Run();

