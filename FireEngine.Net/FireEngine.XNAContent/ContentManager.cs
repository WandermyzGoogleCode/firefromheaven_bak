using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.ContentInterface;
using System.IO;

namespace FireEngine.XNAContent
{
    public class ContentManager : IContentManager
    {
        string contentPath;

        public ContentManager(string contentPath)
        {
            this.contentPath = contentPath;
        }


        #region IContentManager Members

        public AvailableCheckResult CheckAvailabe(string path, ContentType expectedType)
        {
            string absolutePath = contentPath + "\\" + path;

            if (!File.Exists(absolutePath))
            {
                return AvailableCheckResult.NotExist;
            }

            //TODO: Type Check

            return AvailableCheckResult.Available;
        }

        #endregion
    }
}
