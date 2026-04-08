using AiWorkbench.Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using UglyToad.PdfPig;

namespace AiWorkbench.Infrastructure.Services
{
    public class FileTextExtractor : IFileTextExtractor
    {
        public string ExtractText(string fileName, byte[] bytes)
        {
            var ext = Path.GetExtension(fileName).ToLower();

            return ext switch
            {
                ".pdf" => ExtractPdf(bytes),
                ".docx" => ExtractDocx(bytes),
                ".txt" => Encoding.UTF8.GetString(bytes),
                _ => ""
            };
        }

        private string ExtractPdf(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var doc = PdfDocument.Open(ms);

            var sb = new StringBuilder();
            foreach (var page in doc.GetPages())
                sb.AppendLine(page.Text);

            return sb.ToString();
        }

        private string ExtractDocx(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var doc = WordprocessingDocument.Open(ms, false);

            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null)
                return "";

            return body.InnerText ?? "";
        }

    }
}

