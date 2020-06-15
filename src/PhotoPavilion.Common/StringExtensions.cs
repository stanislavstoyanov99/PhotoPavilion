namespace PhotoPavilion.Common
{
    public static class StringExtensions
    {
        public static string RemoveWhiteSpaces(this string text)
        {
            var texWithoutWhitespaces = string.Empty;

            foreach (var ch in text)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                texWithoutWhitespaces += ch;
            }

            return texWithoutWhitespaces;
        }
    }
}
