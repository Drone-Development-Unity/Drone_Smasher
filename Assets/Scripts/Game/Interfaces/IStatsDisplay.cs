namespace Game
{
    /// <summary>
    /// Defines whether object could show info on StatsPanel
    /// </summary>
    public interface IStatsDisplay
    {
        public StatsData GetStats();
    }
}