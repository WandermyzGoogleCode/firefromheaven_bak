using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using FireEngine.ContentInterface;
using FireEngine.Library;

namespace FireEngine.FireMLData.Asset
{
    [Serializable]
    public class ActorAsset : AssetDataBase
    {
        [XmlAttribute]
        public string Group;

        [XmlAttribute("src")]
        public string Source;

        public override bool CheckContent(IDataCheckHelper helper)
        {
            return helper.CheckContent(Source, ContentType.Texture);
        }
    }
}
