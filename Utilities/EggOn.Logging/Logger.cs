using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowOptions.EggOn.Logging
{
    public static class Logger
    {
       // public static NLog.Logger _logger = NLog.LogManager.GetLogger("EggOn");

        public static void Log(LogLevel level, params object[] message)
        {
      //      _logger.Log(NLog.LogLevel.FromOrdinal((int)level), String.Join(" ", message));
        }

        public static void Trace(params object[] message)
        {
      //      _logger.Trace(String.Join(" ", message));
        }

        public static void Debug(params object[] message)
        {
      //      _logger.Debug(String.Join(" ", message));
        }

        public static void Info(params object[] message)
        {
       //     _logger.Info(String.Join(" ", message));
        }

        public static void Warn(params object[] message)
        {
        //    _logger.Warn(String.Join(" ", message));
        }

        public static void Error(params object[] message)
        {
        //    _logger.Error(String.Join(" ", message));
        }

        public static void Fatal(params object[] message)
        {
        //    _logger.Fatal(String.Join(" ", message));
        }
    }

    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
        Off = 6
    }
}
