using JsonData;


class Person
{
    public string name          { get; set; }
    private string password;
    public static bool IsHuman = true;
    public Dictionary<string, List<string>> favorites { get; set; }
    public Person? parent   { get; set; }

    public Person(string name, string password, Person? parent = null)
    {
	    this.name = name;
	    this.password = password;
	    this.parent = parent;
    }

    public void Speak()
    {
	    Console.WriteLine($"I am {name} and i like {favorites.Count} things!");
    }
}


class Program
{
    static void Main(string[] args)
    {
        var Bea = new Person("Bea", "123456");
        
        Bea.favorites = new Dictionary<string, List<string>>()
        {
	        {"Some names", ["Alfred", "John" ]},
	        {"Some foods", ["Bean"  , "Pizza"]}
        };
        var Fred = new Person("Fred", "azertyuiop");

        Bea.parent = Fred;
        
        JsonData.JsonSave.AddObjectToSave(Bea, "../save.dat");
        JsonData.JsonSave.AddObjectToSave(Fred, "../save.dat");
        JsonData.JsonSave.Save();
        
        Person? a = JsonLoad.Load<Person>("../save.dat", 0);
    }
}