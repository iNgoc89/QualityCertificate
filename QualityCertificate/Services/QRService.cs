using QRCoder;

namespace QualityCertificate.Services
{
    public class QRService
    {
        public QRService() { }
        public byte[] GenerateQrCode(string content)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                // return "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);
                return qrCodeImage;
            }
        }
    }
}
