using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FireEngine.XNAContent.GDIFont;

namespace FireEngine.Test.GDIFontTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private const float wordInterval = 0.01f;
        private const int fontNum = 2;
        private const int maxWords = 2000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private float fps = 0f, totalTime = 0f, displayFps = 0f;
        private float wordTime = 0f;
        Font[] fonts;
        FontRenderer renderer;
        string text;
        string subText = "";
        int subCounter;
        int pageHead = 0;
        int counter = 0;
        double missRate = 1.0;

        public Game1()
        {
            IsFixedTimeStep = false;

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

            System.IO.StreamReader reader = new System.IO.StreamReader("font1.txt");
            text = reader.ReadToEnd();

            fonts = new Font[]
            {
                new Font(18, FontStyle.Regular, "1_Disney.ttf"),
                //new Font("¿¬Ìå", 24, FontStyle.Regular),
                new Font("ËÎÌå", 32),
                new Font("Î¢ÈíÑÅºÚ", 18),
                new Font("»ªÎÄ²ÊÔÆ", 24),
                new Font("ºÚÌå", 18),
            };
            renderer = new FontRenderer(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                subCounter = text.Length;
            }

            float elapsedTime = (float)gameTime.ElapsedRealTime.TotalSeconds;
            totalTime += elapsedTime;
            wordTime += elapsedTime;
            if (totalTime >= 1)
            {
                displayFps = fps;
                fps = 0;
                totalTime = 0;
                counter++;
                missRate = renderer.MissRate;
                renderer.ResetCacheCounter();
            }

            if (wordTime >= wordInterval)
            {
                if (subCounter < text.Length)
                {
                    subCounter++;
                    if (subCounter - pageHead > maxWords)
                        pageHead += maxWords;
                }
                subText = text.Substring(pageHead, subCounter - pageHead);
                wordTime = 0;
            }

            for (int i = 0; i < fontNum; i++)
            {
                renderer.PrepareBuffer(subText, fonts[i]);
            }

            Window.Title = string.Format("{0} FPS, {1} sec, {2} words, miss rate {3:P}", displayFps, counter, renderer.BufferCount, missRate);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            fps += 1;

            spriteBatch.Begin();
            for (int i = 0; i < fontNum; i++)
            {
                renderer.DrawString(spriteBatch, subText, fonts[i], new Vector2(0, i * 160), Color.White);
            }
            //renderer.DrawBuffer(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
