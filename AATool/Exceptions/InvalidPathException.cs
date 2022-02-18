using System;
using System.IO;
using AATool.Configuration;
using AATool.Utilities;

namespace AATool.Exceptions
{
    [Serializable]
    public class InvalidPathException : ArgumentException
    {
        public InvalidPathException() : base($"Illegal character(s) in custom save path")
        {

        }
    }
}
