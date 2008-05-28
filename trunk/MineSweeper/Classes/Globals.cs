using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineSweeper.Classes
{
    static class Globals
    {
        public const int SCREEN_WIDTH = 240;
        public const int SCREEN_HEIGHT = 320;
        public const int BLOCKS_HORIZONTAL = 9;
        public const int BLOCKS_VERTICAL = 11;
        public const int BLOCK_WIDTH = 23;
        public const int BLOCK_HEIGHT = 23;
        public const int MINES_COUNT = 11;
        public static Vector2 Position;

        static Globals()
        {
            Position = new Vector2(10, 10);
        }


        public static Vector2 ScreenCenter
        {
            get { return new Vector2(SCREEN_WIDTH / 2,SCREEN_HEIGHT / 2); }
        }
	
    }

}
