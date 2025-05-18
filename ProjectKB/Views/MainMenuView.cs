using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Font;
using ProjectKB.Gameplay;
using ProjectKB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectKB.Utils;
using ProjectKBShared.Model;
using System.Diagnostics;
using System.Threading;

namespace ProjectKB.Views
{
    public class MainMenuView : BaseView, IKBDrawable
    {
        public DrawLayer layer { get; set; }

        private BMFTypesetData line1Typeset;

        private List<GamePresetID> presets;
        private List<BMFTypesetData> presetTypesets;
        private int selectedPresetIndex = 0;

        private BMFTypesetData versionTypeset;

        private List<BMFTypesetData> localScoreTypesets;
        private List<BMFTypesetData> onlineScoreTypesets;
        private BMFTypesetData labelLocalScores;
        private BMFTypesetData labelOnlineScores;
        private BMFTypesetData labelNoScores;
        private BMFTypesetData labelLoading;
        private BMFTypesetData labelErrorLoading;
        private BMFTypesetData labelServerDisabled;

        private Mutex onlineScoreTypesetMutex = new();

        public bool reloadLocalScores = true;
        public bool reloadOnlineScores = true;

        public MainMenuView()
        {
            DLM = new(new DrawLayer());
            DLM.AddToLayer(this, 0);
            presets = new List<GamePresetID>((GamePresetID[])Enum.GetValues(typeof(GamePresetID)));
            presetTypesets = new();
            localScoreTypesets = new();
            onlineScoreTypesets = new();
        }

        public override void OnLoadContent()
        {
            line1Typeset = KBFonts.SAEADA_600_96.Typeset($"PRESS {KBModules.Config.keybinds[KeyAction.MenuEnter]} TO START");
            for (int i = 0; i < presets.Count; i++)
            {
                presetTypesets.Add(KBFonts.SAEADA_600_96.Typeset(presets[i].ToString()));
            }
            versionTypeset = KBFonts.SAEADA_600_96.Typeset(KBModules.VERSION);
            labelLocalScores = KBFonts.SAEADA_600_96.Typeset("LOCAL SCORES");
            labelOnlineScores = KBFonts.SAEADA_600_96.Typeset("ONLINE SCORES");
            labelNoScores = KBFonts.SAEADA_600_96.Typeset("NO SCORES");
            labelLoading = KBFonts.SAEADA_600_96.Typeset("LOADING SCORES...");
            labelErrorLoading = KBFonts.SAEADA_600_96.Typeset("ERROR LOADING SCORES");
            labelServerDisabled = KBFonts.SAEADA_600_96.Typeset("ONLINE FEATURES DISABLED");
        }

        private void UpdateLocalScoreTypesets(GamePresetID presetId)
        {
            localScoreTypesets.Clear();
            for (int i = 0; i < ScoreBoard.N_SCORES; i++)
            {
                if (i < KBModules.ScoreBoard.scoresLocal[presetId].Count)
                {
                    GameResult score = KBModules.ScoreBoard.scoresLocal[presetId][i];
                    localScoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - {score.playerName} - LEVEL {score.level:f3}"));
                }
                else localScoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - ???"));
            }
        }

        private void UpdateOnlineScoreTypesets(GamePresetID presetId)
        {
            onlineScoreTypesetMutex.WaitOne();
            onlineScoreTypesets.Clear();
            switch (KBModules.ScoreBoard.scoresOnline[presetId].a)
            {
                case FetchStatus.UpToDate:
                case FetchStatus.Outdated:
                    int j = KBModules.ScoreBoard.scoresOnline[presetId].b.Count;
                    if (j == 0)
                    {
                        onlineScoreTypesets.Add(labelNoScores);
                    }
                    else for (int i = 0; i < j; i++)
                    {
                        DBScore score = KBModules.ScoreBoard.scoresOnline[presetId].b[i];
                        onlineScoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - {score.playerName} - LEVEL {score.level:f3}"));
                    }
                    break;
                case FetchStatus.Loading:
                    onlineScoreTypesets.Add(labelLoading);
                    break;
                case FetchStatus.Error:
                    onlineScoreTypesets.Add(labelErrorLoading);
                    break;
                case FetchStatus.Disabled:
                    onlineScoreTypesets.Add(labelServerDisabled);
                    break;
            }
            onlineScoreTypesetMutex.ReleaseMutex();
        }

        public void Draw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            int y = 16;
            line1Typeset.Draw(16, y, scale: 0.25f, color: Color.White);
            y += 32;
            int i = 0;
            foreach (BMFTypesetData typeset in presetTypesets)
            {
                typeset.Draw(16, y, scale: 0.25f, color: i == selectedPresetIndex ? Color.Lime : Color.White);
                y += 24;
                i++;
            }
            y = 16;
            labelLocalScores.Draw(-16, y, viewportX: 1, alignX: 1, scale: 0.25f, color: Color.White);
            y += 24;
            foreach (BMFTypesetData typeset in localScoreTypesets)
            {
                typeset.Draw(-16, y, viewportX: 1, alignX: 1, scale: 0.25f, color: Color.White);
                y += 24;
            }
            y += 8;
            labelOnlineScores.Draw(-16, y, viewportX: 1, alignX: 1, scale: 0.25f, color: Color.White);
            y += 24;
            onlineScoreTypesetMutex.WaitOne();
            foreach (BMFTypesetData typeset in onlineScoreTypesets)
            {
                typeset.Draw(-16, y, viewportX: 1, alignX: 1, scale: 0.25f, color: Color.White);
                y += 24;
            }
            onlineScoreTypesetMutex.ReleaseMutex();
            versionTypeset.Draw(16, -16, viewportY: 1, alignY: 1, scale: 0.25f, color: Color.White);
        }

