using Application.Extensions;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Infrastructure.Extensions;

internal static class NPOIExtensions
{
    internal static void CreateCell(IRow xlsRow, int idx, string? value, ICellStyle style)
    {
        var cell = xlsRow.CreateCell(idx);
        cell.SetCellValue(value);
        cell.CellStyle = style;
    }

    internal static void CreateDataRow(ISheet sheet, IEnumerable<string?> values, ICellStyle style)
    {
        var row = sheet.CreateRow(sheet.LastRowNum + 1);
        values.WithIndex().ForEach(i => CreateCell(row, i.index, i.item, style));
    }

    internal static IFont CreateFont(this XSSFWorkbook workbook, string fontName, double size, bool bold)
    {
        var font = workbook.CreateFont();
        font.FontHeightInPoints = size;
        font.FontName = fontName;
        font.Color = IndexedColors.Black.Index;
        font.IsBold = bold;
        return font;
    }

    internal static void SetBorders(this ICellStyle style)
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

    internal static void SetBackgroundColor(this ICellStyle style, short colorIndex)
    {
        style.FillForegroundColor = colorIndex;
        style.FillPattern = FillPattern.SolidForeground;
    }
}
