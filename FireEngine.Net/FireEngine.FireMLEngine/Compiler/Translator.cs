using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Compiler
{
    class Translator
    {
        private string language;

        public Translator(string language)
        {
            this.language = language;
        }

        public string this[string key]
        {
            get
            {
                return key;
            }
        }
    }
}
