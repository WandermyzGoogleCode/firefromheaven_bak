using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.ContentInterface
{
    public interface IContentManager
    {
        AvailableCheckResult CheckAvailabe(string path, ContentType expectedType);
    }
}
