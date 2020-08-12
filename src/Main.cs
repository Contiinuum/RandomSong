using MelonLoader;
using UnityEngine;
using Harmony;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.Events;

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

        public static class BuildInfo
        {
            public const string Name = "RandomSong";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "Continuum"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "0.1.0"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }

        public override void OnApplicationStart()
        {
            var i = HarmonyInstance.Create("RandomSong");
            Hooks.ApplyHooks(i);
        }

        public static void CreateSongPanelButton()
        {
            buttonsBeingCreated = true;
            filterMainButton = GameObject.FindObjectOfType<MainMenuPanel>().buttons[1];
            
                     
            randomSongButton = CreateButton(filterMainButton, "Random Song", OnRandomSongShot, randomButtonPos, randomButtonRot, randomButtonScale);
            panelButtonsCreated = true;
        }

        public static void SetRandomSongButtonActive(bool active)
        {
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
            SongSelect songSelect = GameObject.FindObjectOfType<SongSelect>();
            int maxLength = songSelect.mSongButtons.Count -1;
            var rand = new System.Random();
            songSelect.mSongButtons[rand.Next(maxLength)].OnSelect();
        }
    }
}



