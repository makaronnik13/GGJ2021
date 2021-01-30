namespace com.armatur.common.util
{
    public static class TextUtil
    {
        public static string NonBreakable(string s)
        {
            return "<nobr>" + s + "</nobr>";
        }
    }
}