// <copyright file="NpgsqlLoggingProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using Npgsql.Logging;

/// <summary>
/// A <see cref="INpgsqlLoggingProvider"/> for the logging abstraction.
/// </summary>
public class NpgsqlLoggingProvider : INpgsqlLoggingProvider
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlLoggingProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger provider.</param>
    private NpgsqlLoggingProvider(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Initializes the logging with the specified logger factory.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public static void Initialize(ILoggerFactory loggerFactory)
    {
        NpgsqlLogManager.Provider = new NpgsqlLoggingProvider(loggerFactory);
    }

    /// <summary>
    /// Creates a new INpgsqlLogger instance of the given name.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>The logger.</returns>
    public Npgsql.Logging.NpgsqlLogger CreateLogger(string name)
    {
        var logger = this._loggerFactory.CreateLogger(name);
        return new NpgsqlLogger(logger);
    }

    /// <summary>
    /// A NpgsqlLogger wrapper for an abstracted logger.
    /// </summary>
    /// <seealso cref="Npgsql.Logging.NpgsqlLogger" />
    private class NpgsqlLogger : Npgsql.Logging.NpgsqlLogger
    {
        /// <summary>
        /// The log4net logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlLogger"/> class.
        /// </summary>
        /// <param name="logger">The log4net logger.</param>
        public NpgsqlLogger(ILogger logger)
        {
            this._logger = logger;
        }

        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return level switch
            {
                NpgsqlLogLevel.Trace => this._logger.IsEnabled(LogLevel.Trace),
                NpgsqlLogLevel.Debug => this._logger.IsEnabled(LogLevel.Debug),
                NpgsqlLogLevel.Info => this._logger.IsEnabled(LogLevel.Information),
                NpgsqlLogLevel.Warn => this._logger.IsEnabled(LogLevel.Warning),
                NpgsqlLogLevel.Error => this._logger.IsEnabled(LogLevel.Error),
                NpgsqlLogLevel.Fatal => this._logger.IsEnabled(LogLevel.Critical),
                _ => false
            };
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception? exception = null)
        {
            switch (level)
            {
                case NpgsqlLogLevel.Trace:
                    this._logger.Log(LogLevel.Trace, exception, msg);
                    break;
                case NpgsqlLogLevel.Debug:
                    this._logger.Log(LogLevel.Debug, exception, msg);
                    break;
                case NpgsqlLogLevel.Info:
                    this._logger.Log(LogLevel.Information, exception, msg);
                    break;
                case NpgsqlLogLevel.Warn:
                    this._logger.Log(LogLevel.Warning, exception, msg);
                    break;
                case NpgsqlLogLevel.Error:
                    this._logger.Log(LogLevel.Error, exception, msg);
                    break;
                case NpgsqlLogLevel.Fatal:
                    this._logger.Log(LogLevel.Critical, exception, msg);
                    break;
                default:
                    // log nothing
                    break;
            }
        }
    }
}