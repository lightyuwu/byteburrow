namespace BBTests;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var data = new TestSaveable();

        try
        {
            data.LoadFromFile("savedata.tsv");
        }
        catch(FileNotFoundException ex)
        {
            Console.WriteLine("Failed to Load! File Not found!");
            Console.Write("Enter Username:,");
            data.Username = Console.ReadLine();
            data.Coins = Random.Shared.Next(-10, 10+1);
        }
        
        
        Console.WriteLine("Hello, " + data.Username);
        Console.WriteLine("Your coins WERE: " + data.Coins);
        data.Coins += Random.Shared.Next(-10, 10+1);
        Console.WriteLine("Your coins ARE: " + data.Coins);

        data.NeverSaving = 100;
        data.SaveToFile("savedata.tsv");
        data.NeverSaving = 0;
        
        data.LoadFromFile("savedata.tsv");
        
        Console.WriteLine("This should be 0: " + data.NeverSaving);
    }
}