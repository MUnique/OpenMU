// <copyright file="ArgumentsBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Text;

    /// <summary>
    /// The base of every arguments class used in the commands.
    /// </summary>
    public class ArgumentsBase
    {
        /// <summary>
        /// This makes it easier to print the arguments and its values for debugging.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            var properties = this.GetType().GetProperties();
            var stringBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (var property in properties)
            {
                if (!isFirst)
                {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append($"{property.Name}:{property.GetValue(this)}");
                isFirst = false;
            }

            return stringBuilder.ToString();
        }
    }
}