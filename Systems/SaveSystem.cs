using System.Numerics;
using Newtonsoft.Json;
using StardewValleyClone.Engine;
using StardewValleyClone.GameObjects;

namespace StardewValleyClone.Systems;

public class SaveSystem
{
    private const string SAVE_FOLDER = "Saves";
    private const string SAVE_FILE_EXTENSION = ".sav";

    public SaveSystem()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public bool SaveGame(string saveName, GameSaveData saveData)
    {
        try
        {
            string filePath = Path.Combine(SAVE_FOLDER, saveName + SAVE_FILE_EXTENSION);
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, json);

            Console.WriteLine($"Game saved successfully: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving game: {ex.Message}");
            return false;
        }
    }

    public GameSaveData? LoadGame(string saveName)
    {
        try
        {
            string filePath = Path.Combine(SAVE_FOLDER, saveName + SAVE_FILE_EXTENSION);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Save file not found: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);
            var saveData = JsonConvert.DeserializeObject<GameSaveData>(json);

            Console.WriteLine($"Game loaded successfully: {filePath}");
            return saveData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading game: {ex.Message}");
            return null;
        }
    }

    public List<string> GetSaveFiles()
    {
        var files = Directory.GetFiles(SAVE_FOLDER, "*" + SAVE_FILE_EXTENSION);
        return files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
    }

    public bool DeleteSave(string saveName)
    {
        try
        {
            string filePath = Path.Combine(SAVE_FOLDER, saveName + SAVE_FILE_EXTENSION);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"Save deleted: {filePath}");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting save: {ex.Message}");
            return false;
        }
    }
}

public class GameSaveData
{
    public string SaveName { get; set; } = "";
    public DateTime SaveDate { get; set; } = DateTime.Now;
    public int PlayTimeMinutes { get; set; } = 0;

    // Player data
    public PlayerSaveData Player { get; set; } = new();

    // World data
    public TimeSaveData Time { get; set; } = new();
    public List<TileSaveData> FarmTiles { get; set; } = new();
    public List<NPCSaveData> NPCs { get; set; } = new();

    // Other data
    public Dictionary<string, bool> Flags { get; set; } = new(); // Quest/event flags
}

public class PlayerSaveData
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Energy { get; set; }
    public int Gold { get; set; }
    public List<ItemSaveData> Inventory { get; set; } = new();
    public int SelectedSlot { get; set; }
}

public class ItemSaveData
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public bool IsStackable { get; set; } = true;
}

public class TimeSaveData
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Day { get; set; }
    public string Season { get; set; } = "";
    public int Year { get; set; }
}

public class TileSaveData
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public string State { get; set; } = "";
    public bool IsWatered { get; set; }
    public CropSaveData? Crop { get; set; }
}

public class CropSaveData
{
    public string Type { get; set; } = "";
    public int DaysGrown { get; set; }
    public int PlantDay { get; set; }
    public string PlantSeason { get; set; } = "";
}

public class NPCSaveData
{
    public string Name { get; set; } = "";
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public Dictionary<string, int> Relationships { get; set; } = new();
}

// Extension methods for easy conversion
public static class SaveDataExtensions
{
    public static PlayerSaveData ToSaveData(this Player player)
    {
        var data = new PlayerSaveData
        {
            PositionX = player.Position.X,
            PositionY = player.Position.Y,
            Energy = player.Energy,
            Gold = player.Gold,
            SelectedSlot = player.Inventory.SelectedSlot
        };

        // Save inventory
        for (int i = 0; i < player.Inventory.Size; i++)
        {
            var item = player.Inventory.GetItem(i);
            if (item != null)
            {
                data.Inventory.Add(new ItemSaveData
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type.ToString(),
                    Quantity = item.Quantity,
                    IsStackable = item.IsStackable
                });
            }
        }

        return data;
    }

    public static TimeSaveData ToSaveData(this TimeManager timeManager)
    {
        return new TimeSaveData
        {
            Hour = timeManager.Hour,
            Minute = timeManager.Minute,
            Day = timeManager.Day,
            Season = timeManager.Season.ToString(),
            Year = timeManager.Year
        };
    }

    public static TileSaveData ToSaveData(this Tile tile)
    {
        var data = new TileSaveData
        {
            PositionX = tile.Position.X,
            PositionY = tile.Position.Y,
            State = tile.State.ToString(),
            IsWatered = tile.IsWatered
        };

        if (tile.Crop != null)
        {
            data.Crop = new CropSaveData
            {
                Type = tile.Crop.Type.ToString(),
                DaysGrown = tile.Crop.DaysGrown,
                PlantDay = tile.Crop.PlantDay,
                PlantSeason = tile.Crop.PlantSeason.ToString()
            };
        }

        return data;
    }

    public static NPCSaveData ToSaveData(this NPC npc)
    {
        return new NPCSaveData
        {
            Name = npc.Name,
            PositionX = npc.Position.X,
            PositionY = npc.Position.Y,
            Relationships = new Dictionary<string, int>(npc.Relationship)
        };
    }
}
