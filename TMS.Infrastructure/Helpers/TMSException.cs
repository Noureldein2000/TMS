using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Infrastructure.Helpers
{
    public class TMSException : Exception
    {
        public string ErrorCode { get; private set; }
        public object ExceptionDate { get; set; }
        public TMSException() : base()
        {

        }

        public TMSException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
