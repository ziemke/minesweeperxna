using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineSweeper.Classes
{
    public class Field
    {
        public Block[,] blocks;
        Random random;
        public Point HoveredBlock = new Point(6, 2);
        public TimeSpan time = new TimeSpan();
        public int UnFlaggedMinesCount;

        public Field()
        {
            random = new Random();
            blocks = new Block[Globals.BLOCKS_HORIZONTAL, Globals.BLOCKS_VERTICAL];
            UnFlaggedMinesCount = Globals.MINES_COUNT;
            FillField();
        }

        private void FillField()
        {
            // First fill in emptyblocks
            for (int x = 0; x <= Globals.BLOCKS_HORIZONTAL - 1; x++)
            {
                for (int y = 0; y <= Globals.BLOCKS_VERTICAL - 1; y++)
                {
                   blocks[x, y] = new Block(BLOCKTYPE.Empty, new Point(x, y));
                }
            }

            //Try to fill in the mines
            for (int i = 0; i < Globals.MINES_COUNT; i++)
            {
                Point desiredIndex = new Point(random.Next(Globals.BLOCKS_HORIZONTAL), random.Next(Globals.BLOCKS_VERTICAL));
                do
                {
                    desiredIndex = new Point(random.Next(Globals.BLOCKS_HORIZONTAL), random.Next(Globals.BLOCKS_VERTICAL));
                   blocks[desiredIndex.X, desiredIndex.Y].Index = desiredIndex;
               } while (blocks[desiredIndex.X, desiredIndex.Y].BlockType != BLOCKTYPE.Empty && blocks[desiredIndex.X, desiredIndex.Y].BlockType != BLOCKTYPE.Mine);


               blocks[desiredIndex.X, desiredIndex.Y].BlockType = BLOCKTYPE.Mine;
            }

            //Finally calculate the hints
            int nearbyMINES_COUNT;
            for (int x = 0; x <= Globals.BLOCKS_HORIZONTAL - 1; x++)
            {
                for (int y = 0; y <= Globals.BLOCKS_VERTICAL - 1; y++)
                {
                    if (blocks[x, y].BlockType == BLOCKTYPE.Empty)
                    {
                        nearbyMINES_COUNT = 0;
                        for (int cx = x - 1; cx <= x + 1; cx++)
                        {
                            for (int cy = y - 1; cy <= y + 1; cy++)
                            {
                                if (cx < 0 || cy < 0 || cx >= Globals.BLOCKS_HORIZONTAL || cy >= Globals.BLOCKS_VERTICAL)
                                    continue;

                                if (blocks[cx, cy].BlockType == BLOCKTYPE.Mine)
                                    nearbyMINES_COUNT++;
                            }
                        }

                        if (nearbyMINES_COUNT > 0)
                        {
                            switch (nearbyMINES_COUNT)
                            {
                                case 1:
                                    blocks[x, y].BlockType = BLOCKTYPE.One;
                                    break;

                                case 2:
                                    blocks[x, y].BlockType = BLOCKTYPE.Two;
                                    break;

                                case 3:
                                    blocks[x, y].BlockType = BLOCKTYPE.Three;
                                    break;

                                default:
                                    break;
                            }
                        }

                    }
                }
            }

        }

        public Block GetBlock(Point index)
        {
            return blocks[index.X, index.Y];
        }

        public void ReveilSurroundingEmptyblocks(Block centerBlock)
        {
            centerBlock.Uncovered = true;

            int x = centerBlock.Index.X;
            int y = centerBlock.Index.Y;

            for (int cx = x - 1; cx <= x + 1; cx++)
            {
                for (int cy = y - 1; cy <= y + 1; cy++)
                {
                    if (cx < 0 || cy < 0 || cx > Globals.BLOCKS_HORIZONTAL - 1 || cy > Globals.BLOCKS_VERTICAL - 1)
                        continue;

                    Block surroundingBlock = GetBlock(new Point(cx, cy));
                    if (surroundingBlock.BlockType == BLOCKTYPE.Empty && !surroundingBlock.Uncovered && !surroundingBlock.Flagged)
                        ReveilSurroundingEmptyblocks(surroundingBlock);
                    else if (surroundingBlock.BlockType != BLOCKTYPE.Mine && !surroundingBlock.Flagged)
                    {
                        surroundingBlock.Uncovered = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.textureFieldFill, new Rectangle((int)Globals.Position.X, (int)Globals.Position.Y, Globals.BLOCKS_HORIZONTAL * (Globals.BLOCK_WIDTH + 1) + 1, Globals.BLOCKS_VERTICAL * (Globals.BLOCK_HEIGHT + 1) + 1), Color.Black);

            for (int x = 0; x < Globals.BLOCKS_HORIZONTAL; x++)
            {
                for (int y = 0; y < Globals.BLOCKS_VERTICAL; y++)
                {
                    blocks[x, y].Draw(spriteBatch);
                }
            }
        }

        public void ReveilAllBlocks()
        {
            for (int x = 0; x < Globals.BLOCKS_HORIZONTAL; x++)
            {
                for (int y = 0; y < Globals.BLOCKS_VERTICAL; y++)
                {
                    blocks[x, y].Uncovered = true; 
                }
            }
            HoveredBlock = new Point(-1, -1);
        }
    }
}
