namespace Dreamrosia.Koin.UPbit.Infrastructure.Conveters
{
    public static class NumericConverter
    {
        public static string ToString(double volume)
        {
            var convert = $"{volume:F9}".TrimEnd('0');

            if (convert[^1] == '.')
            {
                return convert[..^1];
            }
            else
            {
                return convert;
            }
        }
    }
}
