using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using FireEngine.ContentInterface;

namespace FireEngine.FireMLData.Asset
{
    [Serializable]
    public class VideoAsset : AssetDataBase
    {
        [XmlAttribute("src")]
        public string Source;

        public override bool CheckContent(IDataCheckHelper helper)
        {
            return helper.CheckContent(Source, ContentType.Texture);
        }
    }
}
