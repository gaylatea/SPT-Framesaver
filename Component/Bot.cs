using System;
using System.Collections;
using System.Reflection;

using EFT;
using EFT.Game.Spawning;

using UnityEngine;

using Aki.Reflection.Utils;

using static Config.Bots;

namespace Framesaver.Component
{
    public class Bot : MonoBehaviour
    {
        public BotOwner bot;

        public void Awake()
        {
            bot = GetComponent<BotOwner>();
            StartCoroutine(Activate());
        }

        public IEnumerator Activate()
        {
            // We need to wait for the bot's weapon to ready up, which seems
            // to be delayed from its initialization.
            while (!bot.WeaponManager.IsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            bot.Transform.position = bot.BotsGroup.BotZone.SpawnPoints.RandomElement<ISpawnPoint>().Position + Vector3.up * 0.5f;

            var actualActivate = typeof(BotOwner)?.GetMethod("method_10", BindingFlags.Instance | BindingFlags.NonPublic);
            actualActivate.Invoke(bot, null);

            StartCoroutine(Run());
            StartCoroutine(CalculateNewGoals());

            yield return new WaitForEndOfFrame();
        }

        public IEnumerator CalculateNewGoals()
        {
            while (true)
            {
                try
                {
                    bot.CalcGoal();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"On frame {Time.frameCount}, {bot.ProfileId} had a goals issue: {ex}");
                    Console.ResetColor();
                }
                yield return new WaitForSeconds(3.0f);
            }
        }

        public IEnumerator Run()
        {
            // This is the main runloop of the bot. Compatibility with existing
            // AI client-side mods is not currently a priority.
            while (true)
            {
                try
                {
                    BrainUpdate();
                    BotUpdate();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"On frame {Time.frameCount}, {bot.ProfileId} had an update issue: {ex}");
                    Console.ResetColor();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void BrainUpdate()
        {
            if (!BrainEnabled.Value) { return; }

            bot?.Brain?.Agent?.Update();
        }

        public void BotUpdate()
        {
            if (!BotEnabled.Value) { return; }

            // Do not update bots that aren't alive.
            if (bot.BotState != EBotState.Active || !bot.GetPlayer.HealthController.IsAlive) { return; }

            bot.StandBy.Update();
            bot.LookSensor.UpdateLook();

            // Don't update bots that are paused, for whatever reason.
            if (bot.StandBy.StandByType == BotStandByType.paused) { return; }

            bot.ShootData.ManualUpdate();
            bot.DogFight.ManualUpdate();
            bot.RecoilData.LosingRecoil();
            bot.AimingData.PermanentUpdate();

            if (bot.WeaponManager != null)
            {
                bot.WeaponManager.ManualUpdate();
            }

            bot.HeadData.ManualUpdate();
            bot.Tilt.ManualUpdate();
            bot.NightVision.ManualUpdate();
            bot.CellData.Update();
            bot.FriendChecker.ManualUpdate();
            bot.TrianglePosition.ManualUpdate();
            bot.Medecine.ManualUpdate();
            bot.Boss.ManualUpdate();
            bot.BotTalk.ManualUpdate();
            bot.BotRequestController.Update();
            bot.Tactic.UpdateChangeTactics();
            bot.Memory.ManualUpdate(Time.deltaTime);
            bot.Settings.UpdateManual();
            bot.BotRequestController.TryToFind();

            bot.Mover.ManualUpdate();
            bot.Mover.ManualFixedUpdate();
            bot.Steering.ManualFixedUpdate();
        }

        // public void BotLateUpdate()
        // {
        //     // This is a re-implementation of the Player::LateUpdate() that is specific to bots.

        //     // Don't update dead bots at all.
        //     if (bot.GetPlayer.HealthController == null || !bot.GetPlayer.HealthController.IsAlive)
        //     {
        //         return;
        //     }

        //     bot.GetPlayer.MovementContext?.AnimatorStatesLateUpdate();

        //     bot.GetPlayer.Physical.LateUpdate();
        //     bot.GetPlayer.VisualPass();

        //     bot.GetPlayer.ProceduralWeaponAnimation.StartFovCoroutine(bot.GetPlayer);
        //     bot.GetPlayer.PropUpdate();
        //     botComplexUpdateMethod.Invoke(bot.GetPlayer, new object[] { EUpdateQueue.Update, bot.GetPlayer.DeltaTime });
        // }
    }
}