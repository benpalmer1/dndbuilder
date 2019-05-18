/*
 * DnD Character Builder Assignment.
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 18MAY19
 * 
 * Purpose:
 * Html utility class to hold functions related to html.
 */

using System.Web;

namespace DndBuilder.WebApi
{
    public static class HtmlUtil
    {
        public static string Encode(string htmlInput)
        {
            return HttpUtility.HtmlEncode(htmlInput);
        }

        // Decode(string) - Decodes a html input string of any encoding depth < 1000
        // Designed to handle possibility of any issues from multiple encoding / XSS attempts
        public static string Decode(string htmlInput)
        {
            if (htmlInput ==  HttpUtility.HtmlDecode(htmlInput))
            {
                return htmlInput;
            }
            else
            {
                int ii = 0;
		        string temp = htmlInput;
		        string decoded = HttpUtility.HtmlDecode(htmlInput);
                
                // Limited to prevent long running script on some error
		        while (temp != decoded && ii++ < 1000)
		        {
			        temp = decoded;
			        decoded = HttpUtility.HtmlDecode(decoded);
		        }

                return decoded;
            }
        }
    }
}
