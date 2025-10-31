using System.Numerics;
using StardewValleyClone.Engine;

namespace StardewValleyClone.Systems;

public class FishingSystem
{
    private TimeManager _timeManager;
    private Random _random = new();
    private Dictionary<string, FishData> _fishDatabase = new();

    private bool _isFishing = false;
    private float _fishingTimer = 0;
    private float _timeToHook = 0;
    private bool _fishHooked = false;
    private string _currentFish = "";

    public bool IsFishing => _isFishing;
    public bool FishHooked => _fishHooked;

    public FishingSystem(TimeManager timeManager)
    {
        _timeManager = timeManager;
        LoadFishDatabase();
    }

    private void LoadFishDatabase()
    {
        // Spring fish
        AddFish(new FishData
        {
            Id = "sunfish",
            Name = "Sunfish",
            Difficulty = 1,
            MinSize = 4,
            MaxSize = 10,
            SellPrice = 30,
            Seasons = new() { Season.Spring, Season.Summer },
            TimeOfDay = TimeOfDay.Any,
            Weather = Weather.Sunny,
            Locations = new() { "River", "Lake" }
        });

        AddFish(new FishData
        {
            Id = "catfish",
            Name = "Catfish",
            Difficulty = 3,
            MinSize = 12,
            MaxSize = 30,
            SellPrice = 200,
            Seasons = new() { Season.Spring, Season.Fall },
            TimeOfDay = TimeOfDay.Any,
            Weather = Weather.Rainy,
            Locations = new() { "River" }
        });

        AddFish(new FishData
        {
            Id = "bass",
            Name = "Bass",
            Difficulty = 2,
            MinSize = 10,
            MaxSize = 25,
            SellPrice = 100,
            Seasons = new() { Season.Spring, Season.Summer, Season.Fall },
            TimeOfDay = TimeOfDay.Any,
            Weather = Weather.Any,
            Locations = new() { "Lake" }
        });

        // Legendary fish
        AddFish(new FishData
        {
            Id = "legend",
            Name = "Legend",
            Difficulty = 10,
            MinSize = 40,
            MaxSize = 60,
            SellPrice = 5000,
            Seasons = new() { Season.Spring },
            TimeOfDay = TimeOfDay.Any,
            Weather = Weather.Rainy,
            Locations = new() { "Mountain Lake" },
            IsLegendary = true
        });
    }

    private void AddFish(FishData fish)
    {
        _fishDatabase[fish.Id] = fish;
    }

    public void StartFishing(string location)
    {
        _isFishing = true;
        _fishingTimer = 0;
        _fishHooked = false;
        _timeToHook = _random.Next(2, 8); // 2-8 seconds

        Console.WriteLine("Started fishing...");
    }

    public void Update(double deltaTime)
    {
        if (!_isFishing) return;

        _fishingTimer += (float)deltaTime;

        if (!_fishHooked && _fishingTimer >= _timeToHook)
        {
            _fishHooked = true;
            Console.WriteLine("Fish hooked! Press C to reel in!");
        }
    }

    public Item? AttemptCatch(string location)
    {
        if (!_fishHooked)
        {
            _isFishing = false;
            return null;
        }

        // Get available fish for current conditions
        var availableFish = GetAvailableFish(location);

        if (availableFish.Count == 0)
        {
            _isFishing = false;
            _fishHooked = false;
            Console.WriteLine("Nothing caught...");
            return null;
        }

        // Select random fish
        var fish = availableFish[_random.Next(availableFish.Count)];

        // Calculate size
        int size = _random.Next(fish.MinSize, fish.MaxSize + 1);

        // Success chance based on difficulty (simplified)
        float successChance = 1.0f - (fish.Difficulty * 0.05f);

        if (_random.NextDouble() < successChance)
        {
            _isFishing = false;
            _fishHooked = false;

            var caughtFish = new Item(fish.Id, fish.Name, ItemType.Fish, true, 1);
            caughtFish.SellPrice = fish.SellPrice;

            Console.WriteLine($"Caught a {fish.Name}! Size: {size} inches");
            return caughtFish;
        }
        else
        {
            _isFishing = false;
            _fishHooked = false;
            Console.WriteLine("The fish got away!");
            return null;
        }
    }

    private List<FishData> GetAvailableFish(string location)
    {
        var currentSeason = _timeManager.Season;
        var currentHour = _timeManager.Hour;

        return _fishDatabase.Values
            .Where(f => f.Locations.Contains(location))
            .Where(f => f.Seasons.Contains(currentSeason))
            .Where(f => f.TimeOfDay == TimeOfDay.Any ||
                       (f.TimeOfDay == TimeOfDay.Morning && currentHour >= 6 && currentHour < 12) ||
                       (f.TimeOfDay == TimeOfDay.Afternoon && currentHour >= 12 && currentHour < 18) ||
                       (f.TimeOfDay == TimeOfDay.Evening && currentHour >= 18 && currentHour < 24))
            .ToList();
    }

    public void CancelFishing()
    {
        _isFishing = false;
        _fishHooked = false;
        _fishingTimer = 0;
    }
}

public class FishData
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Difficulty { get; set; } = 1; // 1-10
    public int MinSize { get; set; }
    public int MaxSize { get; set; }
    public int SellPrice { get; set; }
    public List<Season> Seasons { get; set; } = new();
    public TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Any;
    public Weather Weather { get; set; } = Weather.Any;
    public List<string> Locations { get; set; } = new();
    public bool IsLegendary { get; set; } = false;
}

public enum TimeOfDay
{
    Any,
    Morning,    // 6 AM - 12 PM
    Afternoon,  // 12 PM - 6 PM
    Evening,    // 6 PM - 12 AM
    Night       // 12 AM - 6 AM
}

public enum Weather
{
    Any,
    Sunny,
    Rainy,
    Snowy
}
