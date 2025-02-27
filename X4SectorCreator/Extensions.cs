namespace X4SectorCreator
{
    internal static class Extensions
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input[1..];
        }
    }
}
