using BepInEx;

namespace Framesaver
{
    [BepInPlugin("com.gaylatea.framesaver", "SPT-Framesaver", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public Plugin()
        {
            // Here we experiment with just straight disabling shit to save FPS.

            // This seems to disable a bunch of expensive rigid body physics
            // calculations that don't have much of an effect on gameplay.
            GClass672.GClass674.Enabled = false;

            new DontSpawnShellsFiringPatch().Enable();
            new DontSpawnShellsJamPatch().Enable();
            new DontSpawnShellsAtAllReallyPatch().Enable();

            new AmbientLightOptimizeRenderingPatch().Enable();
            new AmbientLightDisableFrequentUpdatesPatch().Enable();
            
            new WeaponSoundPlayerDisablePatch().Enable();
        }
    }
}