namespace SocialWeave.Extensions
{
    static public class IEnumerableExtension
    {

        public static string CountLikes<T>(this IEnumerable<T> thisIEnumarable) 
        {
            int amount = 0;

            if(thisIEnumarable == null) 
            {
                return "0";
            }
            else if (thisIEnumarable.Count() <= 999) 
            {
                return thisIEnumarable.Count().ToString();
            }
            else if(thisIEnumarable.Count() <= 9999) 
            {
                amount = thisIEnumarable.Count();
                string result = amount.ToString().Substring(0, 1) + " K";
                return result;
            }
            else if (thisIEnumarable.Count() <= 99999) 
            {
                amount = thisIEnumarable.Count();
                string result = amount.ToString().Substring(0, 2) + " K";
                return result;
            }
            else if (thisIEnumarable.Count() <= 999999)
            {
                amount = thisIEnumarable.Count();
                string result = amount.ToString().Substring(0, 3) + " K";
                return result;
            }
            else
            {
                amount = thisIEnumarable.Count();
                string result = amount.ToString().Substring(0, 4) + " M";
                return result;
            }
        }
    }
}
