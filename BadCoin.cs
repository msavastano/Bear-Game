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
    // Comment one
    public class BadCoin : Coin
    {
        private Texture2D badSprite;
        private Rectangle badRectangle;
        private Rectangle sourceRectangle;

        const int COIN_TILES = 16;  

        public BadCoin(ContentManager contentManager, string spriteName, int x, int y) : 
            base(contentManager, spriteName, x, y)
        {
            badRectangle.X = x;
            badRectangle.Y = y;
            LoadContent(contentManager, spriteName, x, y);
        }

        public override Rectangle CollisionRectangle
        {
            get { return badRectangle; }
        }

        public override bool isBad
        {
            get { return true; }
        }

        protected override void LoadContent(ContentManager contentManager, string spriteName, int x, int y)
        {
            badSprite = contentManager.Load<Texture2D>(spriteName);
            badRectangle = new Rectangle(x, y, COIN_TILES * 2, COIN_TILES * 2);
            sourceRectangle = new Rectangle(0, 0, COIN_TILES, COIN_TILES);
        }

        /**
        * Comment two
         */
        public override void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(badSprite, badRectangle, sourceRectangle, Color.White);            
        }

        public override void Update()
        {
            if (base.Active)
            {                
                badRectangle.X -= 1;
            }
        }
    }
}
