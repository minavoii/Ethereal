using System.Collections.Generic;
using System.Linq;

namespace Generators.Helpers;

internal static class CommentHelper
{
    /// <summary>
    /// Turns a list of comments into a single, formatted one. <br/>
    /// This is required because the generated output differs in debug and release modes.
    /// </summary>
    /// <param name="comments"></param>
    /// <returns></returns>
    internal static string ToString(List<string> comments)
    {
        // Release mode
        if (comments.Count == 1)
        {
            string formatted = string.Join("", comments.Select(x => x.Replace("///", "    ///")));
            formatted = formatted.Substring(0, formatted.LastIndexOf("\n") - 1);

            return $"        ///{formatted}";
        }
        // Debug mode
        else
            return string.Join("\n", comments.Select(x => $"        {x}"));
    }
}
