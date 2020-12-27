using Rocket.API;

namespace GY.DayNight
{
    public class Config : IRocketPluginConfiguration
    {
        public int VoteInterval;
        public int VoteTime;
        public bool AutoVoteForNight;
        public bool AutoVoteForDay;
        public decimal SuccessfulVotePercent;

        public void LoadDefaults()
        {
            VoteInterval = 120;
            VoteTime = 20;
            AutoVoteForNight = false;
            AutoVoteForDay = true;
            SuccessfulVotePercent = 60;
        }
    }
}