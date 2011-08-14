using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GDI = System.Drawing;

namespace FireEngine.XNAContent.GDIFont
{
    public class FontRenderer
    {
        private const int BUFFER_WIDTH = 1024;
        private const int BUFFER_HEIGHT = 1024;
        //private const int DEFAULT_HEIGHT = 16;

        private Texture2D buffer;
        private Dictionary<FontChar, Rectangle> charMap;
        private Rectangle lastAddedRegion;
        private int currentLineBottom;
        private GDI.Bitmap gdiBmp;
        private GDI.Graphics gdiGraphics;
        private GraphicsDevice device;
        private GDI.StringFormat stringFormat;

#if DEBUG
        private int cacheHit = 0;
        private int cacheMiss = 0;
#endif
             
        public FontRenderer(GraphicsDevice device)
        {
            this.device = device;
            buffer = new Texture2D(device, BUFFER_WIDTH, BUFFER_HEIGHT);
            charMap = new Dictionary<FontChar, Rectangle>();
            
            lastAddedRegion = new Rectangle(0, 0, 0, 0);
            currentLineBottom = 0;

            gdiBmp = new GDI.Bitmap(BUFFER_WIDTH, BUFFER_HEIGHT);
            gdiGraphics = GDI.Graphics.FromImage(gdiBmp);
            gdiGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


            stringFormat = GDI.StringFormat.GenericTypographic.Clone() as GDI.StringFormat;
            stringFormat.FormatFlags |= GDI.StringFormatFlags.MeasureTrailingSpaces;
        }
        
        public void DrawString(SpriteBatch sprite, string text, Font font, Vector2 position, Color color)
        {
            string[] lines = text.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            Vector2 cp = new Vector2(position.X, position.Y);

            foreach (string str in lines)
            {
                foreach (char c in str)
                {
                    FontChar fc = new FontChar(font, c);

                    checkChar(fc);

                    Rectangle region;
                    region = charMap[fc];

                    sprite.Draw(buffer, cp, region, color);

                    cp.X += region.Width;
                }
                cp.X = position.X;
                cp.Y += font.gdiFont.Height;
            }
        }

        /// <summary>
        /// 向Buffer中存入指定的text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public void PrepareBuffer(string text, Font font)
        {
            foreach (char c in text)
            {
                FontChar fc = new FontChar(font,c);
                checkChar(fc);
            }
        }

        private void checkChar(FontChar fc)
        {
            if (charMap.ContainsKey(fc))     //Cache Hit
            {
#if DEBUG
                cacheHit++;
#endif
            }
            else                             //Cache Miss
            {
#if DEBUG
                cacheMiss++;
#endif
                generateNewChar(fc);
            }
        }

        private void generateNewChar(FontChar fc)
        {
            Font font = fc.Font;
            char c = fc.Char;

#if DEBUG
            Console.WriteLine("Char {0}: {1} in {2}", charMap.Count, c, font.Name);
#endif
            GDI.SizeF sizeF = gdiGraphics.MeasureString(c.ToString(), font.gdiFont, BUFFER_WIDTH, stringFormat);
            GDI.Size size = new GDI.Size((int)(sizeF.Width + 0.5), (int)(sizeF.Height + 0.5));

            gdiGraphics.Clear(GDI.Color.Transparent);
            gdiGraphics.DrawString(c.ToString(), font.gdiFont, GDI.Brushes.White, new GDI.PointF(0, 0), GDI.StringFormat.GenericTypographic);

            Rectangle region = allocateRegion(size);

            if (region.Width > 0 && region.Height > 0)
            {
                GDI.Imaging.BitmapData bmpData = gdiBmp.LockBits(
                    new GDI.Rectangle(0, 0, size.Width, size.Height),
                    GDI.Imaging.ImageLockMode.ReadOnly,
                    gdiBmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;

                byte[] data = new byte[4 * size.Width * size.Height];
                for (int i = 0; i < size.Height; i++)
                {
                    IntPtr start = (IntPtr)((Int64)ptr + i * bmpData.Stride);
                    System.Runtime.InteropServices.Marshal.Copy(start, data, i * 4 * size.Width, size.Width * 4);
                }

                gdiBmp.UnlockBits(bmpData);

                device.Textures[0] = null;  //TODO: ??
                buffer.SetData<byte>(0, region, data, 0, data.Length, SetDataOptions.None);
            }

            charMap.Add(fc, region);
            //return region;
        }

        private Rectangle allocateRegion(GDI.Size size)
        {
            Rectangle region;
            bool rightExceed = lastAddedRegion.Right + size.Width > BUFFER_WIDTH;
            bool bottomExceed = currentLineBottom + size.Height > BUFFER_HEIGHT;

            if(bottomExceed)
            {
                #if DEBUG
                    Console.WriteLine("Buffer is full with {0} characters.", charMap.Count);
                #endif   
                    Reset();
                return allocateRegion(size);    
            }

            if (rightExceed)
            {
                region = new Rectangle(0, currentLineBottom, size.Width, size.Height);
                currentLineBottom = region.Bottom;
            }
            else
            {
                region = new Rectangle(lastAddedRegion.Right, lastAddedRegion.Top, size.Width, size.Height);
                if (region.Bottom > currentLineBottom)
                    currentLineBottom = region.Bottom;
            }

            lastAddedRegion = region;

            return region;
        }

        private class FontChar
        {
            private Font font;
            private char c;
            private int hash;

            public FontChar(Font font, char c)
            {
                this.font = font;
                this.c = c;

                hash = font.GetHashCode() ^ (int)c;     //TODO: ??
            }

            public Font Font { get { return font; } }
            public char Char { get { return c; } }

            public override int GetHashCode()
            {
                return hash;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is FontChar))
                    return false;
                FontChar rhs = obj as FontChar;

                return font.Equals(rhs.font) && c == rhs.c;
            }
        }


        #region For Debug
#if DEBUG
        public void DrawBuffer(SpriteBatch sprite)
        {
            sprite.Draw(buffer, new Vector2(0, 0), Color.White);
        }

        public void Reset()
        {
            charMap.Clear();
            lastAddedRegion = new Rectangle(0, 0, 0, 0);
            currentLineBottom = 0;
        }

        public int BufferCount
        {
            get { return charMap.Count; }
        }

        public double MissRate
        {
            get { return (double)cacheMiss / (double)(cacheMiss+cacheHit); }
        }

        public void ResetCacheCounter()
        {
            cacheMiss = 0;
            cacheHit = 0;
        }
#endif
        #endregion
    }
}
