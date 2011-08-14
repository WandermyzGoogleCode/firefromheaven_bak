using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FireEngine.FireMLData
{
    [Serializable]
    public class PositionData : FireMLDataBase
    {
        /// <summary>
        /// 横坐标
        /// </summary>
        [XmlAttribute]
        public int X;

        /// <summary>
        /// 纵坐标
        /// </summary>
        [XmlAttribute]
        public int Y;

        public static PositionData Parse(string str)
        {
            string[] nums = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            PositionData p = new PositionData();
            p.X = int.Parse(nums[0]);
            p.Y = int.Parse(nums[1]);
            return p;
        }
    }
}
