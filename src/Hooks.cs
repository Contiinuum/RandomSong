using Harmony;
using System.Reflection;
using System;
using System.Collections.Generic;
using MelonLoader;

namespace AudicaModding
{
    internal static class Hooks
    {

        [HarmonyPatch(typeof(MenuState), "SetState", new Type[] { typeof(MenuState.State) })]
        private static class PatchSetMenuState
        {
            private static void Postfix(MenuState __instance, ref MenuState.State state)
            {
                if (!AudicaMod.panelButtonsCreated)
                {
                    if (!AudicaMod.buttonsBeingCreated && state == MenuState.State.SongPage) AudicaMod.CreateSongPanelButton();
                    return;
                }
                if (state == MenuState.State.SongPage && !AudicaMod.timerSet) AudicaMod.StartTimer();
                else if(state == MenuState.State.LaunchPage || state == MenuState.State.MainPage) AudicaMod.SetRandomSongButtonActive(false);
            }

        }

    }
}
