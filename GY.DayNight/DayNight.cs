using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Math = System.Math;

namespace GY.DayNight
{
    public class DayNight : RocketPlugin<Config>
    {
        public static DayNight Instance;
        public static Config Cfg;
        private static bool _isAllow = true;
        public static DateTime LastCalledVote;
        public static readonly HashSet<CSteamID> Voters = new HashSet<CSteamID>();
        public static bool IsVoteInProgress;
        public static bool IsAllowVote
        {
            get => (DateTime.Now - LastCalledVote).TotalSeconds >= Cfg.VoteInterval && _isAllow;
            
            private set
            {
                _isAllow = value;
                IsVoteInProgress = !_isAllow;
            }
        }

        protected override void Load()
        {
            Instance = this;
            Cfg = Configuration.Instance;
            IsAllowVote = true;
            
            LightingManager.onDayNightUpdated_ModHook += OnDayNightUpdated;
        }

        private void OnDayNightUpdated(bool isDay)
        {
            switch (isDay)
            {
                case true when Cfg.AutoVoteForNight:
                    CallVote(Time.Night);
                    break;
                case false when Cfg.AutoVoteForDay:
                    CallVote(Time.Day);
                    break;
            }
        }

        public void CallVote(Time time) => StartCoroutine(nameof(StartVote), time);
        
        private IEnumerator StartVote(Time time)
        {
            if (!IsAllowVote) yield break;

            IsAllowVote = false;

            var voters = (byte) Provider.clients.Count;
            
            switch (time)
            {
                case Time.Day:
                    UnturnedChat.Say(Translate("start_day"), Color.magenta);
                    yield return new WaitForSeconds(Cfg.VoteTime);
                    var dayRes = CalculateResult(voters);
                    
                    if (dayRes >= (double) Cfg.SuccessfulVotePercent)
                    {
                        UnturnedChat.Say(Translate("day"), Color.cyan);
                        Commander.execute(CSteamID.Nil, "day");
                    }
                    else
                    {
                        UnturnedChat.Say(Translate("day_fail"), Color.white);
                    }
                    
                    break;
                case Time.Night:
                    UnturnedChat.Say(Translate("start_night"), Color.magenta);
                    yield return new WaitForSeconds(Cfg.VoteTime);
                    var nightRes = CalculateResult(voters);
                    
                    if (nightRes >= (double) Cfg.SuccessfulVotePercent)
                    {
                        UnturnedChat.Say(Translate("night"), Color.cyan);
                        Commander.execute(CSteamID.Nil, "night");
                    }
                    else
                    {
                        UnturnedChat.Say(Translate("night_fail"), Color.white);
                    }

                    break;
                default:
                    yield break;
            }
            
            Voters.Clear();
            LastCalledVote = DateTime.Now;
            IsAllowVote = true;
        }

        private static double CalculateResult(byte voters) => Math.Round((double) Voters.Count / voters * 100, 0);
        

        protected override void Unload()
        {
           StopCoroutine(nameof(StartVote));
           Voters.Clear();
           LastCalledVote = DateTime.Now;
           IsAllowVote = false;
           IsVoteInProgress = false;
           
           // ReSharper disable once DelegateSubtraction
           LightingManager.onDayNightUpdated_ModHook -= OnDayNightUpdated;
        }
        
        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"start_day", "Голосование за День началось! Используйте команду /vote!"},
            {"start_night", "Голосование за Ночь началось! Используйте команду /vote!"},
            {"day", "По результатам голосования решено включить День!"},
            {"day_fail", "Голосование за День провалено!"},
            {"night", "По результатам голосования решено включить Ночь!"},
            {"night_fail", "Голосование за Ночь провалено!"},
            {"vote_interval", "Голосования за смену времени недоступно!"},
            {"global_msg", "Игрок {0} проголосовал за смену времени!"},
            {"command_call_vote_invalid", "Команда введена неверно, используйте /CallVote [day | night]!"},
            {"command_vote_invalid", "Команда введена неверно, используйте /Vote!"},
            {"already_voted", "Вы уже голосовали за смену времени!"}
        };
        
        
    }
}