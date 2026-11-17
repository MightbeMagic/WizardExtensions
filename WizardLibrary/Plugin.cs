using BepInEx;
using DressmakerMods;
using Framework;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;


namespace WizardLibrary
{
    [BepInPlugin("ambrosia.wizardlibrary", "Wizard Library", "0.1.0")]
    [BepInDependency("yuu.dressmaker.mods")]
    public class Plugin : BaseUnityPlugin
    {
        private ModContext mod;

        private DialogueRunner yarn;
        private Image nightBackground;
        public string Folder;
        private void Awake()
        {
            mod = ModLoaderApi.Register(this, new ModInfo { Author = "AmbrosiaFae", Name = "Wizard Library" });
            Folder = Path.GetDirectoryName(Info.Location);

            ModLifecycleApi.Subscribe(mod, ModLifecycleEvent.SaveLoaded, x => InitLibrary());
        }

        public void InitLibrary()
        {
            nightBackground = Resources.FindObjectsOfTypeAll<Image>().FirstOrDefault(x => x.name == "BackgroundNight");

            yarn = Resources.FindObjectsOfTypeAll<DialogueRunner>().FirstOrDefault();
            yarn.AddCommandHandler("SetNight", ChangeToNight);

            yarn.AddCommandHandler("SetDay", ChangeToDay);
            Logger.LogInfo("Wizard Library set up complete.");
        }


        [YarnCommand("SetNight")]
        public void ChangeToNight()
        {
            
            nightBackground.color = Color.white.WithAlpha(1f);
        }

        [YarnCommand("SetDay")]
        public void ChangeToDay()
        {
            nightBackground.color = Color.white.WithAlpha(0f);
        }
    }
}
