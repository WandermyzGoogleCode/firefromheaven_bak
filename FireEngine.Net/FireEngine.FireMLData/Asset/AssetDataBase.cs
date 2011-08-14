using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using FireEngine.FireMLData;

namespace FireEngine.FireMLData.Asset
{
    [Serializable]
    public abstract class AssetDataBase : FireMLDataBase
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public AssetAccess Access = AssetAccess.Hidden;
    }
}
