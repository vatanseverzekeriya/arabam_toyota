using System.Numerics;
using StardewValleyClone.Engine;

namespace StardewValleyClone.Systems;

public class FarmingSystem
{
    private Dictionary<Vector2, Tile> _tiles = new();
    private TimeManager _timeManager;

    public FarmingSystem(TimeManager timeManager)
    {
        _timeManager = timeManager;
        _timeManager.OnDayChanged += OnDayChanged;
    }

    public void TillSoil(Vector2 position)
    {
        if (!_tiles.ContainsKey(position))
        {
            _tiles[position] = new Tile { Position = position, State = TileState.Tilled };
        }
        else if (_tiles[position].State == TileState.Normal)
        {
            _tiles[position].State = TileState.Tilled;
        }
    }

    public bool PlantSeed(Vector2 position, CropType cropType)
    {
        if (_tiles.ContainsKey(position) && _tiles[position].State == TileState.Tilled)
        {
            _tiles[position].Crop = new Crop(cropType, _timeManager.Day, _timeManager.Season);
            _tiles[position].State = TileState.Planted;
            return true;
        }
        return false;
    }

    public bool WaterTile(Vector2 position)
    {
        if (_tiles.ContainsKey(position) && _tiles[position].State == TileState.Planted)
        {
            _tiles[position].IsWatered = true;
            return true;
        }
        return false;
    }

    public Item? HarvestCrop(Vector2 position)
    {
        if (_tiles.ContainsKey(position) && _tiles[position].Crop != null)
        {
            var crop = _tiles[position].Crop!;
            if (crop.IsReadyToHarvest)
            {
                var harvestedItem = new Item(
                    crop.Type.ToString(),
                    crop.Type.ToString(),
                    ItemType.Crop,
                    true,
                    crop.Yield
                );

                _tiles[position].Crop = null;
                _tiles[position].State = TileState.Tilled;

                return harvestedItem;
            }
        }
        return null;
    }

    private void OnDayChanged(int newDay)
    {
        // Update all crops
        foreach (var tile in _tiles.Values)
        {
            if (tile.Crop != null && tile.IsWatered)
            {
                tile.Crop.DaysGrown++;
            }

            // Reset watered state for new day
            tile.IsWatered = false;
        }
    }

    public Tile? GetTile(Vector2 position)
    {
        return _tiles.GetValueOrDefault(position);
    }

    public IEnumerable<Tile> GetAllTiles()
    {
        return _tiles.Values;
    }
}

public class Tile
{
    public Vector2 Position { get; set; }
    public TileState State { get; set; } = TileState.Normal;
    public Crop? Crop { get; set; }
    public bool IsWatered { get; set; } = false;
}

public enum TileState
{
    Normal,
    Tilled,
    Planted
}

public class Crop
{
    public CropType Type { get; set; }
    public int DaysGrown { get; set; } = 0;
    public int DaysToHarvest { get; set; }
    public int PlantDay { get; set; }
    public Season PlantSeason { get; set; }
    public int Yield { get; set; } = 1;

    public bool IsReadyToHarvest => DaysGrown >= DaysToHarvest;

    public Crop(CropType type, int plantDay, Season plantSeason)
    {
        Type = type;
        PlantDay = plantDay;
        PlantSeason = plantSeason;
        DaysToHarvest = GetDaysToHarvest(type);
        Yield = GetYield(type);
    }

    private int GetDaysToHarvest(CropType type)
    {
        return type switch
        {
            CropType.Parsnip => 4,
            CropType.Cauliflower => 12,
            CropType.Potato => 6,
            CropType.Tomato => 11,
            CropType.Corn => 14,
            CropType.Pumpkin => 13,
            CropType.Wheat => 4,
            CropType.Carrot => 3,
            _ => 7
        };
    }

    private int GetYield(CropType type)
    {
        return type switch
        {
            CropType.Potato => Random.Shared.Next(1, 4), // 1-3 potatoes
            CropType.Tomato => Random.Shared.Next(1, 3), // 1-2 tomatoes
            _ => 1
        };
    }
}

public enum CropType
{
    Parsnip,
    Cauliflower,
    Potato,
    Tomato,
    Corn,
    Pumpkin,
    Wheat,
    Carrot
}
