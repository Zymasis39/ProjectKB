using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Font;
using ProjectKB.Gameplay;
using ProjectKB.Modules;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Views
{
    public class GameplayView : BaseView, IKBDrawable
    {
        private GameBoard board;
        private int ox = 2, oy = 2;
        private bool paused = false, over = false;

        private BMFTypesetData labelPaused;
        private BMFTypesetData labelGameOver;
        private BMFTypesetData labelSubmitScore;
        private BMFTypesetData labelSubmitScoreSuccess;
        private BMFTypesetData labelSubmitScoreError;
        private BMFTypesetData labelPressExit;

        private List<BMFTypesetData> topLines = new();

        public DrawLayer layer { get; set; }

        public GameplayView()
        {
            DLM = new(
                new DrawLayer(),
                new DrawLayer(),
                new DrawLayer(),
                new DrawLayer()
            );
            DLM.AddToLayer(this, 3);
        }

        public override void OnLoadContent()
        {
            DLM.SetLayerEffect(0, KBEffects.RECOLOR);
            DLM.SetLayerEffect(1, KBEffects.RECOLOR);
            DLM.SetLayerEffect(2, KBEffects.RECOLOR);

            labelPaused = KBFonts.SAEADA_600_96.Typeset("PAUSED");
            labelGameOver = KBFonts.SAEADA_600_96.Typeset("GAME OVER");
            labelSubmitScore = KBFonts.SAEADA_600_96.Typeset("SUBMITTING SCORE...");
            labelSubmitScoreSuccess = KBFonts.SAEADA_600_96.Typeset("SCORE SUBMITTED");
            labelSubmitScoreError = KBFonts.SAEADA_600_96.Typeset("ERROR SUBMITTING SCORE");
            labelPressExit = KBFonts.SAEADA_600_96.Typeset($"PRESS {KBModules.Config.keybinds[KeyAction.Exit]} TO RETURN");
        }

        public void InitGame(GamePresetID presetId)
        {
            if (board != null)
            {
                board.RemoveFromDraw();
            }

            board = new GameBoard(GamePreset.Get(presetId));
            DLM.AddToLayer(board, 0);

            over = false;
            paused = false;
            topLines = new();
        }

        public override void OnSwitch()
        {
            KBModules.KeyboardManager.LoadKeyActionList(KeyActionLists.GameplayAbsolute);
        }

        public override void Update(GameTime gt)
        {
            Queue<KeyAction> kaq = KBModules.KeyboardManager.PassQueue();

            if (board == null) return;

            if (!(paused || over)) board.TimedUpdate(gt);
            board.UpdateAnim(gt);

            while (kaq.TryDequeue(out KeyAction ka))
            {
                int pox = ox, poy = oy;
                if (ka == KeyAction.Pause)
                {
                    if (!over)
                    {
                        paused = !paused;
                        topLines.Clear();
                        if (paused)
                        {
                            topLines.Add(labelPaused);
                            topLines.Add(labelPressExit);
                        }
                    }
                }
                else if (ka == KeyAction.Exit)
                {
                    if (paused || over) KBModules.ViewManager.SwitchView(KBModules.ViewManager.mainMenuView);
                }
                if (paused || over) continue;
                switch (ka)
                {
                    case KeyAction.MoveLeft:
                        board.Move(oy, Direction.Left);
                        break;
                    case KeyAction.MoveRight:
                        board.Move(oy, Direction.Right);
                        break;
                    case KeyAction.MoveUp:
                        board.Move(ox, Direction.Up);
                        break;
                    case KeyAction.MoveDown:
                        board.Move(ox, Direction.Down);
                        break;
                    case KeyAction.PickRow1:
                        oy = 0;
                        break;
                    case KeyAction.PickRow2:
                        oy = 1;
                        break;
                    case KeyAction.PickRow3:
                        oy = 2;
                        break;
                    case KeyAction.PickRow4:
                        oy = 3;
                        break;
                    case KeyAction.PickRow5:
                        oy = 4;
                        break;
                    case KeyAction.PickColumn1:
                        ox = 0;
                        break;
                    case KeyAction.PickColumn2:
                        ox = 1;
                        break;
                    case KeyAction.PickColumn3:
                        ox = 2;
                        break;
                    case KeyAction.PickColumn4:
                        ox = 3;
                        break;
                    case KeyAction.PickColumn5:
                        ox = 4;
                        break;
                }
                if (ox != pox)
                {
                    board.UpdateIndHor(ox);
                }
                if (oy != poy)
                {
                    board.UpdateIndVer(oy);
                }
            }
            board.AwardPoints();
            if (board.CheckGameOver() && !over)
            {
                over = true;
                bool onlineEnabled = KBModules.Config.server != "NONE";
                topLines.Add(labelGameOver);
                if (onlineEnabled) topLines.Add(labelSubmitScore);
                topLines.Add(labelPressExit);
                GameResult result = board.GenerateResult();
                result.SavePlaintextFile();
                if (onlineEnabled) TrySubmitScore(result);
                KBModules.ScoreBoard.AddScore(result, out _);
                KBModules.ScoreBoard.Save();                

                KBModules.ViewManager.mainMenuView.reloadLocalScores = true;
            }
        }

        private async Task TrySubmitScore(GameResult result)
        {
            try
            {
                await KBModules.ScoreApi.SubmitScore(result);
                topLines[topLines.FindIndex(e => e == labelSubmitScore)] = labelSubmitScoreSuccess;
                KBModules.ScoreBoard.scoresOnline[result.preset].a = FetchStatus.Outdated;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                topLines[topLines.FindIndex(e => e == labelSubmitScore)] = labelSubmitScoreError;
            }
        }

        public void PrepDraw()
        {
            
        }

        public void Draw()
        {
            float y = 16;
            foreach (var line in topLines)
            {
                line.Draw(0, y, viewportX: 0.5f, alignX: 0.5f, scale: 0.25f, color: Color.White);
                y += 24;
            }
        }
    }
}
