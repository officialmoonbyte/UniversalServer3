using System;
using System.IO;
using static Crayon.Output;

namespace Moonbyte.UniversalServer.Core.Logging
{
    public class ILogger
    {
        public enum Levels { TRACE, DEBUG, INFO, WARN, ERROR, FATAL }
        //Generating a new string for the log file.
        public static string Log;

        public ILogger()
        {
            //Get the execution directory
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(Path.Combine(exeDirectory, "Log.log")))
            {
                Log = File.ReadAllText(Path.Combine(exeDirectory, "Log.log"));
            }
        }

        /// <summary>
        /// Used to add a value to the log string.
        /// </summary>
        public static void AddToLog(Levels level, string value)
        {
            string header = "info";

            switch (level)
            {
                case Levels.TRACE:
                    header = Yellow(Bold("TRACE"));
                    break;
                case Levels.DEBUG:
                    header = Bold("DEBUG");
                    break;
                case Levels.INFO:
                    header = Cyan(Bold("INFO"));
                    break;
                case Levels.WARN:
                    header = Yellow(Bold("WARN"));
                    break;
                case Levels.ERROR:
                    header = Red(Bold("ERROR"));
                    break;
                case Levels.FATAL:
                    header = Red(Bold("FATAL"));
                    break;
            }

            string newValue = $"[{DateTime.Now.ToString("yyyy-MM-dd | HH:mm:ssZ")}] [{header}] {value}";

            //Check if Log is null, if it is not then makes a new line.
            if (Log != null) Log = Log + "\r\n" + newValue;

            //Cehck if log is null, if it is then set log to value
            if (Log == null) Log = newValue;

            //Prints value in console
            Console.WriteLine(newValue);
        }

        /// <summary>
        /// Adds a white space to the command list
        /// </summary>
        public static void AddWhitespace()
        {
            //Add the white space
            if (Log != null) Log += "\r\n";

            //Prints in console
            Console.WriteLine(" ");
        }

        /// <summary>
        /// Set the logging events for the server
        /// </summary>
        public static void SetLoggingEvents()
        {
            AppDomain.CurrentDomain.UnhandledException += ((obj, args) =>
            {
                UnhandledExceptionEventArgs e = args;

                ILogger.AddToLog(Levels.FATAL, "Error with App Domain");

                Exception ex = (Exception)e.ExceptionObject;

                ILogger.AddToLog(Levels.FATAL, "Message : " + ex.Message);
                ILogger.AddToLog(Levels.TRACE, "StackTrace : " + ex.StackTrace);
                ILogger.AddToLog(Levels.TRACE, "Source : " + ex.Source);

                ILogger.WriteLog();
            });
            AppDomain.CurrentDomain.ProcessExit += ((obj, args) =>
            {
                ILogger.WriteLog();
            });
        }

        /// <summary>
        /// Used to write to the log file
        /// </summary>
        public static void WriteLog()
        {

            //Get the execution directory
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //Check if the Log is null
            if (Log != null)
            {
                //Delete the log file if it exist.
                if (File.Exists(exeDirectory + "\\Log.log")) File.Delete(exeDirectory + "\\Log.log");

                //Creates the log file, and then close the file stream.
                File.Create(exeDirectory + "\\Log.log").Close();

                //Write to the log file.
                File.WriteAllText(exeDirectory + "\\Log.log", Log);
            }
        }

        public static void LogExceptions(Exception e)
        {
            ILogger.AddToLog(Levels.INFO, e.Message);
            ILogger.AddToLog(Levels.INFO, e.StackTrace);
        }

    }
}
