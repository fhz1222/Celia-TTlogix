using Application.Common.Models;
using Application.UseCases.InvoiceRequest;
using Application.UseCases.InvoiceRequest.Commands.UploadBatch;
using Application.UseCases.InvoiceRequest.Commands.ValidatePrice;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IInvoiceRequestRepository
{
    InvRequestFlow GetFlow();
    int GetRelevancyThreshold();
    bool GetNoPriceValidationStatus();
    Task<List<FactoryDto>> GetFactoriesForSupplier(string supplierId);
    List<JobForSupplier> GetOutboundsEligibleForCustomsClearanceFlow(int relevancy);
    List<JobForSupplier> GetOutboundsEligibleForStandardFlow(int relevancy);
    List<JobForSupplier> GetStockTransfersEligibleForCustomsClearanceFlow(int relevancy);
    List<JobForSupplier> GetStockTransfersEligibleForStandardFlow(int relevancy);
    Task<List<ProductLineDto>> GetProductsForInvoiceRequest(params string[] jobs);
    Task CreateInvoiceRequest(string factoryId, string supplierId, string jobNo, string refNo, string user, List<ProductLineDto> productLines);
    Task<PaginatedList<InvoiceBatchDto>> GetBatches(GetBatchesQueryFilter filter, PaginationQuery pagination);
    Task Block(string jobNo, string user);
    void Unblock(string jobNo);
    Task<InvoiceBatch?> GetBatch(int batchId);
    Task<List<InvoiceRequest>> GetRequests(int batchId);
    Task<List<InvoiceRequest>> GetRequests(List<int> requestIds);
    Task Update(InvoiceBatch batch);
    Task Update(InvoiceRequest request);
    Task UpsertCustomsAgencyData(int batchId, int hour, string? comment);
    Task<(byte[], string)> GetInvoiceFile(int id);
    InvoiceRequest? GetRequest(string jobNo);
    bool IsOnBlocklist(string jobNo);
    Task<List<JobDto>> GetOpenRequests(string factoryId, string supplierId);
    Task<int> SavePriceValidation(string currency, decimal supplierPrice, decimal ttlogixPrice, bool success, List<int> requests, string user);
    Task<List<PriceInfo>> GetPrices(List<int> requestIds);
    List<int> GetValidationRequestIds(int validationId);
    Task<int> CreateBatch(string batchNumber, string supplierId, string factoryId, string userCode, List<int> requestIds, List<UploadInvoiceDto> invoices, string currency);
    Task<string> GetNextBatchNumber(string factoryId, string supplierId);
    Task<PriceValidation?> GetPriceValidation(int priceValidationId);
    void DeletePriceValidation(int validationId);
    CustomerSupplierDto GetCustomerSupplierData(string factoryId, string supplierId);
    Task<List<NamedStream>> GetInvoiceFiles(int batchId);
    JobForSupplier GetJob(string jobNo);
    (string departureHour, string? comment)? GetCustomsInfo(int batchId);
    Task DeleteInvoiceFiles(int batchId);
    Task UpdateJobCommercialInvNumber(int batchId, List<string> jobs);
    void AddForCustomsAgencyIntegration(string jobNo);
    Task<List<FactoryDto>> GetFactoriesForWarehouse(string whsCode);
}
