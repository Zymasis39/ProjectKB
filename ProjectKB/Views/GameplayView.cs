using Microsoft.Xna.Framework;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Gameplay;
using ProjectKB.Modules;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Views
{
    public class GameplayView : BaseView
    {
        private GameBoard board;
        private int ox = 2, oy = 2;
        private bool paused = false, over = false;

        public GameplayView()
        {
            DLM = new(
                new DrawLayer(),
                new DrawLayer(),
                new DrawLayer(),
                new DrawLayer()
            );
        }

        public override void OnLoadContent()
        {
            DLM.SetLayerEffect(0, KBEffects.RECOLOR);
            DLM.SetLayerEffect(1, KBEffects.RECOLOR);
            DLM.SetLayerEffect(2, KBEffects.RECOLOR);
        }

        public void InitGame()
        {
            if (board != null)
            {
                board.RemoveFromDraw();
            }

            board = new GameBoard(GamePreset.Get(GamePresetID.EXPERT));
            DLM.AddToLayer(board, 0);
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
                    paused = !paused;
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
                GameResult result = board.GenerateResult();
                result.SavePlaintextFile();
                KBModules.ScoreBoard.AddScore(result, out _);
                KBModules.ScoreBoard.Save();
            }
        }
    }
}
