using MelonLoader;
using UnityEngine;
using Harmony;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Timers;

namespace AudicaModding
{
    public class AudicaMod : MelonMod
    {

        public static bool panelButtonsCreated = false;
        public static bool buttonsBeingCreated = false;

        private static GameObject filterMainButton = null;
        private static GameObject randomSongButton = null;

        private static Vector3 randomButtonPos = new Vector3(10.2f, -9.4f, 24.2f);
        private static Vector3 randomButtonRot = new Vector3(0f, 0f, 0f);
        private static Vector3 randomButtonScale = new Vector3(2f, 2f, 2f);

        private static int randomSongBagSize = 10;
        private static int mainSongCount = 33;
        private static bool availableSongListsSetup = false;
        private static List<int> availableMainSongs = new List<int>();
        private static List<int> availableExtrasSongs = new List<int>();
        private static List<int> lastPickedSongs = new List<int>();
        private static List<int> availableSongs = new List<int>();

        private static SongSelect songSelect = null;

        private static Il2CppSystem.Collections.Generic.List<SongSelectItem> songs = new Il2CppSystem.Collections.Generic.List<SongSelectItem>();

        public static class BuildInfo
        {
            public const string Name = "RandomSong";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "Continuum"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "0.1.0"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }

        private void CreateConfig()
        {
            ModPrefs.RegisterPrefInt("RandomSong", "RandomSongBagSize", randomSongBagSize);

        }

        private void LoadConfig()
        {
            randomSongBagSize = ModPrefs.GetInt("RandomSong", "RandomSongBagSize");
            if (randomSongBagSize > mainSongCount) randomSongBagSize = mainSongCount;

        }

        public static void SaveConfig()
        {
            ModPrefs.SetInt("RandomSong", "RandomSongBagSize", randomSongBagSize);
        }

        public override void OnLevelWasLoaded(int level)
        {

            if (!ModPrefs.HasKey("RandomSong", "RandomSongBagSize"))
            {
                CreateConfig();
            }
            else
            {
                LoadConfig();

            }
        }


        public override void OnApplicationStart()
        {
            HarmonyInstance.Create("RandomSong");
        }

        public static void CreateSongPanelButton()
        {
            buttonsBeingCreated = true;
            filterMainButton = GameObject.FindObjectOfType<MainMenuPanel>().buttons[1];
                                
            randomSongButton = CreateButton(filterMainButton, "Random Song", OnRandomSongShot, randomButtonPos, randomButtonRot, randomButtonScale);
            panelButtonsCreated = true;
            SetRandomSongButtonActive(true);
        }

        public static IEnumerator SetRandomSongButtonActive(bool active)
        {
            if (active) yield return new WaitForSeconds(.65f);
            else yield return null;
            randomSongButton.SetActive(active);
        }

        private static void OnRandomSongShot()
        {
            GetRandomSong();
        }

        private static GameObject CreateButton(GameObject buttonPrefab, string label, Action onHit, Vector3 position, Vector3 eulerRotation, Vector3 scale)
        {
            GameObject buttonObject = UnityEngine.Object.Instantiate(buttonPrefab);
            buttonObject.transform.rotation = Quaternion.Euler(eulerRotation);
            buttonObject.transform.position = position;
            buttonObject.transform.localScale = scale;

            UnityEngine.Object.Destroy(buttonObject.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = buttonObject.GetComponentInChildren<TextMeshPro>();
            buttonText.text = label;
            GunButton button = buttonObject.GetComponentInChildren<GunButton>();
            button.destroyOnShot = false;
            button.disableOnShot = false;
            button.SetSelected(false);
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(onHit);

            return buttonObject.gameObject;
        }

        private static void GetRandomSong()
        {
            songSelect = GameObject.FindObjectOfType<SongSelect>();
            songs = songSelect.songSelectItems.mItems;
            int maxLength = songs.Count - 1;
            if (!availableSongListsSetup)
            {
                availableSongListsSetup = true;

                for(int i = 0; i < mainSongCount; i++)
                {
                    availableMainSongs.Add(i);
                }

                for(int i = mainSongCount; i < maxLength; i++)
                {
                    availableExtrasSongs.Add(i);
                }

                for(int i = 0; i < maxLength; i++)
                {
                    availableSongs.Add(i);
                }
            }
            SongSelect.Filter filter = songSelect.GetListFilter();

            var rand = new System.Random();
            int index;
            if(filter == SongSelect.Filter.All)
            {
                index = availableSongs[rand.Next(0, availableSongs.Count - 1)];
            }
            else if(filter == SongSelect.Filter.Main)
            {
                index = availableMainSongs[rand.Next(0, availableMainSongs.Count - 1)];
                if(availableMainSongs.Count > 0) availableMainSongs.Remove(index);
            }
            else
            {
                index = availableExtrasSongs[rand.Next(0, availableExtrasSongs.Count - 1)];
                if (availableExtrasSongs.Count > 0) availableExtrasSongs.Remove(index);
            }
            songs[index].OnSelect();
            lastPickedSongs.Add(index);
            if (availableSongs.Count > 0) availableSongs.Remove(index);
            
           
            if (lastPickedSongs.Count > randomSongBagSize)
            {
                int oldestIndex = lastPickedSongs[0];
                lastPickedSongs.Remove(oldestIndex);
                availableSongs.Add(oldestIndex);
                if (oldestIndex < 33) availableMainSongs.Add(index);
                else availableExtrasSongs.Add(index);
            }
        }
    }
}



