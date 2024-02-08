using System.Runtime.CompilerServices;

namespace SocialWeave.Extensions
{
    static class StringExtension
    {

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

        public static string CutDescription(this string thisString) 
        {
            if (thisString.Length <= 300)
            {
                return thisString;
            }
            else
            {
                return thisString.Substring(0, 300) + "...";
            }
        }

    }
}
