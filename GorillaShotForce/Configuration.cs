using BepInEx.Configuration;

namespace GorillaShotForce
{
    public class Configuration
    {
        public ConfigEntry<float> LaunchMultiplier, LaunchCooldown;
        public ConfigEntry<bool> NearbyClump;

        public Configuration(ConfigFile file)
        {
            LaunchMultiplier = file.Bind("Gameplay", "Launch Multiplier", 1f, "The multiplier for when the player is launched");
            LaunchCooldown = file.Bind("Gameplay", "Launch Cooldown", 0.4f, "The cooldown for when the player is launched");
            NearbyClump = file.Bind("Gameplay", "Player Clump", false, "If the player should be launched when another player is launched nearby");
        }
    }
}
