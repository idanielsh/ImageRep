using System;

namespace ImageRepServiceLibrary.DataAccess
{
    public class DBException : Exception
    {
        public DBException()
        {

        }

        public DBException(string message)
            : base(message)
        {
        }

        public DBException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}