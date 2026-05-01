using Modding;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Ado_Death_Scream
{
    public class Ado_Death_Scream : Mod, ITogglableMod
    {
        private readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private Dictionary<string, AudioClip> screamTracks = new Dictionary<string, AudioClip>();
        internal AssetBundle screamBundle = null;
        AudioSource source;
        public Ado_Death_Scream() : base("Ado Death Scream")
        {
            using (Stream screamStream = assembly.GetManifestResourceStream("Ado_Death_Scream.Resources.adoscreambundle"))
            {
                if (screamStream != null)
                {
                    screamBundle = AssetBundle.LoadFromStream(screamStream);
                    if (screamBundle != null)
                    {
                        Log("Bundle made succesfully");
                        AudioClip[] cliplist = screamBundle.LoadAllAssets<AudioClip>();
                        foreach (AudioClip clip in cliplist)
                        {
                            screamTracks.Add(clip.name, clip);
                        }
                        Log("Done storing Clips");
                    }
                    else Log("Bundle is null");
                }
                else Log("Didn't find screamStream");
            }
            Log("Finished Constructing");
        }
        public override string GetVersion() => "1.2.0";
        private void Hook()
        {
            ModHooks.BeforePlayerDeadHook += DeathScream;
        }
        private void MakeObjects()
        {
            GameObject audioObject = new GameObject("AdoDeathScreamAudio");
            UnityEngine.Object.DontDestroyOnLoad(audioObject);
            source = audioObject.AddComponent<AudioSource>();
        }
        public override void Initialize()
        {
            MakeObjects();
            Hook();
        }
        public void DeathScream()
        {
            if (screamTracks.TryGetValue("adoScream", out AudioClip newClip))
            {
                source.PlayOneShot(newClip);
                Log("Playing Audio");
            }
            else Log("Clip not found");
            
        }

        public void Unload()
        {
            ModHooks.BeforePlayerDeadHook -= DeathScream;
            UnityEngine.Object.Destroy(source.gameObject);
            source = null;
        }
    }
}
