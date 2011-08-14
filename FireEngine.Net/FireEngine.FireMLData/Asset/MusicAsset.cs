using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using FireEngine.ContentInterface;

namespace FireEngine.FireMLData.Asset
{
    [Serializable]
    public class MusicAsset : AssetDataBase
    {
        internal MusicAsset() : base() { }

        [XmlAttribute("src")]
        public string Source;

        [XmlAttribute]
        public string Group;

        public string Title;

        public string Artist;

        public string LyricsBy;

        public string MusicBy;

        public string Description;

        public string Lyrics;

        public string LRC;

        public override bool CheckContent(IDataCheckHelper helper)
        {
            return helper.CheckContent(Source, ContentType.Music);
        }
    }
}
