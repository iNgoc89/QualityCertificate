using System.ComponentModel.DataAnnotations;

namespace QualityCertificate.Datas.Models
{
    public class VehicleByCardNumber
    {
        public string CardNumber { get; set; }
        public string VehicleCode { get; set; }
        public string SecondVehicleCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:ss}")]
        public DateTime CreatedDate { get; set; }
    }
}
