using System.Numerics;

namespace StardewValleyClone.Systems;

public class MiningSystem
{
    private Dictionary<int, MineLevel> _mineLevels = new();
    private Random _random = new();
    private int _currentFloor = 0;
    private int _deepestFloor = 0;

    public int CurrentFloor => _currentFloor;
    public int DeepestFloor => _deepestFloor;

    public MiningSystem()
    {
        // Initialize first level
        GenerateLevel(0);
    }

    public void GenerateLevel(int floor)
    {
        if (_mineLevels.ContainsKey(floor))
            return;

        var level = new MineLevel
        {
            Floor = floor,
            Rocks = new List<Rock>(),
            Enemies = new List<MineEnemy>(),
            HasLadder = false
        };

        // Generate rocks with minerals
        int rockCount = _random.Next(20, 40);
        for (int i = 0; i < rockCount; i++)
        {
            var rock = new Rock
            {
                Position = new Vector2(_random.Next(0, 50), _random.Next(0, 50)),
                Health = _random.Next(3, 10),
                MineralType = DetermineMineralType(floor)
            };
            level.Rocks.Add(rock);
        }

        // Generate enemies (deeper = more enemies)
        if (floor > 0)
        {
            int enemyCount = _random.Next(1, Math.Min(floor / 5 + 2, 10));
            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = new MineEnemy
                {
                    Type = DetermineEnemyType(floor),
                    Position = new Vector2(_random.Next(0, 50), _random.Next(0, 50)),
                    Health = 10 + floor * 2,
                    Damage = 2 + floor / 10,
                    IsAlive = true
                };
                level.Enemies.Add(enemy);
            }
        }

        _mineLevels[floor] = level;
    }

    private MineralType DetermineMineralType(int floor)
    {
        // Deeper floors have better minerals
        int roll = _random.Next(100);

        if (floor < 40)
        {
            if (roll < 60) return MineralType.Stone;
            if (roll < 85) return MineralType.Copper;
            if (roll < 95) return MineralType.Coal;
            return MineralType.Quartz;
        }
        else if (floor < 80)
        {
            if (roll < 40) return MineralType.Stone;
            if (roll < 65) return MineralType.Iron;
            if (roll < 85) return MineralType.Coal;
            if (roll < 95) return MineralType.Gold;
            return MineralType.Emerald;
        }
        else
        {
            if (roll < 30) return MineralType.Stone;
            if (roll < 50) return MineralType.Gold;
            if (roll < 70) return MineralType.Iridium;
            if (roll < 85) return MineralType.Diamond;
            if (roll < 95) return MineralType.Ruby;
            return MineralType.Prismatic;
        }
    }

    private EnemyType DetermineEnemyType(int floor)
    {
        if (floor < 40) return EnemyType.GreenSlime;
        if (floor < 80) return EnemyType.DustSprite;
        if (floor < 120) return EnemyType.Bat;
        return EnemyType.Shadow;
    }

    public Item? MineRock(Vector2 position, int toolPower)
    {
        var level = _mineLevels.GetValueOrDefault(_currentFloor);
        if (level == null) return null;

        // Find rock at position
        var rock = level.Rocks.FirstOrDefault(r =>
            Vector2.Distance(r.Position, position) < 2f);

        if (rock == null) return null;

        rock.Health -= toolPower;

        if (rock.Health <= 0)
        {
            level.Rocks.Remove(rock);

            // Check if all rocks are cleared to spawn ladder
            if (level.Rocks.Count == 0 && !level.HasLadder)
            {
                level.HasLadder = true;
                level.LadderPosition = position;
                Console.WriteLine("A ladder has appeared!");
            }

            // Return mineral
            return CreateMineralItem(rock.MineralType);
        }

        return null;
    }

    private Item CreateMineralItem(MineralType type)
    {
        var (id, name, price) = type switch
        {
            MineralType.Stone => ("stone", "Stone", 2),
            MineralType.Copper => ("copper_ore", "Copper Ore", 5),
            MineralType.Iron => ("iron_ore", "Iron Ore", 10),
            MineralType.Gold => ("gold_ore", "Gold Ore", 25),
            MineralType.Iridium => ("iridium_ore", "Iridium Ore", 100),
            MineralType.Coal => ("coal", "Coal", 15),
            MineralType.Quartz => ("quartz", "Quartz", 25),
            MineralType.Emerald => ("emerald", "Emerald", 250),
            MineralType.Ruby => ("ruby", "Ruby", 250),
            MineralType.Diamond => ("diamond", "Diamond", 750),
            MineralType.Prismatic => ("prismatic_shard", "Prismatic Shard", 2000),
            _ => ("stone", "Stone", 2)
        };

        var item = new Item(id, name, ItemType.Mineral, true, 1);
        item.SellPrice = price;
        return item;
    }

    public void DescendFloor()
    {
        var level = _mineLevels.GetValueOrDefault(_currentFloor);
        if (level == null || !level.HasLadder) return;

        _currentFloor++;
        _deepestFloor = Math.Max(_deepestFloor, _currentFloor);

        if (!_mineLevels.ContainsKey(_currentFloor))
        {
            GenerateLevel(_currentFloor);
        }

        Console.WriteLine($"Descended to floor {_currentFloor}");
    }

    public void AscendFloor()
    {
        if (_currentFloor > 0)
        {
            _currentFloor--;
            Console.WriteLine($"Ascended to floor {_currentFloor}");
        }
    }

    public bool CanDescend()
    {
        var level = _mineLevels.GetValueOrDefault(_currentFloor);
        return level?.HasLadder ?? false;
    }

    public MineLevel? GetCurrentLevel()
    {
        return _mineLevels.GetValueOrDefault(_currentFloor);
    }
}

public class MineLevel
{
    public int Floor { get; set; }
    public List<Rock> Rocks { get; set; } = new();
    public List<MineEnemy> Enemies { get; set; } = new();
    public bool HasLadder { get; set; }
    public Vector2 LadderPosition { get; set; }
}

public class Rock
{
    public Vector2 Position { get; set; }
    public int Health { get; set; }
    public MineralType MineralType { get; set; }
}

public class MineEnemy
{
    public EnemyType Type { get; set; }
    public Vector2 Position { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public bool IsAlive { get; set; }
}

public enum MineralType
{
    Stone,
    Copper,
    Iron,
    Gold,
    Iridium,
    Coal,
    Quartz,
    Emerald,
    Ruby,
    Diamond,
    Prismatic
}

public enum EnemyType
{
    GreenSlime,
    BlueSlime,
    RedSlime,
    DustSprite,
    Bat,
    Shadow,
    Skeleton
}
