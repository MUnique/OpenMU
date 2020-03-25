namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Linq;
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

            foreach (var property in properties)
            {
                stringBuilder.Append($"{property.Name}:{property.GetValue(this)}");

                // TODO this could be better
                if (property != properties.Last())
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}