using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BearGame
{
    class Bear
    {
        #region Fields
        // texture 2D and rectangles
        private Texture2D sprite;
        private Rectangle bearRectangle;
        private Rectangle sourceRectangle;
        
        const int SPRITE_TILE_COLS = 10;
        const int SPRITE_TILE_ROWS = 2;
        //control animation speed
        const int WALK_MILLISECONDS = 200;
        private int elapsedMilliseconds = 0;

        private int WINDOW_WIDTH;
        private int WINDOW_HEIGHT;
        //speed of bear and jumping
        const int BEAR_MOVE_AMOUNT = 4;
        private bool jumping = false;
        private int jumpspeed = 0;
        //which way is bear facing
        BearFacing facing;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct bear
        /// </summary>
        /// <param name="contentManager">Content</param>
        /// <param name="spriteName">name of object for loading</param>
        /// <param name="windowWidth">window to keep bear in</param>
        /// <param name="windowHeight"></param>
        /// <param name="x">where is bear</param>
        /// <param name="y"></param>
        public Bear(ContentManager contentManager, string spriteName, int windowWidth, int windowHeight, int x, int y)
        {
            WINDOW_WIDTH = windowWidth;
            WINDOW_HEIGHT = windowHeight;
            
            bearRectangle.X = x;
            bearRectangle.Y = y;
            LoadContent(contentManager, spriteName, windowWidth, windowHeight, x, y);            
        }
        #endregion

        #region Properties
        //get bear collision rectangle
        public Rectangle CollisionRectangle
        {
            get { return bearRectangle; }
        }
        //bear x - don't allow to set off screen
        public int X
        {
            get { return bearRectangle.X + bearRectangle.Width / 2; }
            set
            {
                bearRectangle.X = value - bearRectangle.Width / 2;

                // clamp to keep in range
                if (bearRectangle.Left < 0)
                {
                    bearRectangle.X = 0;
                }
                else if (bearRectangle.Right > WINDOW_WIDTH)
                {
                    bearRectangle.X = WINDOW_WIDTH - bearRectangle.Width;
                }
            }
        }
        //set source rectangle
        public int SourceRectangle
        {
            set 
            {
                sourceRectangle.X = value;
            }
        }

        public int Y
        {
            get { return bearRectangle.Y + bearRectangle.Height / 2; }
            set
            {
                bearRectangle.Y = value - bearRectangle.Height / 2;

                // clamp to keep in range
                if (bearRectangle.Bottom <= WINDOW_HEIGHT - bearRectangle.Height -20)
                {
                    jumping = false;
                    bearRectangle.Y = WINDOW_HEIGHT - bearRectangle.Height;

                }
                else if (bearRectangle.Bottom > WINDOW_HEIGHT)
                {
                    bearRectangle.Y = WINDOW_HEIGHT - bearRectangle.Height;
                }
            }
        }
        //get bear height
        public int getBearHeight
        {
            get
            {
                return bearRectangle.Height;
            }
        }
        //get bear direction
        public BearFacing Facing
        {
            get
            {
                return facing;
            }
        }            

        #endregion

        #region Private Methods

        private void LoadContent(ContentManager contentManager, string spriteName,  int windowWidth, int windowHeight,
            int x, int y)
        {
            // load content and set draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);

            bearRectangle = new Rectangle(0, WINDOW_HEIGHT-96, (sprite.Width / SPRITE_TILE_COLS) * 3,
                ((sprite.Height / SPRITE_TILE_ROWS) * 3));//bear scale 3X

            sourceRectangle = new Rectangle(0, 0, sprite.Width / SPRITE_TILE_COLS, (sprite.Height / SPRITE_TILE_ROWS));
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch)
        {          
                spriteBatch.Draw(sprite, bearRectangle, sourceRectangle, Color.White);            
        }

        public void Update(KeyboardState keyboard, GameTime gameTime, bool sword)
        {
            //set bear according to whether sword needed to spin
            if (sword)
            {
                sourceRectangle.Y = 0;
            }
            else
            {
                sourceRectangle.Y = 32;
            }
            //jump logic
            if (jumping)
            {
                Y += jumpspeed;
                
                    if (sourceRectangle.X < 100)
                    {
                        X += 3;
                    }
                    else
                    {
                        X -= 3;
                    }
                //falling bear
                if (bearRectangle.Bottom != WINDOW_HEIGHT)
                {
                    jumpspeed += 1;
                }
                else
                {                    
                    jumping = false;
                }          
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    jumpspeed = -13;  //Give it upward thrust
                    jumping = true;
                }               
            }
            // dorection of bear and correct source rectangle
            if (keyboard.IsKeyDown(Keys.Right))
            {                
                X += BEAR_MOVE_AMOUNT;
                facing = BearFacing.Right;                
                elapsedMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                //control animation rectangles in walk
                if (elapsedMilliseconds % 200 > 100)
                {
                    sourceRectangle.X = 64;                    
                }

                if (elapsedMilliseconds % 200 < 100)
                {
                    sourceRectangle.X = 32;                    
                }                
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                X -= BEAR_MOVE_AMOUNT;
                facing = BearFacing.Left;
                elapsedMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                
                if (elapsedMilliseconds % 200 > 100)
                {
                    sourceRectangle.X = 256;
                }

                if (elapsedMilliseconds % 200 < 100)
                {
                    sourceRectangle.X = 224;
                }                
            }           
            //set source rectagle on released keys
            if (keyboard.IsKeyUp(Keys.Right) && keyboard.IsKeyUp(Keys.Left))
            {
                if (sourceRectangle.X > 100)
                {
                    sourceRectangle.X = 288;
                }
                else
                {
                    sourceRectangle.X = 0;
                }
            }
        }
        #endregion      
    }
}
