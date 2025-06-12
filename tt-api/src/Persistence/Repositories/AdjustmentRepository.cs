using Application.Common.Models;
using Application.Interfaces.Repositories;
using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using Domain.Entities;
using Persistence.Extensions;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using Persistence.PetaPoco.Extensions;
using PetaPoco;
using PetaPoco.SqlKata;
using SqlKata;
using Application.Exceptions;
using Domain.ValueObjects;
using Persistence.Entities;
using Domain.Enums;

namespace Persistence.Repositories
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly Database dbContext;
        private readonly AppDbContext context;
        private readonly AutoMapper.IMapper mapper;

        public CodePrefix GetCodePrefix => CodePrefix.InventoryAdjustment;

        public AdjustmentRepository(IPPDbContextFactory factory, AppDbContext context, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.context = context;
            this.mapper = mapper;
        }

        // old CTT_InventoryController.GetInventoryAdjustmentMaster method
        public Adjustment? GetAdjustmentDetails(string jobNo)
        {
            var query = new Query(TT_InvAdjustmentMaster.SqlTableName)
                .Where("JobNo", jobNo);

            var adjustmentObject = dbContext.FirstOrDefault<TT_InvAdjustmentMaster>(query.ToSql());
            if (adjustmentObject?.JobNo == null)
                return null;

            var customerName = dbContext.FirstOrDefault<string>(new Query(TT_Customer.SqlTableName)
                .Where("Code", adjustmentObject.CustomerCode)
                .Where("WHSCode", adjustmentObject.WHSCode)
                .Select("Name").ToSql()
                );
            
            return mapper.Map<Adjustment>(adjustmentObject).SetCustomerName(customerName);
        }

        public PaginatedList<Adjustment> GetAdjustments(AdjustmentFilter filter, PaginationQuery pagination,
                                                             string? orderBy,
                                                             bool orderByDescending)
        {
            var query = new Query(TT_InvAdjustmentMaster.SqlTableName)
                .Join(TT_Customer.SqlTableName, j => j.On("CustomerCode", "Code")
                    .On($"{TT_InvAdjustmentMaster.SqlTableName}.WHSCode", $"{TT_Customer.SqlTableName}.WHSCode"));

            filter.ApplyFilter(query);

            query.Select(TT_InvAdjustmentMaster.SqlTableName + ".{CustomerCode, WhsCode, JobNo, RefNo, JobType, CreatedDate, Status, Reason}",
                   TT_Customer.SqlTableName + ".{Name}");

            string orderByColumnName = PrepareOrderByClause(orderBy);

            if (!orderByDescending)
                query.OrderBy(orderByColumnName);
            else
                query.OrderByDesc(orderByColumnName);

            return dbContext.PagedFetch<TT_InvAdjustmentMaster, TT_Customer, (TT_InvAdjustmentMaster, TT_Customer)>(pagination.PageNumber, pagination.ItemsPerPage,
                (adj, cust) => new(adj, cust), query)
                .ToPaginatedList(am => mapper.Map<Adjustment>(am.Item1).SetCustomerName(am.Item2.Name));
        }
        
        public int GetLastJobNumber(string prefix)
        {
            return dbContext.Single<int>($@"SELECT ISNULL(MAX(CAST(RIGHT(JobNo,5) AS INTEGER)),0) AS NumRecord FROM {TT_InvAdjustmentMaster.SqlTableName} WHERE JobNo LIKE '{prefix}%'"); 
        }

        public async Task<string> AddNewAdjustment(string whsCode, string customerCode, string userCode, InventoryAdjustmentJobType jobType, InventoryAdjustmentStatus status,  string jobNo, CancellationToken cancellationToken)
        {
            var adjustmentMaster = new TtInvAdjustmentMaster()
            {
                Whscode = whsCode,
                CustomerCode = customerCode,
                JobNo = jobNo,
                JobType = (byte) jobType,
                Status = (byte) status,
                CreatedBy = userCode,
                CreatedDate = DateTime.Now,
                //not filled fields are set as empty strings - desktop app compability
                RefNo = string.Empty,
                ConfirmedBy = string.Empty,
                CancelledBy = string.Empty,
                Reason = string.Empty,
            };
            context.TtInvAdjustmentMasters.Add(adjustmentMaster);
            await context.SaveChangesAsync();
            return jobNo;
        }

        public async Task Update(Adjustment updatedObject)
        {
            var existingObject = await context.TtInvAdjustmentMasters.FindAsync(updatedObject.JobNo);
            if (existingObject == null)
                throw new EntityDoesNotExistException();

            mapper.Map(updatedObject, existingObject);
        }

        private string PrepareOrderByClause(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
               return "JobNo";
            }
            string orderByResult = orderBy;
            bool addAdjustmentTablePrefix = true;
            switch(orderBy.ToLower())
            {
                case "referenceno": orderByResult = "RefNo"; break;
                case "customername": orderByResult = $"{TT_Customer.SqlTableName}.Name"; addAdjustmentTablePrefix = false; break;
                case "status":
                case "jobno":
                case "customercode":
                case "whscode":
                case "jobtype":
                case "createddate":
                case "reason": break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addAdjustmentTablePrefix)
                return orderByResult;
            return $"{TT_InvAdjustmentMaster.SqlTableName}.{orderByResult}";
        }
    }
}
