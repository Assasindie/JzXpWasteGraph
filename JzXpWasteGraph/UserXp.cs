using System;
using System.Collections.Generic;
using System.Text;

namespace JzXpWasteGraph
{
    public class Skill
    {
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
    }

    public class Minigame
    {
        public int Rank { get; set; }
        public int Score { get; set; }
    }

    //have to redefine the player info class so that it deserializes correctly
    public class PlayerInfo
    {
        public string Name { get; set; }
        public Skill Overall { get; set; }
        public Skill Attack { get; set; }
        public Skill Defence { get; set; }
        public Skill Strength { get; set; }
        public Skill Hitpoints { get; set; }
        public Skill Ranged { get; set; }
        public Skill Prayer { get; set; }
        public Skill Magic { get; set; }
        public Skill Cooking { get; set; }
        public Skill Woodcutting { get; set; }
        public Skill Fletching { get; set; }
        public Skill Fishing { get; set; }
        public Skill Firemaking { get; set; }
        public Skill Crafting { get; set; }
        public Skill Smithing { get; set; }
        public Skill Mining { get; set; }
        public Skill Herblore { get; set; }
        public Skill Agility { get; set; }
        public Skill Thieving { get; set; }
        public Skill Slayer { get; set; }
        public Skill Farming { get; set; }
        public Skill Runecrafting { get; set; }
        public Skill Hunter { get; set; }
        public Skill Construction { get; set; }
        public Minigame BountyHunterRogues { get; set; }
        public Minigame BountyHunter { get; set; }
        public Minigame LastManStanding { get; set; }
        public Minigame TotalCluesScrolls { get; set; }
        public Minigame BeginnerClueScrolls { get; set; }
        public Minigame EasyClueScrolls { get; set; }
        public Minigame MediumClueScrolls { get; set; }
        public Minigame HardClueScrolls { get; set; }
        public Minigame EliteClueScrolls { get; set; }
        public Minigame MasterClueScrolls { get; set; }
    }

    class UserXp
    {
        public string DiscordID;
        public DateTime LastUpdate;
        public PlayerInfo Player;
    }
}
