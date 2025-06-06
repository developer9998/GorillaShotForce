using GorillaShotForce.Tools;
using HarmonyLib;
using UnityEngine;

namespace GorillaShotForce.Patches
{
    [HarmonyPatch(typeof(SlingshotProjectile), nameof(SlingshotProjectile.Launch))]
    public class LaunchPatch
    {
        private static ShotForceEvents _shotForceEvents;

        public static void Prefix(Vector3 position, Vector3 velocity, NetPlayer player)
        {
            bool isLocal = player.IsLocal;
            bool inClump = !isLocal && Vector3.Distance(GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position, position) <= 1f;
            if (isLocal || inClump)
            {
                _shotForceEvents ??= new ShotForceEvents();
                _shotForceEvents.Launch(velocity, inClump ? ForceType.Clump : ForceType.Self);
            }
        }
    }
}
