using System.Drawing.Imaging;

namespace X4SectorCreator.Helpers
{
    internal static class ImageHelper
    {
        public static void SaveAsTga(Image image, string tgaPath)
        {
            using (var bitmap = new Bitmap(image))
            {
                using (var fs = new FileStream(tgaPath, FileMode.Create))
                using (var bw = new BinaryWriter(fs))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    // Write TGA Header (18 bytes)
                    bw.Write((byte)0);               // ID length
                    bw.Write((byte)0);               // Color map type
                    bw.Write((byte)2);               // Image type: uncompressed RGB
                    bw.Write((short)0);              // Color map origin
                    bw.Write((short)0);              // Color map length
                    bw.Write((byte)0);               // Color map depth
                    bw.Write((short)0);              // X-origin
                    bw.Write((short)0);              // Y-origin
                    bw.Write((short)width);          // Image width
                    bw.Write((short)height);         // Image height
                    bw.Write((byte)24);              // Bits per pixel
                    bw.Write((byte)0);               // Image descriptor

                    // Write pixel data (BGR format, bottom to top)
                    for (int y = height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color color = bitmap.GetPixel(x, y);
                            bw.Write(color.B);
                            bw.Write(color.G);
                            bw.Write(color.R);
                        }
                    }
                }
            }
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using MemoryStream ms = new();
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using MemoryStream ms = new(imageBytes);
            return Image.FromStream(ms);
        }
    }
}
