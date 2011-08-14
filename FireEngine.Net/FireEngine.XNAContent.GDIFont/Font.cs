using System;
using System.Collections.Generic;
using GDI = System.Drawing;

namespace FireEngine.XNAContent.GDIFont
{
    public class Font
    {
        //public static byte MaxFontSize = 72;
        //public static byte DefaultFontSize = 16;
        internal readonly GDI.Font gdiFont;
        int hash;

        private static GDI.Text.PrivateFontCollection privateFontCollection = new System.Drawing.Text.PrivateFontCollection();

        public Font(string name, int size, FontStyle fontStyle)
        {
            gdiFont = new GDI.Font(name, size, (GDI.FontStyle)fontStyle, GDI.GraphicsUnit.Pixel);

            refreshHash();
        }

        public Font(int size, FontStyle fontStyle, string path)
        {
            privateFontCollection.AddFontFile(path);
            gdiFont = new GDI.Font(privateFontCollection.Families[privateFontCollection.Families.Length - 1], size, (GDI.FontStyle)fontStyle, GDI.GraphicsUnit.Pixel);
            
            refreshHash();
        }

        public Font(string name, int size) : this(name, size, FontStyle.Regular) { }

        private void refreshHash()
        {
            hash = (gdiFont.Name + gdiFont.Size.ToString() + gdiFont.Style.ToString()).GetHashCode();
        }
        
        public string Name
        {
            get { return gdiFont.Name; }
        }

        public int Size
        {
            get { return (int)gdiFont.Size; }
        }

        public bool IsBold
        {
            get { return gdiFont.Bold; }
        }

        public bool IsItalic
        {
            get { return gdiFont.Italic; }
        }

        public bool IsUnderline
        {
            get { return gdiFont.Underline; }
        }

        public bool IsStrikeout
        {
            get { return gdiFont.Strikeout; }
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Font))
                return false;

            Font rhs = (Font)obj;

            return gdiFont.Name == rhs.gdiFont.Name && gdiFont.Size == rhs.gdiFont.Size && gdiFont.Style == rhs.gdiFont.Style;
        }
    }
}
