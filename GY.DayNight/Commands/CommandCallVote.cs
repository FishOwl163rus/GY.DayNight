using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace GY.DayNight.Commands
{
    public class CommandCallVote : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "CallVote";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>{"gy.call.vote"};
        
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!DayNight.IsAllowVote)
            {
                UnturnedChat.Say(caller, DayNight.Instance.Translate("vote_interval"), Color.red);
                return;
            }
            
            if (command.Length < 1 || !Enum.TryParse(command[0], true, out Time time))
            {
                UnturnedChat.Say(caller, DayNight.Instance.Translate("command_call_vote_invalid"), Color.red);
                return;
            }

            DayNight.Instance.CallVote(time);
        }
    }
}