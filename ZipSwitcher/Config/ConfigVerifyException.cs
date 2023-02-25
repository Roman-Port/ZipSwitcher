using System;
using System.Collections.Generic;
using System.Text;

namespace ZipSwitcher.Config
{
    class ConfigVerifyException : Exception
    {
        public ConfigVerifyException(string message) : base(message) { }
    }
}
