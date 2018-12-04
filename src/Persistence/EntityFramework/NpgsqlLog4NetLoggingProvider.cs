// <copyright file="NpgsqlLog4NetLoggingProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using log4net;
    using Npgsql.Logging;

    /// <summary>
    /// A <see cref="INpgsqlLoggingProvider"/> for log4net.
    /// </summary>
    /// <seealso cref="Npgsql.Logging.INpgsqlLoggingProvider" />
    public class NpgsqlLog4NetLoggingProvider : INpgsqlLoggingProvider
    {
        /// <summary>
        /// Creates a new INpgsqlLogger instance of the given name.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>The logger.</returns>
        public NpgsqlLogger CreateLogger(string name)
        {
            var logger = LogManager.GetLogger(typeof(NpgsqlLog4NetLoggingProvider).Assembly, name);
            return new NpgsqlLog4NetLogger(logger);
        }

        /// <summary>
        /// A NpgsqlLogger wrapper for a log4net logger.
        /// </summary>
        /// <seealso cref="Npgsql.Logging.NpgsqlLogger" />
        private class NpgsqlLog4NetLogger : NpgsqlLogger
        {
            /// <summary>
            /// The log4net logger.
            /// </summary>
            private readonly ILog logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="NpgsqlLog4NetLogger"/> class.
            /// </summary>
            /// <param name="logger">The log4net logger.</param>
            public NpgsqlLog4NetLogger(ILog logger)
            {
                this.logger = logger;
            }

            public override bool IsEnabled(NpgsqlLogLevel level)
            {
                switch (level)
                {
                    case NpgsqlLogLevel.Debug:
                        return this.logger.IsDebugEnabled;
                    case NpgsqlLogLevel.Info:
                        return this.logger.IsInfoEnabled;
                    case NpgsqlLogLevel.Warn:
                        return this.logger.IsWarnEnabled;
                    case NpgsqlLogLevel.Error:
                        return this.logger.IsErrorEnabled;
                    case NpgsqlLogLevel.Fatal:
                        return this.logger.IsFatalEnabled;
                    default:
                        return false;
                }
            }

            public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
            {
                switch (level)
                {
                    case NpgsqlLogLevel.Debug:
                        this.logger.Debug(msg, exception);
                        break;
                    case NpgsqlLogLevel.Info:
                        this.logger.Info(msg, exception);
                        break;
                    case NpgsqlLogLevel.Warn:
                        this.logger.Warn(msg, exception);
                        break;
                    case NpgsqlLogLevel.Error:
                        this.logger.Error(msg, exception);
                        break;
                    case NpgsqlLogLevel.Fatal:
                        this.logger.Fatal(msg, exception);
                        break;
                    default:
                        // log nothing
                        break;
                }
            }
        }
    }
}