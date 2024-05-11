public static class GameEnd
{
    private static bool winStatus;
    private static float score;
    public static bool GetWinStatus()
    {
        return winStatus;
    }

    public static void SetWinStatus(bool status)
    {
        winStatus = status;
    }
    public static void AddScore(float value)
    {
        score += value;
    }
    public static void SetScore(float value)
    {
        score = value;
    }

    public static float GetScore()
    {
        return score;
    }


}
