using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using TT.Core.QueryResults;
using TT.Services.Interfaces;

namespace TT.Services.Services
{
    public class XlsService : IXlsService
    {
        public Stream WriteOutstandingInboundsToXls(IEnumerable<OutstandingInboundForReportQueryResult> data)
        {
            var ms = new MemoryStream();
            var workbook = new XSSFWorkbook(XSSFWorkbookType.XLSX);
            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

            var fontArial12 = CreateFont(workbook, 12, true);
            var fontBoldArial10 = CreateFont(workbook, 10, true);
            var fontArial10 = CreateFont(workbook, 10, false);

            var styleTitle = CreateStyle(workbook, fontArial12, false, false);
            var styleTable = CreateStyle(workbook, fontArial10, true, false);
            var styleTableHeader = CreateStyle(workbook, fontBoldArial10, true, true);

            var rowIdx = 0;
            IRow rowTitle = sheet1.CreateRow(rowIdx);
            var cellTitle = rowTitle.CreateCell(0);
            cellTitle.SetCellValue("OUTSTANDING INBOUND");
            cellTitle.CellStyle = styleTitle;

            rowIdx++;
            rowIdx++;

            IRow rowDate = sheet1.CreateRow(rowIdx);
            var cellDate = rowDate.CreateCell(0);
            cellDate.SetCellValue("Date:");
            cellDate.CellStyle = styleTableHeader;
            var cellDate2 = rowDate.CreateCell(1);
            cellDate2.SetCellValue(DateTime.Now.Date.ToString("dd MMM yyyy"));
            cellDate2.CellStyle = styleTableHeader;

            rowIdx++;
            var rowTableHeader = sheet1.CreateRow(rowIdx);
            var cl0 = rowTableHeader.CreateCell(0); cl0.SetCellValue("S/N"); cl0.CellStyle = styleTableHeader;
            var cl1 = rowTableHeader.CreateCell(1); cl1.SetCellValue("Inbound Job No"); ; cl1.CellStyle = styleTableHeader;
            var cl2 = rowTableHeader.CreateCell(2); cl2.SetCellValue("ASNNo"); ; cl2.CellStyle = styleTableHeader;
            var cl3 = rowTableHeader.CreateCell(3); cl3.SetCellValue("Type of Inbound"); ; cl3.CellStyle = styleTableHeader;
            var cl4 = rowTableHeader.CreateCell(4); cl4.SetCellValue("Received Date"); ; cl4.CellStyle = styleTableHeader;
            var cl5 = rowTableHeader.CreateCell(5); cl5.SetCellValue("System Status"); ; cl5.CellStyle = styleTableHeader;
            var cl6 = rowTableHeader.CreateCell(6); cl6.SetCellValue("Remark"); ; cl6.CellStyle = styleTableHeader;
            rowIdx++;

            foreach (var item in data)
            {
                IRow xlsRow = sheet1.CreateRow(rowIdx);
                var c0 = xlsRow.CreateCell(0, CellType.Numeric); c0.SetCellValue(rowIdx - 3); c0.CellStyle = styleTable;
                var c1 = xlsRow.CreateCell(1); c1.SetCellValue(item.JobNo); c1.CellStyle = styleTable;
                var c2 = xlsRow.CreateCell(2); c2.SetCellValue(item.ASN); c2.CellStyle = styleTable;
                var c3 = xlsRow.CreateCell(3); c3.SetCellValue(item.InboundType); c3.CellStyle = styleTable;
                var c4 = xlsRow.CreateCell(4); c4.SetCellValue(item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")); c4.CellStyle = styleTable;
                var c5 = xlsRow.CreateCell(5); c5.SetCellValue(item.StatusString); c5.CellStyle = styleTable;
                var c6 = xlsRow.CreateCell(6); c6.CellStyle = styleTable;
                rowIdx++;
            }

            sheet1.SetColumnWidth(0, 2000);
            sheet1.SetColumnWidth(1, 4000);
            sheet1.SetColumnWidth(2, 6500);
            sheet1.SetColumnWidth(3, 4200);
            sheet1.SetColumnWidth(4, 5500);
            sheet1.SetColumnWidth(5, 4000);
            sheet1.SetColumnWidth(6, 5000);

            workbook.Write(ms, true);
            ms.Position = 0;
            return ms;
        }
        private IFont CreateFont(XSSFWorkbook workbook, double sizeInPoints, bool bold)
        {
            var font = workbook.CreateFont();
            font.FontHeightInPoints = sizeInPoints;
            font.FontName = "Arial";
            font.Color = IndexedColors.Black.Index;
            font.IsBold = bold;
            return font;
        }

        private ICellStyle CreateStyle(XSSFWorkbook workbook, IFont font, bool borders, bool background)
        {
            var style = workbook.CreateCellStyle();
            style.SetFont(font);
            if (borders)
            {
                style.BottomBorderColor = HSSFColor.Black.Index;
                style.LeftBorderColor = HSSFColor.Black.Index;
                style.RightBorderColor = HSSFColor.Black.Index;
                style.TopBorderColor = HSSFColor.Black.Index;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
            }
            if (background)
            {
                style.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
            }
            return style;
        }
    }
}
