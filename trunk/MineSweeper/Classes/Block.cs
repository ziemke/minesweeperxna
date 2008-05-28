using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineSweeper.Classes
{
    public enum BLOCKTYPE { Empty, Mine, One, Two, Three }
    public class Block
    {
        public bool Flagged;
        public bool Uncovered;
        public bool Cross;
        public BLOCKTYPE BlockType;
        public Point Index;

        public Block(BLOCKTYPE blockType, Point index)
        {
            this.BlockType = blockType;
            this.Index = index;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = new Rectangle(1 + (int)Globals.Position.X + Index.X * (Globals.BLOCK_WIDTH + 1), 1 + (int)Globals.Position.Y + Index.Y * (Globals.BLOCK_HEIGHT + 1), Globals.BLOCK_WIDTH, Globals.BLOCK_HEIGHT); 

             if (!Uncovered)
             {
                spriteBatch.Draw(Game1.textureBlockCovered, rect, Color.White);
             } else {
                switch (this.BlockType)
                {
                    case BLOCKTYPE.Empty:
                        spriteBatch.Draw(Game1.textureBlockEmpty, rect, Color.White);
                        break;
                    case BLOCKTYPE.Mine:
                        spriteBatch.Draw(Game1.textureBlockMine, rect, Color.White);
                        break;
                    case BLOCKTYPE.One:
                        spriteBatch.Draw(Game1.textureBlockOne, rect, Color.White);
                        break;
                    case BLOCKTYPE.Two:
                        spriteBatch.Draw(Game1.textureBlockTwo, rect, Color.White);
                        break;
                    case BLOCKTYPE.Three:
                        spriteBatch.Draw(Game1.textureBlockThree, rect, Color.White);
                        break;
                    default:
                        break;
                }
            }

            //Draw the flag if im flagged
            if (this.Flagged)
                spriteBatch.Draw(Game1.textureBlockFlag, rect, Color.White);

            //Draw difused cross
            if(this.Cross)
                spriteBatch.Draw(Game1.textureBlockCross, rect, Color.White);

            //Let's see if the user hovered me
            if (this.Index == Game1.field.HoveredBlock)
                spriteBatch.Draw(Game1.textureFieldFill, rect, (!Uncovered ? new Color(255, 255, 255, 128) : new Color (0, 0, 0, 128)));
        }
    }
}
