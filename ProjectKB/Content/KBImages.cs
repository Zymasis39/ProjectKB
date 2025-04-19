using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Content
{
    public static class KBImages
    {
        public static Texture2D NULL1;
        public static Texture2D WHITE1;

        public static Texture2D GP_BACKGROUND;
        public static Texture2D GP_BOARD;
        public static Texture2D GP_TILE1;
        public static Texture2D GP_TILE2;
        public static Texture2D GP_TILE3;
        public static Texture2D GP_TILE4;
        public static Texture2D GP_TILE5;
        public static Texture2D GP_TILE6;
        public static Texture2D GP_TILEG;
        public static Texture2D GP_IND_HOR;
        public static Texture2D GP_IND_VER;
        public static Texture2D GP_IND_SPAWN_BIG;
        public static Texture2D GP_IND_SPAWN;
        public static Texture2D GP_IND_GARBAGE;

        static KBImages()
        {
            NULL1 = new Texture2D(KBModules.GraphicsDeviceManager.GraphicsDevice, 1, 1);
            NULL1.SetData<byte>(new byte[] { 0, 0, 0, 0 });

            WHITE1 = new Texture2D(KBModules.GraphicsDeviceManager.GraphicsDevice, 1, 1);
            WHITE1.SetData<byte>(new byte[] { 255, 255, 255, 0 });
        }
    }
}
