namespace StardewValleyClone.Engine;

public class TimeManager
{
    // In-game time (Stardew Valley style: 6:00 AM to 2:00 AM)
    private int _currentHour = 6;
    private int _currentMinute = 0;
    private int _currentDay = 1;
    private Season _currentSeason = Season.Spring;
    private int _currentYear = 1;

    // Time progression (10 minutes in-game = 7 seconds real time in Stardew Valley)
    private const double REAL_SECONDS_PER_GAME_MINUTE = 0.7;
    private double _timeAccumulator = 0;

    // Events
    public event Action<int, int>? OnTimeChanged;
    public event Action<int>? OnDayChanged;
    public event Action<Season>? OnSeasonChanged;
    public event Action<int>? OnYearChanged;

    public int Hour => _currentHour;
    public int Minute => _currentMinute;
    public int Day => _currentDay;
    public Season Season => _currentSeason;
    public int Year => _currentYear;
    public string TimeString => $"{_currentHour:D2}:{_currentMinute:D2}";
    public string DateString => $"{_currentSeason} {_currentDay}, Year {_currentYear}";

    public void Update(double deltaTime)
    {
        _timeAccumulator += deltaTime;

        if (_timeAccumulator >= REAL_SECONDS_PER_GAME_MINUTE)
        {
            _timeAccumulator -= REAL_SECONDS_PER_GAME_MINUTE;
            AdvanceTime(10); // 10 minutes per tick
        }
    }

    private void AdvanceTime(int minutes)
    {
        _currentMinute += minutes;

        if (_currentMinute >= 60)
        {
            _currentMinute = 0;
            _currentHour++;

            if (_currentHour >= 26) // 2:00 AM (26:00)
            {
                _currentHour = 6; // Reset to 6:00 AM
                AdvanceDay();
            }
        }

        OnTimeChanged?.Invoke(_currentHour, _currentMinute);
    }

    private void AdvanceDay()
    {
        _currentDay++;

        if (_currentDay > 28) // Stardew Valley has 28 days per season
        {
            _currentDay = 1;
            AdvanceSeason();
        }

        OnDayChanged?.Invoke(_currentDay);
    }

    private void AdvanceSeason()
    {
        switch (_currentSeason)
        {
            case Season.Spring:
                _currentSeason = Season.Summer;
                break;
            case Season.Summer:
                _currentSeason = Season.Fall;
                break;
            case Season.Fall:
                _currentSeason = Season.Winter;
                break;
            case Season.Winter:
                _currentSeason = Season.Spring;
                AdvanceYear();
                break;
            default:
                _currentSeason = Season.Spring;
                break;
        }

        OnSeasonChanged?.Invoke(_currentSeason);
    }

    private void AdvanceYear()
    {
        _currentYear++;
        OnYearChanged?.Invoke(_currentYear);
    }

    public void SetTime(int hour, int minute)
    {
        _currentHour = hour;
        _currentMinute = minute;
        OnTimeChanged?.Invoke(_currentHour, _currentMinute);
    }

    public void SetDate(int day, Season season, int year)
    {
        _currentDay = day;
        _currentSeason = season;
        _currentYear = year;
    }
}

public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}
