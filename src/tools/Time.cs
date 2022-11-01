namespace soufBot.src.tools;

public static class Time
{
    public static int MINUTES_TO_SECONDS_RATIO = 120;

    public static int CurrentTimeSeconds()
    {
        return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}