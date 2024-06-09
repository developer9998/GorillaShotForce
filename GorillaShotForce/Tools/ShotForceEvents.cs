using System;
using UnityEngine;

namespace GorillaShotForce.Tools
{
    public class ShotForceEvents
    {
        public static event Action<Vector3, ForceType> OnLaunch;
        public virtual void Launch(Vector3 velocity, ForceType force) => OnLaunch?.Invoke(velocity, force);
    }
}
