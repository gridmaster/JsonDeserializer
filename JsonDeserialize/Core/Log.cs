using System;
using System.Globalization;
using System.IO;

namespace JsonDeserialize.Core
{
    public static class Log
    {
        public static void WriteLog(string message)
        {
            using (StreamWriter log = File.AppendText("log.txt")) //directoryPath + "/log.txt"))
            {
                log.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message + "\r\n");
                log.Flush();
                log.Close();
            }
        }
    }
}
