namespace Hig.Compiler
{
    using System;

    public static class StringExtensions
    {
        public static int CountWords(this string text, int stratIndex = 0, int endIndex = 0)
        {
            if (endIndex > text.Length)
                endIndex = text.Length;

            int count = 0;
            bool isCalc = false;

            for (int i = stratIndex; i < endIndex; i++)
            {
                if (text[i] == ' ' && isCalc)
                {
                    count++;
                    isCalc = false;
                }
                else if (text[i] != ' ')
                {
                    isCalc = true;
                }
            }

            return count;
        }

        public static string TrimWordLeft(this string text)
        {
            while (text[0] != ' ' && text != String.Empty)
                text = text.Substring(1, text.Length - 1);

            return text;
        }

        public static string TrimWordRight(this string text)
        {
            while (text[text.Length - 1] != ' ' && text != String.Empty)
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        public static string TrimWordsRound(this string text)
        {
            return TrimWordRight(TrimWordLeft(text));
        }
    }
}
