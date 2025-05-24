using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB
{
    public static class KBModules
    {
        public const string VERSION = "V250524";

        public static Config Config;
        public static ScoreBoard ScoreBoard;
        public static ScoreApi ScoreApi;
        
        public static KeyboardManager KeyboardManager;
        public static MouseManager MouseManager;

        public static ViewManager ViewManager;

        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static SpriteBatch SpriteBatch;

        public static Random GameplayRNG;
    }
}
