using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace GY.DayNight.Commands
{
    public class CommandVote : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Vote";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>{"gy.vote"};
        
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!DayNight.IsVoteInProgress)
            {
                UnturnedChat.Say(caller, DayNight.Instance.Translate("vote_interval"), Color.red);
                return;
            }

            var voter = (UnturnedPlayer) caller;

            if (DayNight.Voters.Add(voter.CSteamID))
            {
                UnturnedChat.Say(DayNight.Instance.Translate("global_msg", caller.DisplayName), Color.yellow);
                return;
            }
            
            UnturnedChat.Say(caller, DayNight.Instance.Translate("already_voted", caller.DisplayName), Color.red);
        }
    }
}