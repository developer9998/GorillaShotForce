using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace GorillaShotForce.Patches
{
    [HarmonyPatch(typeof(Slingshot), "LateUpdateLocal")]
    public class GrabPatch
    {
        public static void Prefix(Slingshot __instance)
        {
            if (__instance.itemState == TransferrableObject.ItemStates.State2 || __instance.itemState == TransferrableObject.ItemStates.State3)
            {
                Rigidbody rb = Player.Instance.bodyCollider.attachedRigidbody;
                Vector3 velocity = rb.velocity * 0.995f;

                Player.Instance.currentVelocity = velocity;
                rb.velocity = velocity;
                rb.AddForce(Physics.gravity * -0.4f * rb.mass * Player.Instance.scale);
            }
        }
    }
}
