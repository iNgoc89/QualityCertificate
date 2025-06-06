using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualityCertificate.Datas.Entity
{
    [Table("BulkCementCQ")]
    public class BulkCementCQ
    {
        [Key]
        public int Id { get; set; }
        public int Source_Id { get; set; }
        public byte SourceTable_Id { get; set; }
        public Guid? Source_Gid { get; set; }
        [MaxLength(30)]
        public string? Region_Name { get; set; }
        [MaxLength(100)]
        public string? DeliveryGate_Name { get; set; }
        [MaxLength(30)]
        public string? CargoWeightStation_Name { get; set; }
        [MaxLength(300)]
        public string? Item_Name { get; set; }
        [MaxLength(20)]
        public string? DeliveryCode { get; set; }
        public int DeliveryQty_Kg { get; set; }
        public DateTime? Weight_1_Date { get; set; }
        public DateTime? Weight_2_Date { get;set; }
        [MaxLength(20)]
        public string? Vehicle_Code { get; set; }
        [MaxLength(20)]
        public string? Second_Vehicle_Code { get; set; }
        [MaxLength(20)]
        public string? Batch_Number { get; set; }
        [MaxLength(20)]
        public string? Quality_Number { get; set; }
        public DateTime? Quality_Date { get; set; }
        [MaxLength(200)]
        public string? Notes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? Gid { get; set; }
    }
}
