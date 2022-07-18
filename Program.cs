using System;
using System.Collections.Generic;

public class Program
{

    public static List<IPL> list=new List<IPL>();
    public static void Main()
    {
        var importer = new Importer();
        importer.Mappings = new List<KeyValuePair<string, string>> {
        new KeyValuePair<string, string>("Name", "name"),
        new KeyValuePair<string, string>("Team", "team"),
        new KeyValuePair<string, string>("Role", "role"),
        new KeyValuePair<string, string>("Matches", "matches"),
        new KeyValuePair<string, string>("Runs", "runs"),
        new KeyValuePair<string, string>("Average", "average"),
        new KeyValuePair<string, string>("SR", "strikeRate"),
        new KeyValuePair<string, string>("Wickets", "wickets")
        };
        list = importer.Import<IPL>(@"D:\sessions_hw\Assingment2\IPL_Data.csv");
        
        bool showMenu = true;
        while (showMenu)
        {
            showMenu = MainMenu();
        }


    }

    private static bool MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1) Get Match Fixtures List");
        Console.WriteLine("2) Bowlers of any with more than 40 wickets ");
        Console.WriteLine("3) Search a player ");
        Console.WriteLine("4) Highest Wicket Taker and Runscorer ");
        Console.WriteLine("5) Next Gen Players ");
        Console.WriteLine("6) Get top 3 Bowlers Batsmen Allrounders of the season ");
        Console.WriteLine("7) Fantasy Team Match with 11 batsmen score prediction");
        string? input=Console.ReadLine();

        switch (input)
        {
            case "1":
                 Console.WriteLine("Pending");
                return true;
            case "2":
                Console.WriteLine("Name the team ypu want to search for : ");
                string team_name = Console.ReadLine();
                Players players = new Players();
                players.bowlers_with_40_above_wickets(team_name);
                return true;
            case "3":
                Console.WriteLine("Type the name you want to search for");
                string player_name = Console.ReadLine();
                Players player_serach = new Players();
                player_serach.search(player_name);
                return true;
            case "4":
                Console.WriteLine("Type the name of the team you want to search for the top performers");
                string inp = Console.ReadLine();
                Players top_performers_serach = new Players();
                top_performers_serach.Best_Performers(inp);
                return true;
            case "5":
                Players Next_Gen = new Players();
                Next_Gen.NextGen();
                return true;
            case "6":
                Players top_three = new Players();
                top_three.Top_Three_Performers_season();
                return true;
            case "7":
                Teams teams = new Teams();
                teams.fantasy_match();
                return false;
            default:
                return true;
        }
       
    }


}
public class Importer
{
    public List<KeyValuePair<string, string>> Mappings;
    public List<T> Import<T>(string file)
    {
        List<T> list = new List<T>();
        List<string> lines = System.IO.File.ReadAllLines(file).ToList();
        string headerLine = lines[0];
        var headerInfo = headerLine.Split(',').ToList().Select((v, i) => new
        {
            ColName = v,
            ColIndex = i
        });
        Type type = typeof(T);
        var properties = type.GetProperties();
        var dataLines = lines.Skip(1);
        dataLines.ToList().ForEach(line =>
        {
            var values = line.Split(',');
            T obj = (T)Activator.CreateInstance(type);
                //Set values to object properties from csv columns
            foreach (var prop in properties)
            {
                    //find mapping for the prop
                var mapping = Mappings.SingleOrDefault(m => m.Value == prop.Name);
                var colName = mapping.Key;
                var colIndex = headerInfo.SingleOrDefault(s => s.ColName == colName).ColIndex;
                var value = values[colIndex];
                var propType = prop.PropertyType;
                prop.SetValue(obj, Convert.ChangeType(value, propType));
            }
            list.Add(obj);
        });
        return list;
    }
}
public class IPL
{

    public string name { get; set; }
    public string team { get; set; }
    public string role { get; set; }
    public int matches { get; set; }
    public int runs { get; set; }
    public double average { get; set; }
    public double strikeRate { get; set; }
    public int wickets { get; set; }
}
public class Teams { 
    
    

    public void fantasy_match()
    {
        
        var res = Program.list.GroupBy(t => new { team = t.team }).Select(g => new { Average = g.Average(p => p.average), team = g.Key.team });
        var final_res = res.OrderByDescending(x => x.Average).Take(2);
        foreach(var item in final_res)
        {
            Console.WriteLine(item.team + " - " + "Score: " + (int)(item.Average*11));
        }

        Console.ReadLine();

    }

}


public class Players
{
    
    
    public void search (string input)
    {
       var result = Program.list.Where(x => x.name.Contains(input));

        foreach (var item in result)
        {
            Console.WriteLine(item.name);
            Console.WriteLine(item.role);
            Console.WriteLine(item.runs);
            Console.WriteLine(item.average);
            Console.WriteLine("---------------------------------------");
        }
        Console.ReadLine();
    }
    public void bowlers_with_40_above_wickets(string team_name)
    {
        
        var result = from l in Program.list
                     where l.team == team_name
                     where l.wickets >= 40
                     select l;
        
       foreach(var item in result)
        {
            Console.WriteLine(item.name);
        }
       Console.ReadLine();

 }
    public void NextGen()
    {
        var players_with_less_matches = Program.list.Where(x => x.matches <= 23 && x.matches>=6 );
        var Next_Gen_Bowlers = players_with_less_matches.OrderByDescending(x => x.wickets).Take(5);
        var Next_Gen_Batsmen = players_with_less_matches.OrderByDescending(x => x.wickets).Take(5);
        var Next_Gen_All_rounders = players_with_less_matches.Where(x => x.role == "ALL ROUNDER").OrderByDescending(x => (x.wickets * x.average)).Take(5);
        foreach(var p in Next_Gen_Bowlers)
        {
            Console.WriteLine("Next Gen Bowler" + " " + p.name);
        }
        foreach (var p in Next_Gen_Batsmen)
        {
            Console.WriteLine("Next Gen Batsman" + " " + p.name);
        }
        foreach (var p in Next_Gen_All_rounders)
        {
            Console.WriteLine("Next Gen Allrounder"  + " " + p.name);
        }
        Console.ReadLine();
    }
    public void Best_Performers(string input)
    {
        
        var res_bowler = Program.list.Where(t=> t.team==input).OrderByDescending(x => x.wickets).FirstOrDefault();
        var res_batsman = Program.list.Where(t => t.team == input).OrderByDescending(x => x.runs).FirstOrDefault();
        Console.WriteLine(res_bowler.name + " " + res_batsman.name);
        Console.ReadLine();
    }
    public void Top_Three_Performers_season()
    {
       
        var top_three_batsmen= Program.list.OrderByDescending(x => x.runs).Take(3);
        var top_three_bowlers = Program.list.OrderByDescending(x => x.wickets).Take(3);
        var top_three_allrounders = Program.list.OrderByDescending(a => a.runs).ThenBy(a => a.wickets).Take(3);
        foreach(var top in top_three_batsmen)
        {
            Console.WriteLine(top.name);
        }
        foreach (var top in top_three_bowlers)
        {
            Console.WriteLine(top.name);
        }
        foreach (var top in top_three_allrounders) 
        {
            Console.WriteLine(top.name);
        }

        Console.ReadLine();


    }

}
    



    




