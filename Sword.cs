using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BearGame
{
    class Sword
    {
        #region Fields

        private Texture2D swordSprite;
        private Rectangle swordRectangle;
        private Rectangle sourceRectangle;

        const int SWORD_TILES = 16;
        const int FRAME_TIME = 10;
        private int elapsedFrameTime = 0;
        private int currentFrame = 0;

        private bool swordActive = false;

        int[] swordAniRight = {0, 7, 6, 5 };
        int[] swordAniLeft = {0, 1, 2, 3 };        

        #endregion

        #region Constructors

        public Sword(ContentManager contentManager, string spriteName, int x, int y)
        {
            swordRectangle.X = x;
            swordRectangle.Y = y;
            LoadContent(contentManager, spriteName, x, y);            
        }

        #endregion

        #region Properties

        public Rectangle CollisionRectangle
        {
            get { return swordRectangle; }
        }

        public bool SwordActive
        {
            get 
            {
                return swordActive;
            }
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            if (swordActive)
            {
                spriteBatch.Draw(swordSprite, swordRectangle, sourceRectangle, Color.White);
            }
        }

        public void Update(KeyboardState keyboard, GameTime gameTime, int bearX, int bearY, BearFacing f)
        {

            if (keyboard.IsKeyDown(Keys.Space))
            {
                swordActive = true;
                
            }
            
            if (keyboard.IsKeyUp(Keys.Space))
            {
                swordActive = false;
            }           

            if (swordActive == true)
            {                
                elapsedFrameTime += gameTime.ElapsedGameTime.Milliseconds;                

                if (f == BearFacing.Right)
                
                {
                    swordRectangle.X = bearX + 8;
                    swordRectangle.Y = bearY - 8;
                    
                    if (elapsedFrameTime > FRAME_TIME)
                    {
                        elapsedFrameTime = 0;
                        
                        if (currentFrame < swordAniRight.Length)
                        {
                            sourceRectangle.X = swordAniRight[currentFrame] * SWORD_TILES;      
                            currentFrame++;
                        }
                        else
                        {
                            currentFrame = 0;                            
                        }                        
                    }
                }

                if (f == BearFacing.Left)
                {
                    swordRectangle.X = bearX - 60;
                    swordRectangle.Y = bearY - 8;
                    if (elapsedFrameTime > FRAME_TIME)
                    {
                        elapsedFrameTime = 0;
                        
                        if (currentFrame < swordAniLeft.Length)
                        {
                            sourceRectangle.X = swordAniLeft[currentFrame] * SWORD_TILES;     
                            currentFrame++;
                        }
                        else
                        {
                            currentFrame = 0;                            
                        }                        
                    }
                }
            }         
        }

        #endregion

        #region Private Methods

        private void LoadContent(ContentManager contentManager, string spriteName, int x, int y)
        {
            swordSprite = contentManager.Load<Texture2D>(spriteName);
            swordRectangle = new Rectangle(x, y, SWORD_TILES*4, SWORD_TILES*4);
            sourceRectangle = new Rectangle(0, 0, SWORD_TILES, SWORD_TILES);       
        }
        #endregion
    }
}
