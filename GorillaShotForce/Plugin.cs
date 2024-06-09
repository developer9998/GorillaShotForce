using BepInEx;
using BepInEx.Logging;
using GorillaLocomotion;
using GorillaShotForce.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilla;

namespace GorillaShotForce
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version), ModdedGamemode, BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.12"), BepInDependency("dev.auros.bepinex.bepinject", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private AssetLoader _assetLoader;
        private Configuration _config;

        private bool _inModdedRoom;

        private Harmony _harmony;

        private Player _player;

        private List<AudioClip> _launchSFX;
        private AudioClip _intenseLaunchSFX;

        private float _lastLaunch;

        public void Awake()
        {
            _assetLoader = new AssetLoader();
            _config = new Configuration(Config);

            _harmony = new Harmony(Constants.GUID);
            GorillaTagger.OnPlayerSpawned(Initialize);
        }

        private async void Initialize()
        {
            try
            {
                _player = Player.Instance;

                _launchSFX = new List<AudioClip>()
                {
                    await _assetLoader.LoadAsset<AudioClip>("bird shot-a1"),
                    await _assetLoader.LoadAsset<AudioClip>("bird shot-a2"),
                    await _assetLoader.LoadAsset<AudioClip>("bird shot-a3")
                };
                _intenseLaunchSFX = await _assetLoader.LoadAsset<AudioClip>("bird 01 flying");

                ShotForceEvents.OnLaunch += Launch;
            }
            catch (Exception ex)
            {
                Log(ex.ToString(), LogLevel.Error);
            }
        }

        private void Launch(Vector3 velocity, ForceType force)
        {
            // check for clump compatibility, thank you LX and/or socks for pointing that out !!
            if (force == ForceType.Clump && !_config.NearbyClump.Value) return;

            if (Time.realtimeSinceStartup > _lastLaunch)
            {
                _lastLaunch = Time.realtimeSinceStartup + _config.LaunchCooldown.Value;

                velocity *= _config.LaunchMultiplier.Value;
                _player.currentVelocity = velocity;
                _player.bodyCollider.attachedRigidbody.velocity = velocity;

                float distanceMagnitude = Vector3.Distance(_player.transform.position, _player.transform.position + velocity);
                PlaySound(distanceMagnitude > 17.5f);
            }
        }

        private void PlaySound(bool intense)
        {
            GorillaTagger.Instance.offlineVRRig.tagSound.PlayOneShot(_launchSFX[UnityEngine.Random.Range(0, _launchSFX.Count)], 0.6f);
            if (intense) GorillaTagger.Instance.offlineVRRig.tagSound.PlayOneShot(_intenseLaunchSFX, 0.5f);
        }

        public void OnEnable()
        {
            if (_inModdedRoom)
            {
                _harmony ??= new Harmony(Constants.GUID);
                _harmony.PatchAll(typeof(Plugin).Assembly);
            }
        }

        public void OnDisable()
        {
            _harmony?.UnpatchSelf();
        }

        [ModdedGamemodeJoin]
        public void OnJoin()
        {
            _inModdedRoom = true;
            if (enabled)
            {
                _harmony ??= new Harmony(Constants.GUID);
                _harmony.PatchAll(typeof(Plugin).Assembly);
            }
        }

        [ModdedGamemodeLeave]
        public void OnLeave()
        {
            _inModdedRoom = false;
            _harmony?.UnpatchSelf();
        }

        private void Log(object message, LogLevel level = LogLevel.Info) => Logger.Log(level, message);
    }
}
