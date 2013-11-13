using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;    

namespace BearGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //game state
        StateOfGame gameState = StateOfGame.Menu;
        //countdown timer
        const int COUNTDOWN_START = 15;
        int countdown = COUNTDOWN_START;
        const int MOVE_ON_SCORE = 10;
        //window
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;
        //coin rollout timer
        const int COIN_MILLI = 3000;
        int elapsedMilli = 0;
        int elapsedMilliBad = 0;
        //objects
        Bear bear;
        Sword sword;   
        
        bool countdownActive = true;
        int countMilli = 0;
        int toMilli = 0;
        const int TO_MILLI = 3000;
        int score = 0;
        
        const string SCORE_STRING_PREFIX = "Score: ";

        List<Coin> coins = new List<Coin>();
        //2D textures and rectangles
        Texture2D background;
        Rectangle bground;

        Texture2D playButton;
        Rectangle playRect;
        Rectangle playSource;

        Texture2D quitButton;
        Rectangle quitRect;
        Rectangle quitSource;
        //mouse control
        bool clickStarted = false;
        bool buttonReleased = true;
        //text font 
        SpriteFont font;
        //random objecs
        Random randX = new Random();
        Random randY = new Random(); 
        // audio components
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            
            //loading objects and source rectangles
            background = Content.Load<Texture2D>("background");
            bground = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
            //cerate objects from game classes
            bear = new Bear(Content, "bearSword", WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_WIDTH / 2, (WINDOW_HEIGHT / 2));
            
            sword = new Sword(Content, "swordStrip", bear.X, bear.Y);
            //load font
            font = Content.Load<SpriteFont>("SpriteFont1");            

            playButton = Content.Load<Texture2D>("playbutton");
            playRect = new Rectangle(50, 50, playButton.Width / 2, playButton.Height);
            playSource = new Rectangle(0, 0, playButton.Width / 2, playButton.Height);

            quitButton = Content.Load<Texture2D>("quitButton");
            quitRect = new Rectangle(525, 50, quitButton.Width / 2, quitButton.Height);
            quitSource = new Rectangle(0, 0, quitButton.Width / 2, quitButton.Height);

            // load audio content
            audioEngine = new AudioEngine(@"Content\sounds.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Sound Bank.xsb");
            //start background music and loop
            soundBank.PlayCue("backgroundMusic");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //capture input states
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            //menu state
            if (gameState == StateOfGame.Menu)
            {
                //change source rec to make buttons highlight on mouseover
                if (playRect.Contains(mouse.X, mouse.Y))
                {
                    playSource.X = 200;
                }
                else
                {
                    playSource.X = 0;
                }

                if (quitRect.Contains(mouse.X, mouse.Y))
                {
                    quitSource.X = 200;
                }
                else
                {
                    quitSource.X = 0;
                }
                // if mouse is not pressed but pressed on button
                if (mouse.LeftButton == ButtonState.Pressed &&
                        buttonReleased)
                {
                    clickStarted = true;
                    if (playRect.Contains(mouse.X, mouse.Y) && clickStarted)
                    {
                        soundBank.PlayCue("buttonClick"); //play click
                        gameState = StateOfGame.Play; //changes state to play                        
                        clickStarted = false;
                    }
                    else if (clickStarted && quitRect.Contains(mouse.X, mouse.Y))
                    {
                        soundBank.PlayCue("buttonClick");
                        this.Exit();//exit game
                    }
                }
                else if (mouse.LeftButton == ButtonState.Released)
                {
                    buttonReleased = true;                    
                }
            }
            // if player wins
            if (gameState == StateOfGame.LevelChange)
            {
                toMilli += gameTime.ElapsedGameTime.Milliseconds;  //times winner screen
                coins.Clear();//clears list of coins
                bear.SourceRectangle = 128;//changes bear
                if (toMilli > TO_MILLI)
                {                    
                    Reset();//calls reset function after a set time
                }            
            }
            // start play
            if (gameState == StateOfGame.Play)
            {
                if (score >= MOVE_ON_SCORE)//if win
                {
                    gameState = StateOfGame.LevelChange;
                }

                else if (countdown < 0) // if lose
                {
                    countdownActive = false; //don't show countdown
                    toMilli += gameTime.ElapsedGameTime.Milliseconds; // time for lose screen
                    bear.SourceRectangle = 96;
                    coins.Clear();                        
                        
                    if (toMilli > TO_MILLI)
                    {                            
                        Reset();
                    }
                }
                else
                {
                    //countdown
                    countMilli += gameTime.ElapsedGameTime.Milliseconds;
                    if (countMilli > 1000)
                    {
                        countdown--;
                        countMilli = 0;
                    }
                    //set coin count add to list , draw random coins
                    elapsedMilli += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedMilli > COIN_MILLI)
                    {
                        int yinit = 400;
                        int ySep = 50;
                        for (int i = 0; i < 3; i++)
                        {
                            Coin newCoin = new Coin(Content, "coin",
                                    randX.Next(730, 777), yinit += ySep);

                            coins.Add(newCoin);
                        }
                        elapsedMilli = 0;
                    }
                    // for bad coins
                    elapsedMilliBad += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedMilliBad > 2 * COIN_MILLI)
                    {                            
                        for (int i = 0; i < 1; i++)
                        {
                            BadCoin newBadCoin = new BadCoin(Content, "badCoin",
                                    randX.Next(730, 777), randY.Next(400, 575));

                            coins.Add(newBadCoin);
                            Console.WriteLine(coins[i].ToString());
                        }
                        elapsedMilliBad = 0;
                    }
                    //update bear
                    bear.Update(keyboard, gameTime, sword.SwordActive);
                    //update sword
                    sword.Update(keyboard, gameTime, bear.X, bear.Y, bear.Facing);
                    //update coins in list, loop through
                    foreach (Coin c in coins)
                    {
                        c.Update();
                        // kill coins that are colliding 
                        if (c.Active && bear.CollisionRectangle.Intersects(c.CollisionRectangle))
                        {
                            c.Active = false;
                            if (c.isBad)//coin difference
                            {
                                soundBank.PlayCue("Bomb");
                                score -= 10;
                            }
                            else
                            {
                                soundBank.PlayCue("Hurt");
                                score -= 1;
                            }     
                        }
                        // handle sword on coins
                        if (c.Active && sword.SwordActive && sword.CollisionRectangle.Intersects(c.CollisionRectangle))
                        {
                            soundBank.PlayCue("Hit");
                            c.Active = false;
                            score += 1;  
                        }
                    }
                    // get rid of dead coins
                    for (int i = coins.Count - 1; i >= 0; i--)
                    {
                        if (!coins[i].Active)
                        {
                            coins.RemoveAt(i);
                        }
                    }
                    // make a sword sound
                    if (sword.SwordActive && sword.CollisionRectangle.Intersects(bear.CollisionRectangle))
                    {
                        soundBank.PlayCue("sword");
                    }
                }

                }  

                base.Update(gameTime);            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (gameState == StateOfGame.Play || gameState == StateOfGame.LevelChange)// play and winner/loser screens
            {
                spriteBatch.Draw(background, bground, Color.White);                
                sword.Draw(spriteBatch);//draw sword
                bear.Draw(spriteBatch);//draw bear
                foreach (Coin c in coins)
                {
                    c.Draw(spriteBatch);  // draw coins                  
                }
                // draw fonts on condition
                spriteBatch.DrawString(font, SCORE_STRING_PREFIX + score + "/" + MOVE_ON_SCORE, new Vector2(50, 100), 
                    Color.PowderBlue);
                if (countdownActive)
                {
                    spriteBatch.DrawString(font, "Countdown! : " + countdown.ToString(), new Vector2(50, 50), 
                        Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(font, "Time's UP!", new Vector2(WINDOW_WIDTH / 3, WINDOW_HEIGHT / 3), 
                        Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                }
                // winner screen
                if (gameState == StateOfGame.LevelChange)
                {
                    spriteBatch.DrawString( font , "Winner!", new Vector2(WINDOW_WIDTH/3, WINDOW_HEIGHT/3) , Color.White , 0,new Vector2(0, 0), 3, SpriteEffects.None ,0);
                }
            }
            //menu
            if (gameState == StateOfGame.Menu)
            {
                //strings for directions
                string dir1 = "Use arrows to move, up arrow to jump. Space bar swings power stick";
                string dir2 = "Hit  coins with stick, don't let 'em hit you, especially red ones!";
                string dir3 = "Get the right number before time's up and win a skateboard";
                //buttons
                spriteBatch.Draw(playButton, playRect, playSource, Color.White);
                spriteBatch.Draw(quitButton, quitRect, quitSource, Color.White);
                //fonts
                spriteBatch.DrawString(font, dir1 , new Vector2(10, 200), Color.White);
                spriteBatch.DrawString(font, dir2, new Vector2(10, 240), Color.White);
                spriteBatch.DrawString(font, dir3, new Vector2(10, 280), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        // resets game on win/loss
        private void Reset()
        {
            gameState = StateOfGame.Menu;
            countdown = COUNTDOWN_START;
            score = 0;
            countdownActive = true;
            toMilli = 0;
            bear.X = 0;        
        }    
    }
}
