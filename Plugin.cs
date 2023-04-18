using BepInEx;
using EFT;

using HarmonyLib;

using Config;

using Aki.Reflection.Utils;

namespace Framesaver
{
    [BepInPlugin("com.gaylatea.framesaver", "SPT-Framesaver", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public Plugin()
        {
            Profiles.Init(Config);
            Bots.Init(Config);

            // Here we experiment with just straight disabling shit to save FPS.

            // This seems to disable a bunch of expensive rigid body physics
            // calculations that don't have much of an effect on gameplay.
            GClass670.GClass672.Enabled = false;

            new DontSpawnShellsFiringPatch().Enable();
            new DontSpawnShellsJamPatch().Enable();
            new DontSpawnShellsAtAllReallyPatch().Enable();

            new AmbientLightOptimizeRenderingPatch().Enable();
            new AmbientLightDisableFrequentUpdatesPatch().Enable();

            var p = HookObject.AddOrGetComponent<Profiling>();
            // p.EnableOn(typeof(Component.Bot), "BrainUpdate");
            // p.EnableOn(typeof(Component.Bot), "BotUpdate");
            // p.EnableOn(typeof(Component.Bot), "LateUpdate");
            
            // This is an extremely quick method- so bot generation on the
            // server isn't slow at all, from what I can tell.
            // p.EnableOn(AccessTools.TypeByName("Class223"), "LoadBots");

            // These two are the different methods of loading bots from the
            // server, it's worth profiling them to see.
            // p.EnableOn(typeof(GClass831), "method_0");
            // p.EnableOn(typeof(BotsPresets), "method_0");
            p.EnableOn(typeof(Diz.Jobs.JobScheduler), "LateUpdate");

            // Something in these ticks seems to be a hotspot. Let's profile
            // to figure out which they are.
            // p.EnableOn(typeof(LocalPlayer), "LateUpdate");
            // p.EnableOn(typeof(AmbientLight), "LateUpdate");
            // p.EnableOn(typeof(BotControllerClass), "method_0");
            // p.EnableOn(typeof(AICoreControllerClass), "Update");
            // p.EnableOn(typeof(AiTaskManagerClass), "Update");
            // p.EnableOn(typeof(BotsClass), "UpdateByUnity");
            // p.EnableOn(typeof(GClass25<BotLogicDecision>), "Update");
        }
    }
}