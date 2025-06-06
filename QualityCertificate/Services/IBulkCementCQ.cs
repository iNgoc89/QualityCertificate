using QualityCertificate.Datas.Entity;
using QualityCertificate.Datas.Models;

namespace QualityCertificate.Services
{
    public interface IBulkCementCQ
    {
        Task<List<BulkCementCQ>?> GetBulkCementCQ();
        Task<(BulkCementCQ BulkCementCQ, bool IsSuccess, string ErrorMsg)> GetBulkCementCQByVehicle(string vehicleCode);
        Task<(string? VehicleCode, bool IsSuccess, string ErrorMsg)> GetVehicleByCardNumbers(string cardNumber);
        Task<(BulkCementCQ BulkCementCQ, bool IsSuccess, string ErrorMsg)> GetBulkCementCQByGid(Guid gid);


    }
}
