
using Dapper;
using HoangThach.AccountShared.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QualityCertificate.Datas.Entity;
using QualityCertificate.Datas.Models;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QualityCertificate.Services
{
    public class BulkCementCQService : IBulkCementCQ
    {
        private readonly WSMDbContext _context;
        private readonly IConfiguration _configuration;
        IDbConnection wsmDb { get { return new SqlConnection(_wsmDb); } }
        private readonly string? _wsmDb;
        IDbConnection wsmApplDb { get { return new SqlConnection(_wsmApplDb); } }
        private readonly string? _wsmApplDb;
        public BulkCementCQService(WSMDbContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
            _wsmDb = _configuration["ConnectionStrings:WSMDbContext"];
            _wsmApplDb = _configuration["ConnectionStrings:WSMAPPLDbContext"];
        }
        public async Task<List<BulkCementCQ>?> GetBulkCementCQ()
        {
            var data = await _context.BulkCementCQs.AsNoTracking().ToListAsync();
            if (data.Count > 0)
            {
                return data;
            }
          return new List<BulkCementCQ>();
        }
        public async Task<(BulkCementCQ BulkCementCQ, bool IsSuccess, string ErrorMsg)> GetBulkCementCQByVehicle(string vehicle)
        {
            return await GetCQByVehicle(vehicle);
        }
        public async Task<(string? VehicleCode, bool IsSuccess, string ErrorMsg)> GetVehicleByCardNumbers(string cardNumber)
        {
            return await GetVehicleCardNumbers(cardNumber);
        }

        public async Task<(string? VehicleCode, bool IsSuccess, string ErrorMsg)> GetVehicleCardNumbers(string cardNumber)
        {
            string Error_Msg = "Không có dữ liệu về xe theo thẻ này!";
        
            using (var connection = wsmApplDb)
            {
                connection.Open();
                string sql = "wsms.p_GetVehicleByCardNumbers";

                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        CardNumbers = cardNumber,
                    });


                    var data = connection.Query<VehicleByCardNumber?>(sql: sql, param: pars,

                    commandType: CommandType.StoredProcedure);

                    if (data != null && data.Count() > 0)
                    {
                        return (data.FirstOrDefault().VehicleCode, true, "Có dữ liệu về xe theo thẻ này");
                    }
                   
                }
                catch (Exception ex)
                {
                  return  ("", false, "Lỗi wsms.p_GetVehicleByCardNumbers: " + ex.Message);
                }

            }
            return ("", false, Error_Msg);
        }

        public async Task<(BulkCementCQ BulkCementCQ, bool IsSuccess, string ErrorMsg)> GetBulkCementCQByGid(Guid gid)
        {
            var data = await _context.BulkCementCQs.Where(x=>x.Gid == gid).ToListAsync();
            if (data.Count > 0)
            {
                return (data.FirstOrDefault(), true, "Có " + data.Count + "bản ghi");
            }
            return (new BulkCementCQ(), false, "Không tồn tại bản ghi!");
        }
        private async Task<(BulkCementCQ, bool IsSuccess, string ErrorMsg)> GetCQByVehicle(string vehicleCode)
        {
            string Error_Msg = "Không có dữ liệu BulkCementCQ theo xe";

            using (var connection = wsmDb)
            {
                connection.Open();
                string sql = "KCSs.P_GetBulkCementCQByVehicle";

                try
                {
                    var pars = new DynamicParameters();
                    pars.AddDynamicParams(new
                    {
                        vehicleCode = vehicleCode,
                    });


                    var data = connection.Query<BulkCementCQ?>(sql: sql, param: pars,

                    commandType: CommandType.StoredProcedure);

                    if (data != null && data.Count() > 0)
                    {
                        return (data.FirstOrDefault(), true, "Xe hợp lệ được phép in phiếu");
                    }

                   
                }
                catch (Exception ex)
                {
                    return (new BulkCementCQ(), false, "Lỗi KCSs.P_GetBulkCementCQByVehicle: " + ex.Message);
                }

            }
         return (new BulkCementCQ(), false, Error_Msg);
        }

      
    }
}
