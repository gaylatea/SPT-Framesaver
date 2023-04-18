using BepInEx.Configuration;

namespace Config
{
    internal class Bots
    {
        public static ConfigEntry<bool> BrainEnabled { get; private set; }
        public static ConfigEntry<bool> BotEnabled { get; private set; }

        public static void Init(ConfigFile Config)
        {
            string section = "2. Bots";

            BrainEnabled = Config.Bind(section, "Brain Enabled", false, new ConfigDescription("", null, null));
            BotEnabled = Config.Bind(section, "Bot Enabled", false, new ConfigDescription("", null, null));
        }
    }
}