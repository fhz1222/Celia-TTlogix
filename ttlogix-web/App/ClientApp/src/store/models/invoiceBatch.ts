export class InvoiceBatch {
        batchId: number
        batchNumber: string
        supplierName: string
        uploadedOn: string //date
        uploadedBy: string
        status: "Approved" | "Rejected" | "PendingApproval"
        comment?: string
        truckDepartureHour: number //time
        loadingEtd?: string //date
        invoices: Array<{
            invoiceNumber: string
            value: number
            fileId: number,
            currency: string
        }>
        jobs: Array<{
            type: "Outbound" | "StockTransfer"
            jobNo: string
            deliveryDocket?: string
            details: Array<{
                asnNo: string
                productCode: string
                poNumber: string
                poLineNo: string
                qty: number

            }> 
        }>
}