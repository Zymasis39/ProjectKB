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

        private List<BMFTypesetData> scoreTypesets;

        public MainMenuView()
        {
            DLM = new(new DrawLayer());
            DLM.AddToLayer(this, 0);
            presets = new List<GamePresetID>((GamePresetID[])Enum.GetValues(typeof(GamePresetID)));
            presetTypesets = new();
            scoreTypesets = new();
        }

        public override void OnLoadContent()
        {
            line1Typeset = KBFonts.SAEADA_600_96.Typeset($"PRESS {KBModules.Config.keybinds[KeyAction.MenuEnter]} TO START");
            for (int i = 0; i < presets.Count; i++)
            {
                presetTypesets.Add(KBFonts.SAEADA_600_96.Typeset(presets[i].ToString()));
            }
            UpdateScoreTypesets(presets[selectedPresetIndex]);
            versionTypeset = KBFonts.SAEADA_600_96.Typeset(KBModules.VERSION);
        }

        private void UpdateScoreTypesets(GamePresetID presetId)
        {
            scoreTypesets.Clear();
            for (int i = 0; i < ScoreBoard.N_SCORES; i++)
            {
                if (i < KBModules.ScoreBoard.scores[presetId].Count)
                {
                    GameResult score = KBModules.ScoreBoard.scores[presetId][i];
                    scoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - {score.playerName} - LEVEL {score.level:f3}"));
                }
                else scoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - ???"));
            }
        }

        public void Draw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            float x = 16, y = 16;
            foreach (BMFTypesetGlyph glyph in line1Typeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.25f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            }
            y += 32;
            int i = 0;
            foreach (BMFTypesetData typeset in presetTypesets)
            {
                foreach (BMFTypesetGlyph glyph in typeset.glyphs)
                {
                    Vector2 position = glyph.offset.ToVector2() * 0.25f + new Vector2(x, y);
                    KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect,
                        i == selectedPresetIndex ? Color.Lime : Color.White,
                        0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                }
                y += 24;
                i++;
            }
            y = 16;
            foreach (BMFTypesetData typeset in scoreTypesets)
            {
                x = vp.Width - typeset.width * 0.25f - 16;
                foreach (BMFTypesetGlyph glyph in typeset.glyphs)
                {
                    Vector2 position = glyph.offset.ToVector2() * 0.25f + new Vector2(x, y);
                    KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                        0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                }
                y += 24;
            }
            x = 16;
            y = vp.Height - (int)(versionTypeset.height * 0.25) - 16;
            foreach (BMFTypesetGlyph glyph in versionTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.25f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            }
        }

        public override void OnSwitch()
        {
            KBModules.KeyboardManager.LoadKeyActionList(KeyActionLists.Menu);
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
                    UpdateScoreTypesets(presets[selectedPresetIndex]);
                }
                else if (ka == KeyAction.MenuUp)
                {
                    selectedPresetIndex = (selectedPresetIndex + presets.Count - 1) % presets.Count;
                    UpdateScoreTypesets(presets[selectedPresetIndex]);
                }
                else if (ka == KeyAction.MenuEnter)
                {
                    KBModules.ViewManager.SwitchView(KBModules.ViewManager.gameplayView);
                    KBModules.ViewManager.gameplayView.InitGame(presets[selectedPresetIndex]);
                }
            }
        }
    }
}
