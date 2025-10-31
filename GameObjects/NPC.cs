using System.Numerics;
using StardewValleyClone.Graphics;
using StardewValleyClone.Engine;

namespace StardewValleyClone.GameObjects;

public class NPC : GameObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Dialog> Dialogs { get; set; } = new();
    public Dictionary<string, int> Relationship { get; set; } = new(); // Player ID -> Friendship level
    public NPCSchedule Schedule { get; set; }
    public List<string> LikedGifts { get; set; } = new();
    public List<string> DislikedGifts { get; set; } = new();
    public bool IsMarriageable { get; set; } = false;

    private int _currentDialogIndex = 0;
    private TimeManager _timeManager;
    private Vector2 _targetPosition;
    private float _moveSpeed = 100f;

    public NPC(string name, TimeManager timeManager)
    {
        Name = name;
        _timeManager = timeManager;
        Schedule = new NPCSchedule();
    }

    public override void Update(double deltaTime)
    {
        // Update position based on schedule
        UpdateSchedule();

        // Move towards target position
        if (Vector2.Distance(Position, _targetPosition) > 5f)
        {
            var direction = Vector2.Normalize(_targetPosition - Position);
            Position += direction * _moveSpeed * (float)deltaTime;
        }
    }

    private void UpdateSchedule()
    {
        var currentTime = _timeManager.Hour * 60 + _timeManager.Minute;
        var schedulePoint = Schedule.GetLocationAtTime(currentTime);

        if (schedulePoint != null)
        {
            _targetPosition = schedulePoint.Position;
        }
    }

    public Dialog GetCurrentDialog()
    {
        if (_currentDialogIndex >= Dialogs.Count)
            _currentDialogIndex = 0;

        return Dialogs.Count > 0 ? Dialogs[_currentDialogIndex] : new Dialog("...");
    }

    public void AdvanceDialog()
    {
        _currentDialogIndex = (_currentDialogIndex + 1) % Math.Max(1, Dialogs.Count);
    }

    public void AddFriendship(string playerId, int amount)
    {
        if (!Relationship.ContainsKey(playerId))
            Relationship[playerId] = 0;

        Relationship[playerId] = Math.Clamp(Relationship[playerId] + amount, 0, 2500); // Max 10 hearts * 250 points
    }

    public int GetFriendshipLevel(string playerId)
    {
        return Relationship.GetValueOrDefault(playerId, 0);
    }

    public int GetHearts(string playerId)
    {
        return GetFriendshipLevel(playerId) / 250; // 250 points = 1 heart
    }
}

public class Dialog
{
    public string Text { get; set; }
    public List<DialogOption>? Options { get; set; }
    public string? NextDialogId { get; set; }

    public Dialog(string text)
    {
        Text = text;
    }
}

public class DialogOption
{
    public string Text { get; set; }
    public string NextDialogId { get; set; }
    public Action? OnSelect { get; set; }

    public DialogOption(string text, string nextDialogId)
    {
        Text = text;
        NextDialogId = nextDialogId;
    }
}

public class NPCSchedule
{
    private List<SchedulePoint> _schedulePoints = new();

    public void AddSchedulePoint(int timeInMinutes, Vector2 position, string activity)
    {
        _schedulePoints.Add(new SchedulePoint
        {
            TimeInMinutes = timeInMinutes,
            Position = position,
            Activity = activity
        });

        _schedulePoints = _schedulePoints.OrderBy(s => s.TimeInMinutes).ToList();
    }

    public SchedulePoint? GetLocationAtTime(int timeInMinutes)
    {
        SchedulePoint? currentPoint = null;

        foreach (var point in _schedulePoints)
        {
            if (timeInMinutes >= point.TimeInMinutes)
                currentPoint = point;
            else
                break;
        }

        return currentPoint;
    }
}

public class SchedulePoint
{
    public int TimeInMinutes { get; set; }
    public Vector2 Position { get; set; }
    public string Activity { get; set; } = "";
}
