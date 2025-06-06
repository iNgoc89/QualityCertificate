using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QRCoder;
using QualityCertificate.Datas.Entity;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;

namespace QualityCertificate.Services
{
    public class WordOpenXmlService
    {
        private readonly QRService _qrService;
        private readonly IConfiguration _configuration;
        private string _paperSize;
        private int _width = 0;
        private int _height = 0;
        private float _startX = 0;
        private float _startY = 0;
        private string _qrUrl;
        private int _qrWidth = 0;
        private int _qrHeight = 0;


        public WordOpenXmlService(QRService qrService, IConfiguration configuration) 
        {
            _qrService = qrService;
            _configuration = configuration;
            _paperSize = _configuration.GetValue<string>("PrintSetting:PaperSize") ?? "A4";
            _width = _configuration.GetValue<int>("PrintSetting:Width");
            _height = _configuration.GetValue<int>("PrintSetting:Height");
            _startX = _configuration.GetValue<float>("PrintSetting:StartX");
            _startY = _configuration.GetValue<float>("PrintSetting:StartY");
            _qrUrl = _configuration.GetValue<string>("PrintSetting:QrUrl");
            _qrWidth = _configuration.GetValue<int>("PrintSetting:QrWidth");
            _qrHeight = _configuration.GetValue<int>("PrintSetting:QrHeight");
      
        }
        public (string ErrorMsg, bool IsSucress) PrintInfoWithQR(BulkCementCQ input)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                // Cấu hình máy in (mặc định)
                //"A4", 827, 1169
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169); // A4: 8.27in x 11.69in in 1/100 inch

                printDoc.PrintPage += async (sender, e) =>
                {
                    // Lấy Graphics từ máy in
                    Graphics g = e.Graphics;

                    // Tọa độ bắt đầu (pixel)
                    //x=20, y=900
                    float startX = _startX;
                    float startY = _startY;
                    System.Drawing.Font font = new System.Drawing.Font("Times New Roman", 10, FontStyle.Bold);
                    Brush brush = Brushes.Black;

                    // In văn bản
                    g.DrawString($"Xe: {input.Vehicle_Code} - {input.Second_Vehicle_Code}", font, brush, startX, startY);
                    g.DrawString($"Khối lượng: {input.DeliveryQty_Kg.ToString("N0", CultureInfo.InvariantCulture)} Kg", font, brush, startX, startY + 20);
                    g.DrawString($"Ngày xuất: {input.Quality_Date.Value.ToString("dd/MM/yyyy")}", font, brush, startX, startY + 40);

                    // Tạo mã QR
                    var url = _qrUrl + input.Gid.ToString();
                    var img = _qrService.GenerateQrCode(url);


                    using (var ms = new MemoryStream(img))
                    {
                        using (var image = Image.FromStream(ms))
                        {
                            Rectangle qrRect = new Rectangle((int)(startX + 247), (int)(startY - 35), _qrWidth, _qrHeight);
                            g.DrawImage(image, qrRect);
                        }
                    }


                };

                printDoc.Print();

                return ("In thành công", true);
            }
            catch (Exception ex)
            {

                return ("Lỗi: " + ex.Message, false);
            }
      
        }
     
    }
}
