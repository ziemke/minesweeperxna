using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MineSweeper.Classes;

namespace MineSweeper
{
    public enum GameState
    {
        Menu,
        Playing,
        Lost,
        Won,
        Pause,

    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
     

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D textureHUD, textureFieldFill, textureBlockCovered, textureBlockEmpty, textureBlockOne, textureBlockMine, textureBlockFlag, textureBlockThree, textureBlockTwo, textureBlockCross;
        SpriteFont tahoma;
        static  public Field field;
        /// <summary>
        /// GamePadState References for easier access
        /// </summary>
        GamePadState gamePadState, oldGamePadState;

        /// <summary>
        /// KeyboardState Referencse for easier access
        /// </summary>
        KeyboardState keyboardState, oldKeyboardState;

        GameState gameState = GameState.Playing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            field = new Field();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = Globals.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = Globals.SCREEN_HEIGHT;
            graphics.ApplyChanges();

            textureHUD = Content.Load<Texture2D>("HUD");
            textureFieldFill = Content.Load<Texture2D>("FIELD_FILL");
            textureBlockCovered = Content.Load<Texture2D>("BLOCK_Covered");
            textureBlockEmpty = Content.Load<Texture2D>("BLOCK_Empty");
            textureBlockOne = Content.Load<Texture2D>("BLOCK_One");
            textureBlockMine = Content.Load<Texture2D>("BLOCK_Mine");
            textureBlockTwo = Content.Load<Texture2D>("BLOCK_Two");
            textureBlockThree = Content.Load<Texture2D>("BLOCK_Three");
            textureBlockFlag = Content.Load<Texture2D>("BLOCK_Flag");
            textureBlockCross = Content.Load<Texture2D>("BLOCK_Cross");
            tahoma = Content.Load<SpriteFont>("Tahoma");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);


            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (gameState)
            {
                case GameState.Menu:
                    break;
                case GameState.Playing:

                    //Handle Pause
                    if ((gamePadState.Buttons.Start == ButtonState.Pressed)
                        && oldGamePadState.Buttons.Start == ButtonState.Released)
                    {
                        gameState = GameState.Pause;
                    }

                    //Handle movements of the hoverfield
                    if ((gamePadState.DPad.Up == ButtonState.Pressed)
                        && (oldGamePadState.DPad.Up == ButtonState.Released)
                        && field.HoveredBlock.Y > 0)
                    {
                        field.HoveredBlock.Y -= 1;
                    }
                    if ((gamePadState.DPad.Down == ButtonState.Pressed)
                        && (oldGamePadState.DPad.Down == ButtonState.Released)
                        && field.HoveredBlock.Y < Globals.BLOCKS_VERTICAL - 1)
                    {
                        field.HoveredBlock.Y += 1;
                    }
                    if ((gamePadState.DPad.Left == ButtonState.Pressed)
                        && (oldGamePadState.DPad.Left == ButtonState.Released)
                        && field.HoveredBlock.X > 0)
                    {
                        field.HoveredBlock.X -= 1;
                    }
                    if ((gamePadState.DPad.Right == ButtonState.Pressed)
                        && (oldGamePadState.DPad.Right == ButtonState.Released)
                        && field.HoveredBlock.X < Globals.BLOCKS_HORIZONTAL - 1)
                    {
                        field.HoveredBlock.X += 1;
                    }

                    //Reveal the hovered field
                    if ((gamePadState.Buttons.A == ButtonState.Pressed)
                        && (oldGamePadState.Buttons.A == ButtonState.Released))
                    {
                        Block checkBlock = field.GetBlock(field.HoveredBlock);
                        if (!checkBlock.Flagged)
                        {
                            checkBlock.Uncovered = true;

                            //Player loses if he hits a bomb
                            if (checkBlock.BlockType == BLOCKTYPE.Mine)
                            {
                                field.ReveilAllBlocks();
                                CheckFlags();
                                gameState = GameState.Lost;
                            }

                            //If we hit a empty field, reveal all fields in all directions until we hit a number
                            if (checkBlock.BlockType == BLOCKTYPE.Empty)
                            {
                                field.ReveilSurroundingEmptyblocks(checkBlock);
                            }
                        }
                    }

                    //Flag/Unflag the hovered field
                    if ((gamePadState.Buttons.B == ButtonState.Pressed)
                       && (oldGamePadState.Buttons.B == ButtonState.Released))
                    {
                        Block checkBlock = field.GetBlock(field.HoveredBlock);
                        if (!checkBlock.Uncovered)
                        {
                            field.UnFlaggedMinesCount += (checkBlock.Flagged ? 1 : -1);
                            checkBlock.Flagged = !checkBlock.Flagged;
                        }
                    }


                    //Check for a Win
                    bool won = true;
                    for (int x = 0; x < Globals.BLOCKS_HORIZONTAL; x++)
                    {
                        for (int y = 0; y < Globals.BLOCKS_VERTICAL; y++)
                        {
                            Block checkBlock = field.GetBlock(new Point(x, y));

                            if (checkBlock.BlockType == BLOCKTYPE.Empty && !checkBlock.Uncovered)
                                won = false;
                        }
                    }

                    bool flaggedOnlyAllMines = true;
                    for (int x = 0; x < Globals.BLOCKS_HORIZONTAL; x++)
                    {
                        for (int y = 0; y < Globals.BLOCKS_VERTICAL; y++)
                        {
                            Block checkBlock = field.GetBlock(new Point(x, y));
                            if ((checkBlock.BlockType == BLOCKTYPE.Mine && !checkBlock.Flagged)
                                || (checkBlock.BlockType != BLOCKTYPE.Mine && checkBlock.Flagged))
                                flaggedOnlyAllMines = false;
                        }
                    }

                    if ((won || flaggedOnlyAllMines) && gameState != GameState.Lost)
                    {
                        field.ReveilAllBlocks();
                        gameState = GameState.Won;
                    }

                    field.time += gameTime.ElapsedGameTime;
                    break;
                case GameState.Lost:
                case GameState.Won:
                    if ((gamePadState.Buttons.A == ButtonState.Pressed)
                        && oldGamePadState.Buttons.A == ButtonState.Released)
                    {
                        Restart();
                    }
                    break;
                case GameState.Pause:
                    if ((gamePadState.Buttons.Start == ButtonState.Pressed)
                        && oldGamePadState.Buttons.Start == ButtonState.Released)
                    {
                        gameState = GameState.Playing;
                    }

                    if ((gamePadState.Buttons.X == ButtonState.Pressed)
                        && oldGamePadState.Buttons.X == ButtonState.Released)
                    {
                        Restart();
                        gameState = GameState.Playing;
                    }
                    break;
                default:
                    break;
            }
            

