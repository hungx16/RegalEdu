using QuestPDF.Infrastructure;
using SkiaSharp;

namespace RegalEdu.Infrastructure.Services
{
    public static class PdfImageHelper
    {
        /// <summary>
        /// Load ảnh và áp dụng độ mờ (opacity) trước khi đưa vào QuestPDF.
        /// opacity: 0.0f = trong suốt, 1.0f = đậm như gốc
        /// </summary>
        public static Image LoadTransparentImage(string filePath, float opacity)
        {
            if (opacity < 0f || opacity > 1f)
                throw new ArgumentOutOfRangeException (nameof (opacity), "Opacity must be between 0 and 1");

            using var originalImage = SKImage.FromEncodedData (filePath);
            var info = new SKImageInfo (
                originalImage.Width,
                originalImage.Height,
                SKColorType.Rgba8888,
                SKAlphaType.Premul);

            using var surface = SKSurface.Create (info);
            var canvas = surface.Canvas;
            canvas.Clear (SKColors.Transparent);

            using var paint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateBlendMode (
                    SKColors.White.WithAlpha ((byte)(opacity * 255)),
                    SKBlendMode.DstIn)
            };

            // vẽ ảnh với lớp “mờ”
            canvas.DrawImage (originalImage, 0, 0, paint);

            using var snapshot = surface.Snapshot ( );
            using var data = snapshot.Encode (SKEncodedImageFormat.Png, 100);
            var bytes = data.ToArray ( );

            return Image.FromBinaryData (bytes);
        }
    }
}
