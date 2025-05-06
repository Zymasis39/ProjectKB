using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Modules;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class GameBoard : IKBDrawable
    {
        public const int DIM = 5;
        public const int TILE_TEX_SIZE = 256;

        public static int DIMPX => DIM * TILE_TEX_SIZE;

        public static Point topLeft { get; private set; }
        public static float scale = 0.5f;

        private GameTile[] board;
        private GarbageSpawn[] garbageSpawns;
        private int nGarbageFill = 0; // add when garbage is dropped on non-garbage, subtract when garbage is cleared
        private Queue<Corner> spawnQueue = new Queue<Corner>();
        private CornerSpawnGenerator spawnGenerator = new CornerSpawnGenerator();
        private static int spawnQueueSize = 5;

        private ColumnIndicator ind_hor, ind_ver;
        private CornerSpawnIndicator ind_spawn;
        private ScoreDisplay scoreDisplay;
        private ScoreDisplayV2 scoreDisplayV2;

        public double score = 0;
        private double scoreBank = 0;
        public double peakScore = 0;
        public int level = 0;
        public bool over = false;

        private double gameTime = 0;
        private double levelTime = 0;
        private double nextExtraGarbageTime;
        private double garbageRate = 0;
        private double garbageProgress = 0;
        private double nextGarbageAtProgress = 1;
        private int gas = 0;

        private GamePreset preset;

        private List<double> precalcLevelReqs = new();

        public void InitLevelReqs()
        {
            for (int i = 0; i < 20; i++)
            {
                precalcLevelReqs.Add(preset.levelReq(i + 1));
                //Debug.WriteLine("Level " + (i + 1) + " req: " + preset.levelReq(i + 1));
            }
        }

        public GameBoard(GamePreset preset)
        {
            this.preset = preset;
            board = new GameTile[DIM * DIM];
            garbageSpawns = new GarbageSpawn[DIM * DIM];

            ind_hor = new(false, 2, new string[]
            {
                KBModules.Config.keybinds[KeyAction.PickColumn1].ToString(),
                KBModules.Config.keybinds[KeyAction.PickColumn2].ToString(),
                KBModules.Config.keybinds[KeyAction.PickColumn3].ToString(),
                KBModules.Config.keybinds[KeyAction.PickColumn4].ToString(),
                KBModules.Config.keybinds[KeyAction.PickColumn5].ToString(),
            });
            ind_ver = new(true, 2, new string[]
            {
                KBModules.Config.keybinds[KeyAction.PickRow1].ToString(),
                KBModules.Config.keybinds[KeyAction.PickRow2].ToString(),
                KBModules.Config.keybinds[KeyAction.PickRow3].ToString(),
                KBModules.Config.keybinds[KeyAction.PickRow4].ToString(),
                KBModules.Config.keybinds[KeyAction.PickRow5].ToString(),
            });
            ind_spawn = new(spawnQueueSize);
            // scoreDisplay = new();
            // ScoreDisplay.InitTypeset();

            scoreDisplayV2 = new();

            for (int i = 0; i < spawnQueueSize; i++) EnqueueSpawn();

            InitLevelReqs();

            LevelColorAnimator.InitWithColors(preset.levelColors);

            garbageRate = preset.baseGarbage(0);
            nextGarbageAtProgress = (KBModules.GameplayRNG.NextDouble() + 1) * 0.5;
            nextExtraGarbageTime = preset.firstExtraGarbageTime;

            TrySpawn();
        }

        public DrawLayer layer { get; set; }

        public void TimedUpdate(GameTime gt)
        {
            double delta = gt.ElapsedGameTime.TotalMilliseconds;
            gameTime += delta;
            levelTime += delta;

            // tick down existing garbage spawners
            for (int i = 0; i < DIM*DIM; i++)
            {
                GarbageSpawn gs = garbageSpawns[i];
                if (gs == null) continue;
                if (gs.TickDown(delta)) SpawnGarbage(gs);
            }

            // garbage progress update
            double gpDelta = delta;
            while (levelTime >= nextExtraGarbageTime)
            {
                gas++;
                garbageProgress += (nextExtraGarbageTime - (levelTime - gpDelta)) * garbageRate;
                gpDelta = levelTime - nextExtraGarbageTime;
                garbageRate += preset.extraGarbage(level);
                nextExtraGarbageTime += preset.extraGarbageInterval;
            }
            garbageProgress += gpDelta * garbageRate;

            List<int> pgs = new();
            for (int i = 0; i < DIM * DIM; i++)
            {
                if (board[i] == null && garbageSpawns[i] == null) pgs.Add(i);
            }
            if (pgs.Count == 0)
            {
                for (int i = 0; i < DIM * DIM; i++)
                {
                    if (board[i] != null && board[i].tier > 0 && garbageSpawns[i] == null) pgs.Add(i);
                }
            }
            while (garbageProgress >= nextGarbageAtProgress && pgs.Count > 0)
            {
                int ilgs = KBModules.GameplayRNG.Next(pgs.Count);
                int igs = pgs[ilgs];
                pgs.RemoveAt(ilgs);
                int y = igs / DIM, x = igs % DIM;

                garbageSpawns[y * DIM + x] = new(x, y, preset.garbageDelay(garbageRate));
                // also code to handle the extreme case (massive lagspike in extremely high levels)
                // of all 25 positions being occupied with garbage spawners
                // or worse, 25 spawners being created at once with the same delay "wyd in this situation"
                garbageProgress--;
                nextGarbageAtProgress = (KBModules.GameplayRNG.NextDouble() + 1) * 0.5;
            }

            // score update (decay)
            score *= Math.Pow(Math.E, -delta * preset.scoreDecayRate);
            // scoreDisplay.UpdateTypeset(score, peakScore, level, gameTime, levelTime);

            double prevLevelReq = level == 0 ? 0 : precalcLevelReqs[level - 1];
            double lp = (score - prevLevelReq) / (precalcLevelReqs[level] - prevLevelReq);
            double plp = (peakScore - prevLevelReq) / (precalcLevelReqs[level] - prevLevelReq);
            scoreDisplayV2.Update(lp, plp, level, gameTime, gas, nextExtraGarbageTime - levelTime);
        }

        public void UpdateAnim(GameTime gt) // updates always regardless of pause/game over state
        {
            double delta = gt.ElapsedGameTime.TotalMilliseconds;

            // level color update
            LevelColorAnimator.UpdateToLevel(over ? LevelColorAnimator.GAME_OVER : level, delta);

            // tile animations
            for (int i = 0; i < DIM * DIM; i++)
                if (board[i] != null) board[i].UpdateAnim(delta);
        }

        public void AwardPoints()
        {
            if (scoreBank == 0) return;
            score += scoreBank;
            if (score > peakScore)
            {
                peakScore = score;
                while (peakScore >= precalcLevelReqs[level])
                {
                    level++;
                    levelTime = 0;
                    gas = 0;
                    garbageRate = preset.baseGarbage(level);
                    nextExtraGarbageTime = preset.firstExtraGarbageTime;
                    if (level == precalcLevelReqs.Count) precalcLevelReqs.Add(preset.levelReq(level + 1));
                }
            }
            scoreBank = 0;

            //scoreDisplay.UpdateTypeset(score, peakScore, level, gameTime, levelTime);
            double prevLevelReq = level == 0 ? 0 : precalcLevelReqs[level - 1];
            double lp = (score - prevLevelReq) / (precalcLevelReqs[level] - prevLevelReq);
            double plp = (peakScore - prevLevelReq) / (precalcLevelReqs[level] - prevLevelReq);
            scoreDisplayV2.Update(lp, plp, level, gameTime, gas, nextExtraGarbageTime - levelTime);
        }

        public void Draw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            KBModules.SpriteBatch.Draw(KBImages.GP_BACKGROUND, vp.Bounds, Color.White);
            float step = TILE_TEX_SIZE * scale;

            for (int ix = 0; ix < DIM; ix++)
            {
                float x = topLeft.X + step * ix;
                for (int iy = 0; iy < DIM; iy++)
                {
                    float y = topLeft.Y + step * iy;
                    KBModules.SpriteBatch.Draw(KBImages.GP_BOARD, new Vector2(x, y), null, Color.White,
                        0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
        }

        public void PrepDraw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            int drawOffset = (int)(DIMPX * scale * 0.5f);
            topLeft = new Point(vp.Width / 2 - drawOffset, vp.Height / 2 - drawOffset);
        }

        public GameTile SpawnTile(int x, int y)
        {
            GameTile tile = new GameTile(this, x, y);
            board[y * DIM + x] = tile;
            return tile;
        }

        public bool Move(int c, Direction dir)
        {
            int tilesOTW = 0;
            GameTile firstTile = null;
            bool merged = false, mergedMax = false;
            bool moved = false;
            switch (dir)
            {
                case Direction.Left:
                    for (int ix = 0; ix < DIM; ix++)
                    {
                        GameTile tile = board[c * DIM + ix];
                        if (tile != null)
                        {
                            // merge tiles (TODO restrict merge conditions)
                            if (tilesOTW == 0) firstTile = tile;
                            else if (tilesOTW == 1 && tile.tier == firstTile.tier && tile.tier != 0 && !merged)
                            {
                                tile.OnRemove();
                                board[c * DIM + ix] = null;
                                if (firstTile.tier == GameTile.MAX_TIER)
                                {
                                    // special case for the big merge
                                    mergedMax = true;
                                    scoreBank += Math.Pow(1.5, GameTile.MAX_TIER - 1);
                                }
                                else
                                {
                                    firstTile.tier++;
                                    scoreBank += Math.Pow(1.5, firstTile.tier - 2);
                                }
                                firstTile.OnMerge();
                                moved = true;
                                merged = true;
                                continue;
                            }
                            // move tile to desired position
                            if (ix > tilesOTW)
                            {
                                board[c * DIM + ix] = null;
                                UpdateTilePos(tile, tilesOTW, c);
                                moved = true;
                            }
                            tilesOTW++;
                        }
                    }
                    break;
                case Direction.Right:
                    for (int irx = 0; irx < DIM; irx++)
                    {
                        GameTile tile = board[c * DIM + DIM - 1 - irx];
                        if (tile != null)
                        {
                            if (tilesOTW == 0) firstTile = tile;
                            else if (tilesOTW == 1 && tile.tier == firstTile.tier && tile.tier != 0 && !merged)
                            {
                                tile.OnRemove();
                                board[c * DIM + DIM - 1 - irx] = null;
                                if (firstTile.tier == GameTile.MAX_TIER)
                                {
                                    mergedMax = true;
                                    scoreBank += Math.Pow(1.5, GameTile.MAX_TIER - 1);
                                }
                                else
                                {
                                    firstTile.tier++;
                                    scoreBank += Math.Pow(1.5, firstTile.tier - 2);
                                }
                                firstTile.OnMerge();
                                moved = true;
                                merged = true;
                                continue;
                            }
                            if (irx > tilesOTW)
                            {
                                board[c * DIM + DIM - 1 - irx] = null;
                                UpdateTilePos(tile, DIM - 1 - tilesOTW, c);
                                moved = true;
                            }
                            tilesOTW++;
                        }
                    }
                    break;
                case Direction.Up:
                    for (int iy = 0; iy < DIM; iy++)
                    {
                        GameTile tile = board[iy * DIM + c];
                        if (tile != null)
                        {
                            if (tilesOTW == 0) firstTile = tile;
                            else if (tilesOTW == 1 && tile.tier == firstTile.tier && tile.tier != 0 && !merged)
                            {
                                tile.OnRemove();
                                board[iy * DIM + c] = null;
                                if (firstTile.tier == GameTile.MAX_TIER)
                                {
                                    mergedMax = true;
                                    scoreBank += Math.Pow(1.5, GameTile.MAX_TIER - 1);
                                }
                                else
                                {
                                    firstTile.tier++;
                                    scoreBank += Math.Pow(1.5, firstTile.tier - 2);
                                }
                                firstTile.OnMerge();
                                moved = true;
                                merged = true;
                                continue;
                            }
                            if (iy > tilesOTW)
                            {
                                board[iy * DIM + c] = null;
                                UpdateTilePos(tile, c, tilesOTW);
                                moved = true;
                            }
                            tilesOTW++;
                        }
                    }
                    break;
                case Direction.Down:
                    for (int iry = 0; iry < DIM; iry++)
                    {
                        GameTile tile = board[(DIM - 1 - iry) * DIM + c];
                        if (tile != null)
                        {
                            if (tilesOTW == 0) firstTile = tile;
                            else if (tilesOTW == 1 && tile.tier == firstTile.tier && tile.tier != 0 && !merged)
                            {
                                tile.OnRemove();
                                board[(DIM - 1 - iry) * DIM + c] = null;
                                if (firstTile.tier == GameTile.MAX_TIER)
                                {
                                    mergedMax = true;
                                    scoreBank += Math.Pow(1.5, GameTile.MAX_TIER - 1);
                                }
                                else
                                {
                                    firstTile.tier++;
                                    scoreBank += Math.Pow(1.5, firstTile.tier - 2);
                                }
                                firstTile.OnMerge();
                                moved = true;
                                merged = true;
                                continue;
                            }
                            if (iry > tilesOTW)
                            {
                                board[(DIM - 1 - iry) * DIM + c] = null;
                                UpdateTilePos(tile, c, DIM - 1 - tilesOTW);
                                moved = true;
                            }
                            tilesOTW++;
                        }
                    }
                    break;
            }
            if (mergedMax)
            {
                ClearAllGarbage();
            }
            else if (merged)
            {                
                switch (dir)
                {
                    case Direction.Left:
                        ClearGarbageAround(0, c);
                        break;
                    case Direction.Right:
                        ClearGarbageAround(DIM - 1, c);
                        break;
                    case Direction.Up:
                        ClearGarbageAround(c, 0);
                        break;
                    case Direction.Down:
                        ClearGarbageAround(c, DIM - 1);
                        break;
                }
            }
            if (moved) TrySpawn();
            return moved;
        }

        private void TrySpawn()
        {
            int x, y;
            switch (spawnQueue.Peek())
            {
                case Corner.TL:
                    x = 0; y = 0;
                    break;
                case Corner.TR:
                    x = DIM - 1; y = 0;
                    break;
                case Corner.BL:
                    x = 0; y = DIM - 1;
                    break;
                case Corner.BR:
                    x = DIM - 1; y = DIM - 1;
                    break;
                default:
                    return;
            }
            if (board[y * DIM + x] == null)
            {
                board[y * DIM + x] = new GameTile(this, x, y);
                spawnQueue.Dequeue();
                EnqueueSpawn();
            }
        }

        private void EnqueueSpawn()
        {
            // use with Dequeue except for initialization
            Corner next = spawnGenerator.Next();
            spawnQueue.Enqueue(next);
            ind_spawn.Advance(next);
        }


        private void SpawnGarbage(GarbageSpawn gs)
        {
            GameTile tile = board[gs.y * DIM + gs.x];
            if (tile == null)
            {
                tile = new(this, gs.x, gs.y);
                board[gs.y * DIM + gs.x] = tile;
                nGarbageFill++;
            }
            else if (tile.tier != 0) nGarbageFill++;
            tile.tier = 0;

            // destroy spawner
            gs.layer.RemoveFromLayer(gs);
            garbageSpawns[gs.y * DIM + gs.x] = null;
        }

        private void ClearGarbageAround(int x, int y)
        {
            if (x != 0) ClearGarbageAt(x - 1, y);
            if (x != DIM - 1) ClearGarbageAt(x + 1, y);
            if (y != 0) ClearGarbageAt(x, y - 1);
            if (y != DIM - 1) ClearGarbageAt(x, y + 1);
        }

        private void ClearGarbageAt(int x, int y)
        {
            GameTile tile = board[y * DIM + x];
            if (tile != null && tile.tier == 0)
            {
                board[y * DIM + x] = null;
                tile.OnRemove();
                nGarbageFill--;
            }
        }

        private void ClearAllGarbage()
        {
            for (int i = 0; i < DIM*DIM; i++)
            {
                GameTile tile = board[i];
                if (tile != null && tile.tier == 0)
                {
                    board[i] = null;
                    tile.OnRemove();
                }
            }
            nGarbageFill = 0;
        }

        private void UpdateTilePos(GameTile tile, int x, int y)
        {
            board[y * DIM + x] = tile;
            tile.x = x;
            tile.y = y;
        }

        public void UpdateIndHor(int x) => ind_hor.UpdateC(x);
        public void UpdateIndVer(int y) => ind_ver.UpdateC(y);

        public void RemoveFromDraw()
        {
            layer.RemoveFromLayer(this);
            for (int i = 0; i < board.Length; i++)
            {
                GameTile tile = board[i];
                if (tile == null) continue;
                tile.layer.RemoveFromLayer(tile);
            }
            ind_hor.layer.RemoveFromLayer(ind_hor);
            ind_ver.layer.RemoveFromLayer(ind_ver);
            ind_spawn.layer.RemoveFromLayer(ind_spawn);
            scoreDisplay.layer.RemoveFromLayer(scoreDisplay);
        }

        public bool CheckGameOver()
        {
            for (int i = 0; i < DIM*DIM; i++)
            {
                if (board[i] == null) return false;
            }
            for (int ix = 0; ix < DIM; ix++)
            {
                GameTile t1 = board[ix], t2 = board[ix + DIM];
                if (t1.tier == t2.tier && t1.tier > 0) return false;
                t1 = board[ix + DIM * (DIM - 1)];
                t2 = board[ix + DIM * (DIM - 2)];
                if (t1.tier == t2.tier && t1.tier > 0) return false;
            }
            for (int iy = 0; iy < DIM; iy++)
            {
                GameTile t1 = board[iy * DIM], t2 = board[iy * DIM + 1];
                if (t1.tier == t2.tier && t1.tier > 0) return false;
                t1 = board[iy * DIM + DIM - 1];
                t2 = board[iy * DIM + DIM - 2];
                if (t1.tier == t2.tier && t1.tier > 0) return false;
            }
            over = true;
            return true;
        }

        public GameResult GenerateResult()
        {
            double prevLevelReq = level == 0 ? 0 : precalcLevelReqs[level - 1];
            double floatLevel = level + (peakScore - prevLevelReq) / (precalcLevelReqs[level] - prevLevelReq);
            return new GameResult(preset.id, peakScore, floatLevel, gameTime, KBModules.Config.playerName);
        }
    }
}
