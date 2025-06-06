using System.ComponentModel.DataAnnotations;

namespace QualityCertificate.Datas.Models
{
    public class CardNumbers
    {
        public bool HasWeighing { get; set; }
        public long CodeWeighing { get; set; }
        public long CardNumber { get; set; }
        public string VehicleCode { get; set; } = string.Empty;
        public string SecondVehicleCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:ss}")]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:ss}")]
        public DateTime? CardInDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:ss}")]
        public DateTime? CardOutDate { get; set; }
    }
}
