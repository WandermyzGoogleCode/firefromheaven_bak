using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Runtime
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Error error)
        {
            this.error = error;
        }

        private Error error;
        public Error Error
        {
            get { return error; }
        }

        public override string Message
        {
            get
            {
                return error.Message;
            }
        }

        //TODO: 想办法实现
        //public override string StackTrace
        //{
        //    get
        //    {
        //        return base.StackTrace;
        //    }
        //}
    }
}
