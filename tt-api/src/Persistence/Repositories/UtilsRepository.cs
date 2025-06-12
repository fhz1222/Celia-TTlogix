using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddArea;
using Domain.Entities;
using Domain.Enums;
using Persistence.Entities;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using PetaPoco;
using PetaPoco.Extensions;
using PetaPoco.SqlKata;
using SqlKata;

namespace Persistence.Repositories
{
    public class UtilsRepository : IUtilsRepository
    {
        private readonly Database dbContext;
        private readonly AutoMapper.IMapper mapper;
        public UtilsRepository(IPPDbContextFactory factory, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.mapper = mapper;
        }

        private static string TableNameFor(CodePrefix codePrefix) => codePrefix switch
        {
            CodePrefix.AddressContact => "TT_AddressContact",
            CodePrefix.PackageType => "TT_PackageType",
            CodePrefix.ProductCode => "TT_ProductCode",
            CodePrefix.ControlCode => "TT_ControlCode",
            CodePrefix.UOM => "TT_UOM",
            _ => throw new NotImplementedException()
        };

        public string GetNextCode(CodePrefix codePrefix)
        {
            string tableName = TableNameFor(codePrefix);
            var prefix = dbContext.Single<string>(@$"SELECT Prefix FROM TT_JobCode WHERE Code = {(int)codePrefix}");
            var num = dbContext.Single<int>(@$"SELECT ISNULL(MAX(CAST(RIGHT(Code,4) AS INTEGER)),0) FROM {tableName} WHERE Code LIKE '{prefix}%'");
            var code = $"{prefix}{num + 1:D4}";
            return code;
        }

        public string GetAddressBookAutoNum(string companyCode)
        {
            var num = dbContext.Single<int>(@$"SELECT ISNULL(MAX(CAST(RIGHT(Code, 2) AS INTEGER)), 0) FROM TT_AddressBook WHERE CompanyCode = '{companyCode}'");
            var code = $"{num + 1:D2}";
            return code;
        }

        public string GetJobCode(CodePrefix codePrefix)
        {
            return dbContext.Single<string>(@$"SELECT Prefix FROM TT_JobCode WHERE Code = {(int)codePrefix}");
        }

        public bool CheckIfWhsCodeExists(string code)
        {
            return dbContext.SingleOrDefault<string?>(@$"SELECT Code FROM TT_Warehouse WHERE Code = '{code}'") != null;
        }

        public bool CheckIfCustomerCodeExists(string code)
        {
            return dbContext.SingleOrDefault<string?>(@$"SELECT TOP 1 Code FROM TT_Customer WHERE Code = '{code}'") != null;
        }

        public bool CheckIfUserCodeExists(string code)
        {
            return dbContext.SingleOrDefault<string?>(@$"SELECT Code FROM TT_SystemUser WHERE Code = '{code}'") != null;
        }

        public bool CheckIfAreaTypeExists(string code)
        {
            return dbContext.SingleOrDefault<string?>(@$"SELECT Code FROM TT_AreaType WHERE Code = '{code}'") != null;
        }

        public bool CheckIfAreaExists(string code, string whsCode)
        {
            return dbContext.SingleOrDefault<string?>(@$"SELECT Code FROM TT_Area WHERE Code = '{code}' AND WHSCode = '{whsCode}'") != null;
        }

        public Owner? GetOwner(string code)
        {
            var query = new Query(TT_Owner.SqlTableName)
                .Where("Code", code);

            var owner = dbContext.FirstOrDefault<TT_Owner>(query.ToSql());
            if (owner == null)
                return null;
            return mapper.Map<Owner>(owner);
        }

        public async Task<AccessLock?> GetAccessLockAsync(string jobNo)
        {
            var query = new Query(TT_AccessLock.SqlTableName)
                .Where("JobNo", jobNo);

            var accessLock = dbContext.FirstOrDefault<TT_AccessLock>(query.ToSql());
            if (accessLock?.JobNo == null)
                return null;
            return mapper.Map<AccessLock>(accessLock);
        }
    }
}
