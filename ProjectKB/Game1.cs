using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Font;
using ProjectKB.Gameplay;
using ProjectKB.Modules;
using ProjectKB.Utils;
using ProjectKB.Views;
using System;
using System.Diagnostics;

namespace ProjectKB
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            KBModules.GraphicsDeviceManager = _graphics;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            AppPaths.CreateAppDataFolder();

            KBModules.Config = Config.Load();
            KBModules.Config.Save();

            KBModules.ScoreBoard = ScoreBoard.Load();

            TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / KBModules.Config.fps);

            KBModules.KeyboardManager = new();

            Window.KeyDown += KBModules.KeyboardManager.OnKeyDown;

            KBModules.GameplayRNG = new();

            KBModules.ViewManager = new();

            _graphics.PreferredBackBufferWidth = KBModules.Config.displayWidth;
            _graphics.PreferredBackBufferHeight = KBModules.Config.displayHeight;
            _graphics.IsFullScreen = KBModules.Config.fullscreen;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            EmbRes.LogNames();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            KBModules.SpriteBatch = _spriteBatch;

            // TODO: use this.Content to load your game content here

            KBImages.GP_BACKGROUND = Content.Load<Texture2D>("Sprites/bgcol");
            KBImages.GP_BOARD = Content.Load<Texture2D>("Sprites/base");
            KBImages.GP_TILE1 = Content.Load<Texture2D>("Sprites/tile1");
            KBImages.GP_TILE2 = Content.Load<Texture2D>("Sprites/tile2");
            KBImages.GP_TILE3 = Content.Load<Texture2D>("Sprites/tile3");
            KBImages.GP_TILE4 = Content.Load<Texture2D>("Sprites/tile4");
            KBImages.GP_TILE5 = Content.Load<Texture2D>("Sprites/tile5");
            KBImages.GP_TILE6 = Content.Load<Texture2D>("Sprites/tile6");
            KBImages.GP_TILEG = Content.Load<Texture2D>("Sprites/tileG");
            KBImages.GP_IND_HOR = Content.Load<Texture2D>("Sprites/ind_hor");
            KBImages.GP_IND_VER = Content.Load<Texture2D>("Sprites/ind_ver");
            KBImages.GP_IND_SPAWN_BIG = Content.Load<Texture2D>("Sprites/diamond64");
            KBImages.GP_IND_SPAWN = Content.Load<Texture2D>("Sprites/diamond32");
            KBImages.GP_IND_GARBAGE = Content.Load<Texture2D>("Sprites/garbwarn");

            KBFonts.SAEADA_600_96 = new BitmapFont(EmbRes.GetResourceStream("FontDesc.saeada_600_96_bin.fnt"),
                Content.Load<Texture2D>("Fonts/saeada_600_96_0"));

            GameTile.LoadTextures();

            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);

            KBEffects.ARROWS = Content.Load<Effect>("FX/arrows");
            KBEffects.RECOLOR = Content.Load<Effect>("FX/recolor");

            KBEffects.RECOLOR.Parameters["view_projection"].SetValue(projection);
            KBEffects.RECOLOR.Parameters["rep_color_r"].SetValue(Color.Black.ToVector4());
            KBEffects.RECOLOR.Parameters["rep_color_g"].SetValue(Color.Black.ToVector4());
            KBEffects.RECOLOR.Parameters["light_pos_a"].SetValue(0f);
            KBEffects.RECOLOR.Parameters["light_pos_b"].SetValue((float)GraphicsDevice.Viewport.Height);
            KBEffects.RECOLOR.Parameters["light_fac_a"].SetValue(1.125f);
            KBEffects.RECOLOR.Parameters["light_fac_b"].SetValue(0.75f);

            KBModules.ViewManager.mainMenuView.OnLoadContent();
            KBModules.ViewManager.gameplayView.OnLoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KBModules.ViewManager.currentView.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            KBModules.ViewManager.currentView.DLM.Draw();
            base.Draw(gameTime);
        }
    }
}