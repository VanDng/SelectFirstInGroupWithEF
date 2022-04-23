using System;
using System.IO;

namespace CommonConfig
{
    public static class ConnectionString
    {
        public static string Get()
        {
            return File.ReadAllText("connectionstring.txt");
        }
    }
}