            //make references for oldGamepad- and oldKeyboardState
            oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (gameState != GameState.Menu)
            {
                field.Draw(spriteBatch);
                DrawHUD(spriteBatch);
            }

            switch (gameState)
            {
                case GameState.Menu:
                    
                    break;
                case GameState.Lost:
                    spriteBatch.Draw(textureFieldFill, new Rectangle(0, 0, Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT), new Color(0, 0, 0, 190));
                    string texta = "You lost :(";
                    spriteBatch.DrawString(tahoma, texta, new Vector2(Globals.ScreenCenter.X - tahoma.MeasureString(texta).X / 2, Globals.ScreenCenter.Y - 30), Color.Red);
                    break;
                case GameState.Won:
                    spriteBatch.Draw(textureFieldFill, new Rectangle(0, 0, Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT), new Color(0, 0, 0, 190));
                    string textb = "You win!";
                    spriteBatch.DrawString(tahoma, textb, new Vector2(Globals.ScreenCenter.X - tahoma.MeasureString(textb).X / 2, Globals.ScreenCenter.Y - 30), Color.Green);
                    break;
                case GameState.Pause:
                    spriteBatch.Draw(textureFieldFill, new Rectangle(0, 0, Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT), new Color(0, 0, 0, 190));
                    string textc = "Pause";
                    spriteBatch.DrawString(tahoma, textc, new Vector2(Globals.ScreenCenter.X - tahoma.MeasureString(textc).X / 2, Globals.ScreenCenter.Y - 30), Color.White);
                    break;
              
                default:
                    
                    break;
            }
       
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureHUD, new Rectangle(Globals.SCREEN_WIDTH -  textureHUD.Width, Globals.SCREEN_HEIGHT - textureHUD.Height, textureHUD.Width, textureHUD.Height), Color.White);

            spriteBatch.DrawString(tahoma, field.time.Seconds.ToString(), new Vector2(Globals.SCREEN_WIDTH / 4 - tahoma.MeasureString(field.time.Seconds.ToString()).X / 2 + 7, Globals.SCREEN_HEIGHT - textureHUD.Height / 2 - tahoma.MeasureString(field.time.Seconds.ToString()).Y / 2 + 3), Color.White);

            spriteBatch.DrawString(tahoma, field.UnFlaggedMinesCount.ToString(), new Vector2(Globals.SCREEN_WIDTH / 2 - tahoma.MeasureString(field.UnFlaggedMinesCount.ToString()).X / 2 + 7 + 45, Globals.SCREEN_HEIGHT - textureHUD.Height / 2 - tahoma.MeasureString(field.UnFlaggedMinesCount.ToString()).Y / 2 + 3), Color.White);
          
        }

        private void CheckFlags()
        {
            for (int x = 0; x < Globals.BLOCKS_HORIZONTAL; x++)
            {
                for (int y = 0; y < Globals.BLOCKS_VERTICAL; y++)
                {
                    Block checkBlock = field.GetBlock(new Point(x, y));

                    if (checkBlock.BlockType != BLOCKTYPE.Mine && checkBlock.Flagged)
                    {
                        checkBlock.Flagged = false;
                        checkBlock.Cross = true;
                    }
                }
            }
        }

        private void Restart()
        {
            field = new Field();
            gameState = GameState.Playing;
        }
    }
}
