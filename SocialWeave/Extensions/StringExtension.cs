using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SocialWeave.Extensions
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    static class StringExtension
    {
        /// <summary>
        /// Cuts the string to a maximum length of 10 characters and appends "..." if the original string exceeds this length.
        /// </summary>
        /// <param name="thisString">The input string.</param>
        /// <returns>The modified string.</returns>
        public static string CutName(this string thisString)
        {
            if (thisString.Length <= 10)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 10) + "...";
            }
        }

        /// <summary>
        /// Cuts the string to a maximum length of 300 characters and appends "..." if the original string exceeds this length.
        /// </summary>
        /// <param name="thisString">The input string.</param>
        /// <returns>The modified string.</returns>
        public static string CutDescription(this string thisString)
        {
            if(thisString == null) 
            {
                return " ";
            }
            else if (thisString.Length <= 500)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 500) + "...";
            }
        }

        public static string CutComment(this string thisString)
        {
            if (thisString.Length <= 100)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 100) + "...";
            }
        }

    }
}
