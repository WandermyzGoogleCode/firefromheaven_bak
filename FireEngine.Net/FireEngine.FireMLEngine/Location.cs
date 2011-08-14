using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine
{
    /// <summary>
    /// 用来表示所有脚本文件中的某个位置（文件、行、列）
    /// </summary>
    [Serializable]
    public class Location
    {
        private string fileName;
        private int line;
        private int col;

        public Location(string fileName, int line, int col)
        {
            this.fileName = fileName;
            this.line = line;
            this.col = col;
        }

        public Location(string fileName)
            : this(fileName, -1, -1)
        {
        }

        /// <summary>
        /// 按指定的Offset返回一个新的Location
        /// </summary>
        /// <param name="colOffset"></param>
        /// <returns></returns>
        public Location Offset(int colOffset)
        {
            return new Location(fileName, line, col + colOffset);
        }

        public string FileName 
        { 
            get
            {
                return fileName;
            } 
        }

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return col; }
        }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1} in {2}", Line, Column, FileName);
        }
    }
}
