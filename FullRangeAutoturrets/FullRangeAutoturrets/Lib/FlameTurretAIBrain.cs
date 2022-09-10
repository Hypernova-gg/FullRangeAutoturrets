using System;
using System.Collections.Generic;
using FullRangeAutoturrets.Lib.Logging;
using HarmonyTests.Lib;
using UnityEngine;

namespace FullRangeAutoturrets.Lib
{
    public class FlameTurretAIBrain : SingletonComponent<FlameTurretAIBrain>
    {
        /// <summary>
        /// Initializes the custom AI brain as a component we can transpile into stuff
        /// </summary>
        internal static void Initialize()
        {
            new GameObject().AddComponent<FlameTurretAIBrain>();
        }
        
        /// <summary>
        /// Check if the player is vanished by checking the player's current networking state
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>Bool to indicate whether or not the player is on limited networking, signifying the vanish plugin vanished state</returns>
        internal bool PlayerIsVanished(BasePlayer player) => Helpers.GetFieldValue<bool>(player, "_limitedNetworking");
        
        /// <summary>
        /// Borrowed from the AutoTurret class and refactored a bit
        /// </summary>
        /// <param name="instance">The FlameTurret instance itself</param>
        /// <param name="potentialtarget">The potential (player) target to check</param>
        /// <returns></returns>
        internal float AngleToTarget(FlameTurret instance, BaseCombatEntity potentialtarget)
        {
            BasePlayer basePlayer = potentialtarget as BasePlayer;
            Transform centerEyes = instance.eyeTransform;
            Vector3 to = Vector3Ex.Direction2D(basePlayer.eyes.position, centerEyes.position);
            return Vector3.Angle(centerEyes.forward.XZ3D().normalized, to);
        }
        
        /// <summary>
        /// Method to detect players in the vicinity of the turret, and check if they are within the turret's field of view
        /// </summary>
        /// <param name="turret">FlameTurret instance</param>
        /// <param name="maxRangeFromOrigin">Should be whatever is configured in the config</param>
        /// <returns>List of base entities to put into the trigger var</returns>
        internal List<BaseEntity> DetectPlayersInRange(FlameTurret turret, float maxRangeFromOrigin = 90f)
        {
            var players = new List<BasePlayer>();
            
            // This gets all players within the configured range without checking for visibility
            Vis.Entities(turret.transform.position, 9f, players);

            float realMaxRange = maxRangeFromOrigin / 2.0f; // 90 degrees = 45 degrees each side
            
            var playersInLineOfSight = new List<BaseEntity>();
            foreach (var player in players)
            {
                // If a player is not actually there (dead, etc) then skip them
                if (player.IsNpc || player.IsSleeping() || player.IsDead() || player.IsBuildingAuthed() || PlayerIsVanished(player))
                    continue;
                
                // If the player is within the turret's field of view (feet or eyes), add them to the list
                if (turret.IsVisible(player.transform.position, 9f) || turret.IsVisible(player.eyes.position, 9f))
                {
                    float angleToTargetFromOrigin = AngleToTarget(turret, player);
                    bool isWithinRange = (float)Math.Floor(angleToTargetFromOrigin) <= (float)Math.Ceiling(realMaxRange);
                    if (isWithinRange)
                        playersInLineOfSight.Add((BaseEntity)player);
                }
            }
            
            return playersInLineOfSight;
        }
        
        /// <summary>
        /// The method we're transpiling into the original CheckTarget method, basically replacing targetting behavior
        /// </summary>
        /// <param name="instance">The flameturret we're meddling with</param>
        /// <returns>A bool to determine if the turret has found a target</returns>
        internal bool EvalTargetsInRange(FlameTurret instance)
        {
            float maxRangeFromOrigin = (float)Main.instance.Config.Get("FlameTurrets.DetectRange");
            
            List<BaseEntity> playersFound = DetectPlayersInRange(instance, maxRangeFromOrigin);
            if (playersFound.Count == 0)
            {
                instance.SetTriggered(false);
                instance.SendNetworkUpdateImmediate();
                return false;
            }

            instance.trigger.entityContents = new HashSet<BaseEntity>(playersFound);
            instance.SetTriggered(true);
            instance.SendNetworkUpdateImmediate();
            return true;
        } 
    }
}