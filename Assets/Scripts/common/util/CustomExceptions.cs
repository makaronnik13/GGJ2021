using System;

namespace com.armatur.common.util
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException()
        {
        }

        public NotInitializedException(string message)
            : base(message)
        {
        }

        public NotInitializedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


}