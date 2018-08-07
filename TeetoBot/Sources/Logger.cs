using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeetoBot.Sources {
    /// <summary>
    /// Provides basic logging to the console and specified discord users.
    /// </summary>
    public class Logger {

        /// <summary>
        /// The file to write the logs to.
        /// </summary>
        private static readonly String LOG_FILE = "Logs\\" + getTimeStamp().Replace(":", "_") + ".log";

        /// <summary>
        /// The stream writer to the log file.
        /// </summary>
        private static readonly StreamWriter FILE = new StreamWriter("..\\..\\" + LOG_FILE);
        
        /// <summary>
        /// Logs a message to the console, and any user specified
        /// to receive logging events.
        /// </summary>
        /// <param name="level">The priority of the message.</param>
        /// <param name="message">The message to log.</param>
        public void Log(Level level, Object message) {
            _Log(level, message.ToString());
        }

        /// <summary>
        /// Gets the integer value of the level. 0 is lowest or
        /// discord and 6 is highest or FATAL.
        /// </summary>
        /// <param name="level">The logging level.</param>
        /// <returns>The integer value of the logging level.</returns>
        private int LevelPriority(Level level) {
            switch (level) {
                case Level.DISCORD: return 0;
                case Level.TRACE: return 1;
                case Level.DEBUG: return 2;
                case Level.INFO: return 3;
                case Level.WARN: return 4;
                case Level.ERROR: return 5;
                case Level.FATAL: return 6;
            };

            return 0;
        }

        /// <summary>
        /// Internal log method.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void _Log(Level level, String message) {
            Console.WriteLine("[" + level.ToString().ToLower() + "] "
                              + "[" + getTimeStamp() + "] "
                              + "- " + message);

            FILE.WriteLine("[" + level.ToString().ToLower() + "] "
                              + "[" + getTimeStamp() + "] "
                              + "- " + message);
            FILE.Flush();
        }

        /// <summary>
        /// Returns a timestamp of the current time.
        /// </summary>
        /// <returns>The current time in the format: d MMM ddd yyyy HH:mm:ss.</returns>
        private static String getTimeStamp() {
            DateTime time = DateTime.Now;
            string format = "d MMM ddd yyyy HH:mm:ss";
            return time.ToString(format);
        }

        /// <summary>
        /// The level for the logged message.
        /// </summary>
        public enum Level {
            /// <summary>
            /// Indicates the log came from Discord.net.
            /// </summary>
            DISCORD,
            /// <summary>
            /// Trace level priority. Lowest priority.
            /// </summary>
            TRACE,
            /// <summary>
            /// Debug level priority. Second lowest priority.
            /// </summary>
            DEBUG,
            /// <summary>
            /// General logging level. 
            /// </summary>
            INFO,
            /// <summary>
            /// Warning level priority. Indicates a failure
            /// and alerts a user of the message.
            /// </summary>
            WARN,
            /// <summary>
            /// Error level priority. Indicates a failure
            /// and alerts a user of the message.
            /// </summary>
            ERROR,
            /// <summary>
            /// Fatal level priority. Indicates a failure
            /// and alerts a user of the message. This level
            /// indicates the application cannot continue and
            /// needs to be restarted.
            /// </summary>
            FATAL
        };
    }
}
