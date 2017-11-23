// <copyright file="TimeSpanConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using Nancy.Json;

    /// <summary>
    /// A converter which converts timespans.
    /// </summary>
    public class TimeSpanConverter : DefaultJavaScriptConverter
    {
        /// <inheritdoc/>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(TimeSpan);
            }
        }

        /// <inheritdoc/>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary.TryGetValue("Ticks", out object timeSpanTicks) && timeSpanTicks is string timeSpanStr)
            {
                return new TimeSpan(int.Parse(timeSpanStr));
            }

            if (dictionary.TryGetValue("TotalMilliseconds", out object timeSpanMilliseconds) && timeSpanMilliseconds is string timeSpanMillisecondsStr)
            {
                return new TimeSpan(int.Parse(timeSpanMillisecondsStr) * TimeSpan.TicksPerMillisecond);
            }

            throw new ArgumentException("timespan property not found; only Ticks and TotalMilliseconds supported.");
        }
    }
}
