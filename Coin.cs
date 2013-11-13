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
    public class Coin
    {

        #region Fields

        private Texture2D coinSprite;
        private Rectangle coinRectangle;
        private Rectangle sourceRectangle;

        bool active = true;

        const int COIN_TILES = 16; 

        #endregion

        #region Constructors

        public Coin(ContentManager contentManager, string spriteName, int x, int y)
        {
            coinRectangle.X = x;
            coinRectangle.Y = y;
            LoadContent(contentManager, spriteName, x, y);
        }

        #endregion

        #region Properties

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public virtual bool isBad
        {
            get { return false; }
        }  

        public virtual Rectangle CollisionRectangle
        {
            get { return coinRectangle; }
        }

        #endregion

        #region Public Methods

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(coinSprite, coinRectangle, sourceRectangle, Color.White);
            }
        }

        public virtual void Update()
        {
            if (active)
            {
                
                coinRectangle.X -= 1;
            }
        }

        #endregion


        #region Private Methods

        protected virtual void LoadContent(ContentManager contentManager, string spriteName, int x, int y)        {
            coinSprite = contentManager.Load<Texture2D>(spriteName);
            coinRectangle = new Rectangle(x, y, COIN_TILES * 2, COIN_TILES * 2);
            sourceRectangle = new Rectangle(0, 0, COIN_TILES, COIN_TILES);
        }

        #endregion
    }
}
