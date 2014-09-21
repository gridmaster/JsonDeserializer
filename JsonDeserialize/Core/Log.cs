using System;
using System.Globalization;
using System.IO;

namespace JsonDeserialize.Core
{
    public static class Log
    {
        public static void WriteLog(LogEvent message)
        {
            using (StreamWriter log = File.AppendText("log.txt")) //directoryPath + "/log.txt"))
            {
                string msg = string.Format("Location: {0}, Description: {1}, Timestamp: {2}{3}", message.Location,
                                           message.Description, message.Timestamp, Environment.NewLine);
                log.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + msg);
                log.Flush();
                log.Close();
            }
        }
    }
}