        public override void OnSwitch()
        {
            KBModules.KeyboardManager.LoadKeyActionList(KeyActionLists.Menu);
            if (reloadLocalScores)
            {
                UpdateLocalScoreTypesets(presets[selectedPresetIndex]);
                reloadLocalScores = false;
            }
            UpdateOnlineScores();
        }

        private async Task UpdateOnlineScores()
        {
            bool allOutdated = true;
            foreach (var kvp in KBModules.ScoreBoard.scoresOnline)
            {
                if (kvp.Value.a != FetchStatus.Outdated) allOutdated = false;
            }
            if (allOutdated)
            {                
                foreach (var kvp in KBModules.ScoreBoard.scoresOnline)
                {
                    KBModules.ScoreBoard.scoresOnline[kvp.Key].a = FetchStatus.Loading;
                }
                UpdateOnlineScoreTypesets(presets[selectedPresetIndex]);
                try
                {
                    var res = await KBModules.ScoreApi.GetScores();
                    foreach (DBScoresByPreset s in res)
                    {
                        GamePresetID preset = (GamePresetID)s.preset;
                        KBModules.ScoreBoard.scoresOnline[preset].a = FetchStatus.UpToDate;
                        KBModules.ScoreBoard.scoresOnline[preset].b = s.scores;
                    }
                    UpdateOnlineScoreTypesets(presets[selectedPresetIndex]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    foreach (var kvp in KBModules.ScoreBoard.scoresOnline)
                    {
                        KBModules.ScoreBoard.scoresOnline[kvp.Key].a = FetchStatus.Error;
                    }
                    UpdateOnlineScoreTypesets(presets[selectedPresetIndex]);
                }
            }
            else
            {
                foreach (var kvp in KBModules.ScoreBoard.scoresOnline)
                {
                    FetchStatus k = KBModules.ScoreBoard.scoresOnline[kvp.Key].a;
                    if (k == FetchStatus.Outdated)
                    {
                        UpdateOnlineScoresForPreset(kvp.Key);
                    }
                    else if (k == FetchStatus.Disabled && kvp.Key == presets[selectedPresetIndex])
                    {
                        UpdateOnlineScoreTypesets(kvp.Key);
                    }
                }
            }
        }

        private async Task UpdateOnlineScoresForPreset(GamePresetID preset)
        {
            KBModules.ScoreBoard.scoresOnline[preset].a = FetchStatus.Loading;
            if (preset == presets[selectedPresetIndex])
                UpdateOnlineScoreTypesets(preset);
            try
            {
                var res = await KBModules.ScoreApi.GetScoresByPreset(preset);
                KBModules.ScoreBoard.scoresOnline[preset].a = FetchStatus.UpToDate;
                KBModules.ScoreBoard.scoresOnline[preset].b = res;
                if (preset == presets[selectedPresetIndex])
                    UpdateOnlineScoreTypesets(preset);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                KBModules.ScoreBoard.scoresOnline[preset].a = FetchStatus.Error;
                if (preset == presets[selectedPresetIndex])
                    UpdateOnlineScoreTypesets(preset);
            }
        }

        public void PrepDraw()
        {
            
        }

        public override void Update(GameTime gt)
        {
            Queue<KeyAction> kaq = KBModules.KeyboardManager.PassQueue();
            while (kaq.TryDequeue(out KeyAction ka))
            {
                if (ka == KeyAction.MenuDown)
                {
                    selectedPresetIndex = (selectedPresetIndex + 1) % presets.Count;
                    UpdateLocalScoreTypesets(presets[selectedPresetIndex]);
                    UpdateOnlineScoreTypesets(presets[selectedPresetIndex]);
                }
                else if (ka == KeyAction.MenuUp)
                {
                    selectedPresetIndex = (selectedPresetIndex + presets.Count - 1) % presets.Count;
                    UpdateLocalScoreTypesets(presets[selectedPresetIndex]);
                    UpdateOnlineScoreTypesets(presets[selectedPresetIndex]);
                }
                else if (ka == KeyAction.MenuEnter)
                {
                    KBModules.ViewManager.SwitchView(KBModules.ViewManager.gameplayView);
                    KBModules.ViewManager.gameplayView.InitGame(presets[selectedPresetIndex]);
                }
                else if (ka == KeyAction.Exit)
                {
                    KBModules.ViewManager.Exit();
                }
            }
        }
    }
}
