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

        private BMFTypesetData versionTypeset;

        private List<BMFTypesetData> scoreTypesets;

        public MainMenuView()
        {
            DLM = new(new DrawLayer());
            DLM.AddToLayer(this, 0);
            scoreTypesets = new();
        }

        public override void OnLoadContent()
        {
            line1Typeset = KBFonts.SAEADA_600_96.Typeset($"PRESS {KBModules.Config.keybinds[KeyAction.MenuEnter]} TO START");
            for (int i = 0; i < ScoreBoard.N_SCORES; i++)
            {
                if (i < KBModules.ScoreBoard.scores[GamePresetID.EXPERT].Count)
                {
                    GameResult score = KBModules.ScoreBoard.scores[GamePresetID.EXPERT][i];
                    scoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - {score.playerName} - LEVEL {score.level:f3}"));
                }
                else scoreTypesets.Add(KBFonts.SAEADA_600_96.Typeset($"{i + 1} - ???"));
            }
            versionTypeset = KBFonts.SAEADA_600_96.Typeset(KBModules.VERSION);
        }

        public void Draw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            float x = (vp.Width - line1Typeset.width * 0.5f) / 2, y = vp.Height / 2 - 96;
            foreach (BMFTypesetGlyph glyph in line1Typeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            y = vp.Height / 2 - 32;
            foreach (BMFTypesetData typeset in scoreTypesets)
            {
                x = (vp.Width - typeset.width * 0.25f) / 2;
                foreach (BMFTypesetGlyph glyph in typeset.glyphs)
                {
                    Vector2 position = glyph.offset.ToVector2() * 0.25f + new Vector2(x, y);
                    KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                        0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                }
                y += 24;
            }
            x = 16;
            y = vp.Height - (int)(line1Typeset.height * 0.25) - 16;
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
                if (ka == KeyAction.MenuEnter)
                {
                    KBModules.ViewManager.SwitchView(KBModules.ViewManager.gameplayView);
                    KBModules.ViewManager.gameplayView.InitGame();
                }
            }
        }
    }
}
