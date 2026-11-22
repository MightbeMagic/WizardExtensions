using BepInEx;
using DressmakerMods;
using Dummiesman;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using Newtonsoft.Json;

namespace WizardLibrary
{
    [BepInPlugin("ambrosia.wizardlibrary", "Wizard Library", "0.1.0")]
    [BepInDependency("yuu.dressmaker.mods")]
    public class Plugin : BaseUnityPlugin
    {
        private ModContext mod;

        private DialogueRunner yarn;
        private Image NightBackground;
        private Image CharacterPortrait;
        private TextMeshProUGUI CharacterName;
        public string Folder;
        public string ModLoaderFolder;
        public List<SpellList> Spells;

        //Audio
        private IDisposable ready;
        private IDisposable hiddenTrack;
        private List<AssetBundle> bundles;
        private List<string> currentCustomSoundtracks;
        private List<IDisposable> addedTracks;
        

        private void Awake()
        {
            mod = ModLoaderApi.Register(this, new ModInfo { Author = "AmbrosiaFae", Name = "Wizard Library" });

            Folder = Path.GetDirectoryName(Info.Location);
            var file = new FileInfo(Folder);
            //Navigates to Yuuma's Modloader Mod folder
            ModLoaderFolder = Path.Combine(file.Directory.Parent.Parent.ToString(), "Mods");
            currentCustomSoundtracks = new List<string>();
            addedTracks = new List<IDisposable>();
            bundles = new List<AssetBundle>();
            ModLifecycleApi.Subscribe(mod, ModLifecycleEvent.SaveLoaded, x => InitLibrary());
            Spells = new List<SpellList>();

        }

        private void LoadSpells()
        {
            DirectoryInfo dir = new DirectoryInfo(ModLoaderFolder);
            var spelllists = dir.GetFiles( "spelllist.json", SearchOption.AllDirectories);

            Logger.LogInfo("LOADING SPELLS");
            foreach (var item in spelllists)
            {
                var stream = File.ReadAllText(item.FullName);
                try
                {
                    SpellList? spell = JsonConvert.DeserializeObject<SpellList>(stream);
                    spell.filePath = item.Directory.FullName;
                    Logger.LogInfo($"spell list for {spell.mod} loaded\n" + spell);
                    Logger.LogInfo(spell.filePath);
                    Spells.Add(spell);
                }
                catch(Exception e)
                {
                    Logger.LogInfo(e);
                }
            }
        }

        public void InitLibrary()
        {
            NightBackground = Resources.FindObjectsOfTypeAll<Image>().FirstOrDefault(x => x.name == "BackgroundNight");
            var character = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(x => x.name == "Character");
            CharacterPortrait = character.GetChildObjects().First(x => x.name == "Sprite").GetComponent<Image>();
            CharacterName = character.GetChildObjects().First(x => x.name == "Name").GetComponent<TextMeshProUGUI>();
            yarn = Resources.FindObjectsOfTypeAll<DialogueRunner>().FirstOrDefault();
            yarn.AddCommandHandler("SetNight", ChangeToNight);
            yarn.AddCommandHandler("SetDay", ChangeToDay);
            yarn.AddCommandHandler<string, string, string>("SetNpcPortrait", SetNpcPortrait);

            LoadSpells();
            //yarn.AddCommandHandler<string, string>("SetSongFromFile", SetSongFromFile);
            //yarn.AddCommandHandler("ReturnToDefaultSoundtrack", ReturnToDefaultSoundtrack);
            Logger.LogInfo("Wizard Library set up complete.");
        }


        [YarnCommand("SetNight")]
        public void ChangeToNight()
        {
            
            NightBackground.color = Color.white.WithAlpha(1f);
        }

        [YarnCommand("SetDay")]
        public void ChangeToDay()
        {
            NightBackground.color = Color.white.WithAlpha(0f);
        }

        [YarnCommand("SetNpcPortrait")]
        public void SetNpcPortrait(string modname, string npcName, string expression)
        {
            var mod = RetrieveSpellList(modname);

            if (mod.IsNull())
            {
                Logger.LogError($"{modname} is missing from spelllist! ensure the spelllist.json file exists for ${modname}.");
            }

            Logger.LogInfo(mod.GetNpcExpression(npcName, expression));
            try
            {
                CharacterName.text = mod.GetNpcName(npcName);
                Texture2D tst = ImageLoader.LoadTexture(Path.Combine(mod.filePath, mod.GetNpcExpression(npcName, expression)));

                var temp = Sprite.Create(tst, new Rect(0, 0, tst.width, tst.height), new Vector2(0.5f, 0.5f), 100.0f);

                CharacterPortrait.sprite = temp;
            }
            catch (Exception e)
            {
                Logger.LogInfo(e.ToString());
            }
        }

        public SpellList RetrieveSpellList(string modName)
        {
            return Spells.FirstOrDefault(x => x.mod == modName);
        }

        /*
        [YarnCommand("SetSongFromFile")]
        public void SetSongFromFile(string fileName, string songName)
        {
            try
            {
                if (fileName.IsNullOrWhiteSpace())
                {
                    Logger.LogError("File Path was invalid - could not load music.");
                }

                if (!currentCustomSoundtracks.Contains($"wizardlibrary.{songName}"))
                {
                    var newBundle = AssetBundle.LoadFromFile(Path.Combine(ModLoaderFolder, fileName));
                    newBundle.name = $"{fileName}";
                    bundles.AddIfNotContained(newBundle);
                }

                AudioClip clip = bundles.First(x => x.name.Equals(fileName)).LoadAsset<AudioClip>(songName);
                if (clip == null)
                {
                    Logger.LogError($"Could not load music from {fileName}/{songName}.");
                    return;
                }
                ready = SoundtrackModApi.WhenReady(() =>
                {
                    currentCustomSoundtracks.Add($"wizardlibrary.{songName}");
                    addedTracks.AddIfNotContained(SoundtrackModApi.RegisterTrack($"wizardlibrary.{songName}", clip));
                    AudioClip[] originals = SoundtrackModApi.GetOriginalTracks();
                    if (originals.Length > 0)
                        hiddenTrack = SoundtrackModApi.SuppressTrack(originals[0]);

                });
            }
            catch (Exception e)
            {
                Logger.LogInfo(e.ToString());
            }

            
        }

        [YarnCommand("ReturnToDefaultSoundtrack")]
        public void ReturnToDefaultSoundtrack()
        {
            if (addedTracks.IsEmpty())
            {
                Logger.LogError("No custom soundtrack playing - skipping.");
                return;
            }
            ready = SoundtrackModApi.WhenReady(() =>
            {
                foreach (var item in currentCustomSoundtracks)
                {
                    SoundtrackModApi.RemoveTrack(item);
                }
                foreach (var item in addedTracks)
                {
                    item.Dispose();
                }

                hiddenTrack.Dispose();
            });
        }
        */
        private void OnDestroy()
        {
            ready?.Dispose();
            hiddenTrack?.Dispose(); 
            foreach (var item in addedTracks)
            {
                item.Dispose();
            }
            foreach (var item in bundles)
            {
                item.Unload(false);
            }
        }

    }
}
