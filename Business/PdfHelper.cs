using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace CemSys2.Business
{
    public class PdfHelper
    {
        public static byte[] ImagenComoPdf(byte[] imagenBytes)
        {
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var imageData = ImageDataFactory.Create(imagenBytes);
            var img = new Image(imageData);
            img.SetAutoScale(true); // Ajusta sin distorsionar

            document.Add(img);
            document.Close();

            return ms.ToArray();
        }
    }
}
