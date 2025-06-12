using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Services;
using static QRCoder.PayloadGenerator;

namespace TT.Services.Label
{
    public class SatoLabelFactory : LabelFactory
    {
        private readonly IInventoryService inventoryService;
        private readonly IInboundService inboundService;
        private readonly ITTLogixRepository repository;
        private readonly AppSettings appSettings;

        #region Contants
        private const int ESC = 27;
        private const int STX = 2;
        private const int ETX = 3;
        private const char DELIMITER = '\u0005';
        private const char CHARDOLLAR = '$';
        private const char CHARCROSS = '#';

        private const int lenBulkReceiveLabel = 35;
        private const int lenPalletLabel = 30 + 1;
        private const int lenLocationLabel = 35;
        private const int lenStorageLabel = 100 + 1;
        private const int MaxLen = 250;
        #endregion

        #region Class Enumerations
        /// <summary>
        /// List of available label type
        /// </summary>
        /// 
        private enum LABELTYPE : int
        {
            QRPRINT_BULKRECEIVELABEL = 1,
            QRPRINT_PALLETLABEL = 2,
            QRPRINT_LOCATIONLABEL = 3,
            QRPRINT_STORAGELABEL = 4,
            QRPRINT_SPLITLABEL = 5,
            QRPRINT_STORAGEADJUSTLABEL = 6,
            QRPRINT_ORIGINALLABEL = 7,
            QRPRINT_DELIVERYLABEL = 8,
            QRPRINT_PARTIALLABEL = 9,
            QRPRINT_STOCKTRANSFERLABEL = 10,
            QRPRINT_STORAGEDETAILLABEL = 11
        }

        private enum EXPANDFACTOR : int
        {
            Multiple_1 = 1,
            Multiple_2 = 2,
            Multiple_3 = 3,
            Multiple_4 = 4,
            Multiple_5 = 5,
            Multiple_6 = 6,
            Multiple_7 = 7,
            Multiple_8 = 8,
            Multiple_9 = 9,
            Multiple_10 = 10,
            Multiple_11 = 11,
            Multiple_12 = 12
        }

        private enum PRINTROTATION : int
        {
            NoRotate = 0,
            Rotate90CCW = 1,
            Rotate180 = 2,
            Rotate270CCW = 3
        }

        private enum NORMAL_FONT : int
        {
            U_NP5Wx9H = 1,
            S_NP8Wx15H = 2,
            M_NP13Wx20H = 3,
            OCR_A = 4,
            OCR_B = 5,
            XU_P5Wx9H = 6,
            XS_P17Wx17H = 7,
            XM_P24Wx24H = 8
        }

        private enum SMOOTH_FONT : int
        {
            WB_NP18Wx30H = 1,
            WL_NP28Wx52H = 2,
            XB_P48Wx48HBold = 3,
            XL_P48Wx48H = 4
        }

        private enum FONT_VARIATION : int
        {
            Standard = 0,
            Outlined = 1,
            GrayPattern1 = 2,
            GrayPattern2 = 3,
            GrayPattern3 = 4,
            StandardShadow1 = 5,
            StandardShadow2 = 6,
            StandardMirrorImg = 7,
            Italic = 8,
            ItalicShadow = 9,
        }

        private enum SPACING : int
        {
            Proportional = 0,
            Fixed = 1
        }
        private enum AUTO_SMOOTH : int
        {
            Disable = 0,
            Enable = 1
        }
        private enum ORIENTATION
        {
            Horizontal = 1,
            Vertical = 2
        }
        #endregion

        private void StartPrint()
        {
            sb.Append(Convert.ToChar(ESC));
            sb.Append(Convert.ToChar(STX));
            sb.Append(Convert.ToChar(ESC));
            sb.Append(Convert.ToChar('A'));

        }

        private void StartEnlargePrint()
        {
            sb.Append(Convert.ToChar(ESC));
            sb.Append(Convert.ToChar(STX));
            sb.Append(Convert.ToChar(ESC));
            sb.Append(Convert.ToChar('A'));
            sb.Append(Convert.ToChar(ESC));
            sb.Append(Convert.ToChar('A'));
            sb.Append(Convert.ToChar('X'));
        }

        private void EndPrint()
        {
            sb.Append(Convert.ToChar(ESC));
            sb.Append("Q1");
            sb.Append(Convert.ToChar(13));
            sb.Append(Convert.ToChar(ESC));
            sb.Append("Z");
            sb.Append(Convert.ToChar(ETX));
            sb.Append(Convert.ToChar(13));
        }

        private void PrintText(int vCol, int hCol, string TextToPrint, NORMAL_FONT Font,
            EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand)
        {
            PrintText(vCol, hCol, TextToPrint, Font, HorizontalExpand, VerticalExpand, false, false);
        }

        private void PrintText(int vCol, int hCol, string TextToPrint, NORMAL_FONT Font,
            EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand, bool bRotateText)
        {
            PrintText(vCol, hCol, TextToPrint, Font, HorizontalExpand, VerticalExpand, bRotateText, false);
        }

        private void PrintText(int vCol, int hCol, string TextToPrint, NORMAL_FONT Font,
                                EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand, bool bRotateText, bool EnlargePrint)
        {
            string l_strExpand = "";
            if (!EnlargePrint)
            {
                if ((hCol > 2136) && (hCol < 0))
                {
                    sb.Append("X coordinate overflow. (0-2136)"); return;
                }
                if ((vCol > 1248) && (vCol < 0))
                {
                    sb.Append("Y coordinate overflow. (0-1248)"); return;
                }
            }
            // assumes always will have EXPANDFACTOR
            l_strExpand =
                Convert.ToString(Convert.ToChar(ESC)) + "L" + Convert.ToInt32(HorizontalExpand).ToString("00") + Convert.ToInt32(VerticalExpand).ToString("00");

            if (bRotateText)
            {
                sb.Append(Convert.ToChar(ESC));
                sb.Append("R");
            }

            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append(l_strExpand);

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case NORMAL_FONT.U_NP5Wx9H:
                    sb.Append("U");
                    break;
                case NORMAL_FONT.S_NP8Wx15H:
                    sb.Append("S");
                    break;
                case NORMAL_FONT.M_NP13Wx20H:
                    sb.Append("M");
                    break;
                case NORMAL_FONT.OCR_A:
                    sb.Append("OA");
                    break;
                case NORMAL_FONT.OCR_B:
                    sb.Append("OB");
                    break;
                case NORMAL_FONT.XU_P5Wx9H:
                    sb.Append("XU");
                    break;
                case NORMAL_FONT.XS_P17Wx17H:
                    sb.Append("XS");
                    break;
                case NORMAL_FONT.XM_P24Wx24H:
                    sb.Append("XM");
                    break;
            }
            sb.Append(TextToPrint);
        }

        private void PrintText(int vCol, int hCol, string TextToPrint, NORMAL_FONT Font)
        {
            string l_strExpand = "";

            if ((hCol > 2136) && (hCol < 0))
            {
                throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                //l_strResult = "X coordinate overflow. (0-2136)"; return l_strResult;
            }
            if ((vCol > 1248) && (vCol < 0))
            {
                throw new System.ApplicationException("Y coordinate overflow. (0-1248)");
                //l_strResult = "Y coordinate overflow. (0-1248)"; return l_strResult;
            }

            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append(l_strExpand);

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case NORMAL_FONT.U_NP5Wx9H:
                    sb.Append("U");
                    break;
                case NORMAL_FONT.S_NP8Wx15H:
                    sb.Append("S");
                    break;
                case NORMAL_FONT.M_NP13Wx20H:
                    sb.Append("M");
                    break;
                case NORMAL_FONT.OCR_A:
                    sb.Append("OA");
                    break;
                case NORMAL_FONT.OCR_B:
                    sb.Append("OB");
                    break;
                case NORMAL_FONT.XU_P5Wx9H:
                    sb.Append("XU");
                    break;
                case NORMAL_FONT.XS_P17Wx17H:
                    sb.Append("XS");
                    break;
                case NORMAL_FONT.XM_P24Wx24H:
                    sb.Append("XM");
                    break;
            }
            sb.Append(TextToPrint);
        }

        private void PrintSmoothText(int vCol, int hCol, string TextToPrint, SMOOTH_FONT Font,
                                       AUTO_SMOOTH AutoSmooth, EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand)
        {
            PrintSmoothText(vCol, hCol, TextToPrint, Font,
                                       AutoSmooth, HorizontalExpand, VerticalExpand, false);
        }


        private void PrintSmoothText(int vCol, int hCol, string TextToPrint, SMOOTH_FONT Font,
                                       AUTO_SMOOTH AutoSmooth, EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand, bool EnlargePrint)
        {
            string l_strExpand = "";

            if (!EnlargePrint)
            {
                if ((hCol > 2136) && (hCol < 0))
                {
                    throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                    //l_strResult = "X coordinate overflow. (0-2136)";

                }
                if ((vCol > 1248) && (vCol < 0))
                {
                    throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                    //l_strResult = "Y coordinate overflow. (0-1248)";
                }
            }
            l_strExpand =
                Convert.ToString(Convert.ToChar(ESC)) + "L" + Convert.ToInt32(HorizontalExpand).ToString("00") +
                Convert.ToInt32(VerticalExpand).ToString("00");

            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append(l_strExpand);

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case SMOOTH_FONT.WB_NP18Wx30H:
                    sb.Append("WB");
                    break;
                case SMOOTH_FONT.WL_NP28Wx52H:
                    sb.Append("WL");
                    break;
                case SMOOTH_FONT.XB_P48Wx48HBold:
                    sb.Append("XB");
                    break;
                case SMOOTH_FONT.XL_P48Wx48H:
                    sb.Append("XL");
                    break;

            }
            sb.Append(Convert.ToInt32(AutoSmooth));
            sb.Append(TextToPrint);
        }


        private void PrintSmoothText(int vCol, int hCol, string TextToPrint, SMOOTH_FONT Font,
                                       AUTO_SMOOTH AutoSmooth)
        {
            string l_strExpand = "";

            if ((hCol > 2136) && (hCol < 0))
            {
                throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                //l_strResult = "X coordinate overflow. (0-2136)";

            }
            if ((vCol > 1248) && (vCol < 0))
            {
                throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                //l_strResult = "Y coordinate overflow. (0-1248)";
            }
            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append(l_strExpand);

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case SMOOTH_FONT.WB_NP18Wx30H:
                    sb.Append("WB");
                    break;
                case SMOOTH_FONT.WL_NP28Wx52H:
                    sb.Append("WL");
                    break;
                case SMOOTH_FONT.XB_P48Wx48HBold:
                    sb.Append("XB");
                    break;
                case SMOOTH_FONT.XL_P48Wx48H:
                    sb.Append("XL");
                    break;

            }
            sb.Append(Convert.ToInt32(AutoSmooth));
            sb.Append(TextToPrint);
        }


        private void PrintSmoothTextWithoutHV(string TextToPrint, SMOOTH_FONT Font,
                                                AUTO_SMOOTH AutoSmooth, EXPANDFACTOR HorizontalExpand, EXPANDFACTOR VerticalExpand)
        {
            sb.Append(Convert.ToChar(ESC));

            sb.Append(Convert.ToChar(ESC));
            sb.Append("L");
            sb.Append(Convert.ToInt32(HorizontalExpand).ToString("00"));
            sb.Append(Convert.ToInt32(VerticalExpand).ToString("00"));

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case SMOOTH_FONT.WB_NP18Wx30H:
                    sb.Append("WB");
                    break;
                case SMOOTH_FONT.WL_NP28Wx52H:
                    sb.Append("WL");
                    break;
                case SMOOTH_FONT.XB_P48Wx48HBold:
                    sb.Append("XB");
                    break;
                case SMOOTH_FONT.XL_P48Wx48H:
                    sb.Append("XL");
                    break;
            }
            sb.Append(Convert.ToInt32(AutoSmooth));
            sb.Append(TextToPrint);
        }

        private void PrintSmoothTextWithoutHV(string TextToPrint, SMOOTH_FONT Font,
            AUTO_SMOOTH AutoSmooth)
        {

            sb.Append(Convert.ToChar(ESC));

            sb.Append(Convert.ToChar(ESC));
            switch (Font)
            {
                case SMOOTH_FONT.WB_NP18Wx30H:
                    sb.Append("WB");
                    break;
                case SMOOTH_FONT.WL_NP28Wx52H:
                    sb.Append("WL");
                    break;
                case SMOOTH_FONT.XB_P48Wx48HBold:
                    sb.Append("XB");
                    break;
                case SMOOTH_FONT.XL_P48Wx48H:
                    sb.Append("XL");
                    break;
            }
            sb.Append(Convert.ToInt32(AutoSmooth));
            sb.Append(TextToPrint);
        }

        private void PrintLine(int vCol, int hCol, int WidthInDot, ORIENTATION LineOrientation, int LengthOfLine)
        {
            PrintLine(vCol, hCol, WidthInDot, LineOrientation, LengthOfLine, false);
        }

        private void PrintLine(int vCol, int hCol, int WidthInDot, ORIENTATION LineOrientation, int LengthOfLine, bool EnlargePrint)
        {
            if (!EnlargePrint)
            {
                if ((hCol > 2136) && (hCol < 0))
                {
                    throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                }
                if ((vCol > 1248) && (vCol < 0))
                {
                    throw new System.ApplicationException("Y coordinate overflow. (0-1248)");
                }
                if ((WidthInDot > 99) || (WidthInDot < 0))
                {
                    throw new System.ApplicationException("Line Width overflow. (0-99)");
                }
            }
            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append("FW");
            sb.Append(WidthInDot.ToString("00"));
            switch (LineOrientation)
            {
                case ORIENTATION.Horizontal:
                    if (((LengthOfLine > 1248) || (LengthOfLine < 0)) && EnlargePrint)
                        throw new System.ApplicationException("Length of line overflow. (0-1248)");

                    sb.Append("H");
                    break;
                case ORIENTATION.Vertical:
                    if (((LengthOfLine > 2136) || (LengthOfLine < 0)) && EnlargePrint)
                        throw new System.ApplicationException("Length of line overflow. (0-2136)");

                    sb.Append("V");
                    break;
            }
            sb.Append(LengthOfLine.ToString("00000"));
        }

        private void PrintBox(int vCol, int hCol, int HorizontalWidth, int VerticalWidth, int HorizontalLength, int VerticalLength)
        {
            PrintBox(vCol, hCol, HorizontalWidth, VerticalWidth, HorizontalLength, VerticalLength, false);
        }

        private void PrintBox(int vCol, int hCol, int HorizontalWidth, int VerticalWidth, int HorizontalLength, int VerticalLength, bool EnlargePrint)
        {
            if (!EnlargePrint)
            {
                if ((hCol > 2136) && (hCol < 0))
                {
                    throw new System.ApplicationException("X coordinate overflow. (0-2136)");
                }
                if ((vCol > 1248) && (vCol < 0))
                {
                    throw new System.ApplicationException("Y coordinate overflow. (0-1248)");
                }
                if ((HorizontalWidth > 99) || (HorizontalWidth < 0))
                {
                    throw new System.ApplicationException("Line Horizontal Width overflow. (0-99)");
                }
                if ((VerticalWidth > 99) || (VerticalWidth < 0))
                {
                    throw new System.ApplicationException("Line Vertical Width overflow. (0-99)");
                }
            }
            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append("FW");
            sb.Append(HorizontalWidth.ToString("00"));
            sb.Append(VerticalWidth.ToString("00"));
            sb.Append("H");
            sb.Append(HorizontalLength.ToString("00000"));
            sb.Append("V");
            sb.Append(VerticalLength.ToString("00000"));
        }


        private void PrintReverseImage(int vCol, int hCol, int HorizontalLength, int VerticalLength)
        {
            if ((hCol > 2136) && (hCol < 0))
            {
                throw new System.ApplicationException("X coordinate overflow. (0-2136)");
            }
            if ((vCol > 1248) && (vCol < 0))
            {
                throw new System.ApplicationException("Y coordinate overflow. (0-1248)");
            }
            if ((HorizontalLength > 1248) || (HorizontalLength < 1))
            {
                throw new System.ApplicationException("Horizontal Length overflow. (1-1248)");
            }
            if ((VerticalLength > 2136) || (VerticalLength < 1))
            {
                throw new System.ApplicationException("Vertical Length overflow. (0-2136)");
            }
            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append("(");
            sb.Append(HorizontalLength);
            sb.Append(",");
            sb.Append(VerticalLength);
        }

        private void PrintQRCode(int vCol, int hCol, int CodeSize, string TextToCode, int CodeLength, int Type)
        {
            VH(vCol, hCol);

            if (Type == 1)
            {
                sb.Append(Convert.ToChar(ESC));
                sb.Append("BQ20");
                sb.Append(Convert.ToInt32(CodeSize).ToString("0#"));
                sb.Append(",0");
                this.FillString(TextToCode, CodeLength);
            }
            else
            {
                sb.Append(Convert.ToChar(ESC));
                sb.Append("2D30,M,");
                sb.Append(Convert.ToInt32(CodeSize).ToString());
                sb.Append(",1,0");

                sb.Append(Convert.ToChar(ESC));
                sb.Append("DS");
                this.FillString(TextToCode, CodeLength);
            }
        }

        private void PrintBarCode(int vCol, int hCol, int narrowBar, int height, string purposeCode, string TextToCode)
        {
            VH(vCol, hCol);

            sb.Append(Convert.ToChar(ESC));
            sb.Append("B1");
            sb.Append(Convert.ToInt32(narrowBar).ToString("00"));
            sb.Append(Convert.ToInt32(height).ToString("000"));
            sb.Append("*");
            sb.Append(purposeCode);
            sb.Append(TextToCode);
            sb.Append("*");
        }

        private void FillString(string InputText, int DesireLength)
        {

            if ((DesireLength - InputText.Length) > 0)
            {
                sb.Append(InputText.Trim().PadRight(DesireLength));
            }
            else
            {
                sb.Append(InputText.Substring(0, DesireLength));
            }
        }

        private string KeepString(string InputText, int DesireLength)
        {

            if ((DesireLength - InputText.Length) > 0)
            {
                return InputText.Trim().PadRight(DesireLength);
            }
            else
            {
                return InputText.Substring(0, DesireLength);
            }
        }

        private void VH(int vCol, int hCol)
        {
            sb.Append(Convert.ToChar(ESC));
            sb.Append("V");
            sb.Append(vCol.ToString("000"));

            sb.Append(Convert.ToChar(ESC));
            sb.Append("H");
            sb.Append(hCol.ToString("000"));
        }

        public SatoLabelFactory(IInboundService inbound, IInventoryService inventoryService, ITTLogixRepository repository, IOptions<AppSettings> appSettings) : base()
        {
            this.inventoryService = inventoryService;
            this.inboundService = inbound;
            this.repository = repository;
            this.appSettings = appSettings.Value;
        }


        public async override Task Print(int copies)
        {
            var printString = string.Concat(Enumerable.Repeat(sb.ToString(), copies));
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(printer.IP, 1024);
                var asen = new ASCIIEncoding();
                var stm = client.GetStream();
                byte[] ba = asen.GetBytes(printString.ToCharArray());
                await stm.WriteAsync(ba);
                client.Close();
            }
            catch (SocketException) { throw new PrinterUnavailableException(); }
        }

        private async Task<string> LabelOrDefault(string type, string def)
        {
            if (string.IsNullOrEmpty(type))
            {
                return def;
            }
            var obj = await repository.GetProductCodeAsync(type);
            if (obj != null && obj.Name.Trim().Length > 0)
            {
                return obj.Name;
            }
            return def;
        }

        private string DecimalFormat(int decimals)
        {

            if (decimals > 0)
            {
                return "." + string.Concat(Enumerable.Repeat("0", decimals));
            }

            return "#";
        }

        public async override Task AddLabel(StorageDetail pid, ILabelFactory.LabelType type)
        {

            var inventoryControl = await repository.GetInventoryControlAsync(pid.CustomerCode);
            var productCode1Name = await LabelOrDefault(inventoryControl.PC1Type, "Product Code 1");
            var productCode2Name = await LabelOrDefault(inventoryControl.PC2Type, "Product Code 2");
            var controlCode1Name = await LabelOrDefault(inventoryControl.CC1Type, "Control Code 1");
            var controlCode2Name = await LabelOrDefault(inventoryControl.CC2Type, "Control Code 1");
            var partMaster = await repository.PartMasters().Where(x => x.ProductCode1 == pid.ProductCode && x.SupplierID == pid.SupplierID && x.CustomerCode == pid.CustomerCode).FirstAsync();
            var inbound = await inboundService.GetInbound(pid.InJobNo);
            var uom = await repository.UOMs().Where(x => x.Code == partMaster.UOM).FirstAsync();
            var uomDecimal = await repository.UOMDecimals().Where(x => x.CustomerCode == pid.CustomerCode && x.UOM == partMaster.UOM).FirstAsync();
            var decimalFormat = DecimalFormat(uomDecimal.DecimalNum);

            if (type == ILabelFactory.LabelType.SMALL)
            {
                StartPrint();
                int intVOffset = 2;
                int intHOffset = 10;
                PrintBox(15 + intVOffset, 50 + intHOffset, 6, 6, 1170, 735);
                PrintLine(190 + intVOffset, 400 + intHOffset, 3, ORIENTATION.Horizontal, 820); // 02
                PrintLine(380 + intVOffset, 50 + intHOffset, 5, ORIENTATION.Horizontal, 1170); // 02
                PrintLine(480 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 03
                PrintLine(560 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 04
                PrintLine(640 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 05

                PrintLine(15 + intVOffset, 400 + intHOffset, 3, ORIENTATION.Vertical, 370);        // 08
                PrintLine(190 + intVOffset, 870 + intHOffset, 3, ORIENTATION.Vertical, 190);
                PrintLine(480 + intVOffset, 600 + intHOffset, 3, ORIENTATION.Vertical, 265);   // 08

                #region Step 4.8 : Print QR Code
                #region QR Code
                var QRCode = "<S1>" +
                KeepString(pid.PID, 20) + Convert.ToString(DELIMITER) +
                KeepString(pid.ProductCode, 30) + Convert.ToString(DELIMITER) +
                KeepString(partMaster != null ? partMaster.ProductCode2 : "", 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.InboundDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.Qty.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), 25) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlDate.HasValue ? pid.ControlDate.Value.ToString("ddMMyyyy") : "", 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode1, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode2, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode3, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.Version.ToString(), 4) + Convert.ToString(DELIMITER) +
                KeepString(pid.SupplierID, 10) + Convert.ToString(DELIMITER) +
                KeepString(pid.CustomerCode, 10) + Convert.ToString(DELIMITER);


                PrintQRCode(50, 100, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                #region Step 4.9 : Print Product Code
                //Print Product Code
                PrintText(35, 420, productCode1Name + ":",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);


                if (pid.ProductCode.Length < 15)
                    PrintSmoothText(60, 420,
                        pid.ProductCode,
                        SMOOTH_FONT.XB_P48Wx48HBold,
                        AUTO_SMOOTH.Enable,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_3);
                else
                    PrintSmoothText(60, 420,
                        pid.ProductCode,
                        SMOOTH_FONT.XB_P48Wx48HBold,
                        AUTO_SMOOTH.Enable,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_2);


                #endregion

                #region Step 4.10 : Print Quantity
                // Print quantity
                PrintText(210, 420, "Quantity (" + uom.Name + "s): " + (pid.ParentID != null ? "[SPLIT]" : ""),
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);



                // Create decimal format mask

                PrintSmoothText(240, 420, pid.Qty.ToString(DecimalFormat(uomDecimal.DecimalNum)),
                                                         SMOOTH_FONT.XB_P48Wx48HBold,
                                                         AUTO_SMOOTH.Enable,
                                                         EXPANDFACTOR.Multiple_2,
                                                         EXPANDFACTOR.Multiple_3);

                #endregion

                #region  Step 4.16  Print Version
                // Print version
                PrintText(350, 100,
                    "Version: " + pid.Version,
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                #endregion

                #region  Step 4.16  Print LABEL COUNT
                // Print version

                #endregion

                #region  Step 4.17   Supplier ID
                // Print Supplier ID
                PrintText(210, 890,
                    "Supplier ID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);

                PrintSmoothText(230, 900,
                    pid.SupplierID,
                    SMOOTH_FONT.XL_P48Wx48H,
                    AUTO_SMOOTH.Enable,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_2);

                if (Convert.ToBoolean(pid.IsVMI))
                    PrintSmoothText(325, 900,
                        "VMI",
                        SMOOTH_FONT.XB_P48Wx48HBold,
                        AUTO_SMOOTH.Enable,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Step 4.11 : Print description
                PrintText(395, 70, "Description" + ":",
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);
                PrintText(420, 75, partMaster.Description,
                                                   NORMAL_FONT.M_NP13Wx20H,
                                                   EXPANDFACTOR.Multiple_2,
                                                   EXPANDFACTOR.Multiple_2);
                #endregion

                #region Step 4.12 : Print Date Received
                PrintText(495, 70, "Date Received:",
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);
                PrintText(520, 75, pid.InboundDate.ToString("dd MMM yyyy"),
                                                    NORMAL_FONT.M_NP13Wx20H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion

                #region Step 4.13 : Print Job No
                if (!string.IsNullOrEmpty(pid.InJobNo))
                {
                    PrintText(575, 70, "Job No:",
                                                        NORMAL_FONT.XS_P17Wx17H,
                                                        EXPANDFACTOR.Multiple_1,
                                                        EXPANDFACTOR.Multiple_1);
                    PrintText(600, 75, pid.InJobNo,
                                                        NORMAL_FONT.M_NP13Wx20H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                #endregion

                #region Step 4.14 : Print ControlCodes 1, Control Codes 2
                // Print Control Code 1
                PrintText(495, 620, controlCode1Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);

                if (!string.IsNullOrEmpty(pid.ControlCode1))
                {
                    PrintText(520, 620, pid.ControlCode1,
                                                        NORMAL_FONT.M_NP13Wx20H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                // Print Control Code 2
                PrintText(575, 620, controlCode2Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);

                if (!string.IsNullOrEmpty(pid.ControlCode2))
                {
                    PrintText(600, 620, pid.ControlCode2,
                                                        NORMAL_FONT.M_NP13Wx20H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                #endregion

                #region Print PID

                //System.Configuration.AppSettingsReader AppRd = new System.Configuration.AppSettingsReader();
                if (appSettings.OwnerCode == "TTKL")
                {
                    PrintText(658, 73, "PID:",
                        NORMAL_FONT.XS_P17Wx17H,
                        EXPANDFACTOR.Multiple_1,
                        EXPANDFACTOR.Multiple_1);
                    PrintText(693, 73,
                        pid.PID.Substring(0, pid.PID.Length - 6),
                        NORMAL_FONT.M_NP13Wx20H,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_2);
                    PrintText(678, 73 + (31 * (pid.PID.Length - 6)),
                        pid.PID.Substring(pid.PID.Length - 6),
                        NORMAL_FONT.M_NP13Wx20H,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_3);

                    PrintReverseImage(648, 63, 550, 100);
                }
                else
                {
                    PrintText(655, 70, "PID:",
                        NORMAL_FONT.XS_P17Wx17H,
                        EXPANDFACTOR.Multiple_1,
                        EXPANDFACTOR.Multiple_1);
                    PrintText(680, 75, pid.PID,
                        NORMAL_FONT.M_NP13Wx20H,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_2);
                }
                #endregion

                #region Step 4.15 Print Customer Name
                PrintText(655, 620, "Customer:",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);
                PrintText(680, 620, pid.CustomerCode,
                                                    NORMAL_FONT.M_NP13Wx20H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);
                #endregion

                #region Print Owner
                if (appSettings.OwnerCode == "TTKL")
                {
                    PrintText(35, 1450, "TTK LOGISTICS (THAILAND) CO., LTD.",
                        NORMAL_FONT.XM_P24Wx24H,
                        EXPANDFACTOR.Multiple_1,
                        EXPANDFACTOR.Multiple_1, true);

                    PrintReverseImage(30, 1388, 735, 30);
                }
                #endregion

                EndPrint();
            }
            else if (type == ILabelFactory.LabelType.BIG)
            {
                StartEnlargePrint();

                // draw the boxes and lines for the skeleton of the label
                PrintBox(20, 80, 3, 3, 1165, 2330);
                PrintLine(410, 345, 3, ORIENTATION.Horizontal, 303);
                PrintLine(590, 935, 3, ORIENTATION.Horizontal, 305);
                //PrintLine(770, 935, 3, ORIENTATION.Horizontal, 95);
                PrintLine(770, 345, 3, ORIENTATION.Horizontal, 95);
                PrintLine(1190, 80, 3, ORIENTATION.Horizontal, 365);
                PrintLine(1190, 650, 3, ORIENTATION.Horizontal, 595);

                PrintLine(20, 245, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 340, 3, ORIENTATION.Vertical, 1170);
                PrintLine(410, 440, 3, ORIENTATION.Vertical, 1940);
                PrintLine(20, 648, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 750, 3, ORIENTATION.Vertical, 1170);
                PrintLine(1190, 844, 3, ORIENTATION.Vertical, 1160);
                PrintLine(20, 935, 3, ORIENTATION.Vertical, 1170);
                PrintLine(20, 1030, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 1125, 3, ORIENTATION.Vertical, 1170);
                #endregion
                // XXX move
                sb.Append(Convert.ToString(Convert.ToChar(ESC)) + "%" + ((int)PRINTROTATION.Rotate90CCW).ToString());


                #region Print QR Code
                // QR Code
                // no need for partial number

                var QRCode = "<S1>" +
                KeepString(pid.PID, 20) + Convert.ToString(DELIMITER) +
                KeepString(pid.ProductCode, 30) + Convert.ToString(DELIMITER) +
                KeepString(partMaster != null ? partMaster.ProductCode2 : "", 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.InboundDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.Qty.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), 25) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlDate.HasValue ? pid.ControlDate.Value.ToString("ddMMyyyy") : "", 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode1, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode2, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode3, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.Version.ToString(), 4) + Convert.ToString(DELIMITER) +
                KeepString(pid.SupplierID, 10) + Convert.ToString(DELIMITER) +
                KeepString(pid.CustomerCode, 10) + Convert.ToString(DELIMITER);

                PrintQRCode(345, 375, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                #region Print Receiver
                // Print Customer Name
                PrintText(2325, 95, "RECEIVER",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2300, 115, "ELECTROLUX",
                                        SMOOTH_FONT.WL_NP28Wx52H,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);

                var customer = await repository.GetCustomerAsync(pid.CustomerCode, pid.WHSCode);
                PrintSmoothText(2300, 175, customer.Name.ToUpper(),
                                        SMOOTH_FONT.WL_NP28Wx52H,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print Advice Notice No.
                // Print ASN No.
                PrintText(2325, 260, "ADVICE NOTE NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2300, 295, inbound.IRNo,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Part No.
                // Print Part No.
                PrintText(2325, 460, "PART NO (P)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2100, 445, pid.ProductCode,
                                        SMOOTH_FONT.XB_P48Wx48HBold,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_2);
                PrintBarCode(2300, 528, 4, 110, "P", pid.ProductCode);
                #endregion

                #region Print Quantity
                // Create decimal format mask

                // Print Quantity
                PrintText(2325, 662, "QUANTITY (Q)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2100, 651, pid.Qty.ToString(DecimalFormat(uomDecimal.DecimalNum)),
                                        SMOOTH_FONT.XB_P48Wx48HBold,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_2);
                PrintBarCode(2300, 729, 4, 110, "Q", pid.Qty.ToString(DecimalFormat(uomDecimal.DecimalNum)));
                #endregion

                #region Print Supplier Code
                // Print Supplier Code
                PrintText(2325, 862, "SUPPLIER CODE (V)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2100, 852, pid.SupplierID,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintBarCode(2300, 911, 4, 110, "V", pid.SupplierID);
                #endregion

                #region Print Serial No
                // Print Serial No
                PrintText(2325, 1045, "SERIAL NO (S)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2100, 1038, GenerateSerialNo(pid.PID),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintBarCode(2300, 1105, 4, 110, "S", GenerateSerialNo(pid.PID));
                #endregion

                #region Print Supplier Name & Address                    
                // Print Supplier Name & Address
                PrintText(1175, 260, "SUPPLIER'S NAME & ADDRESS",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);

                var supplier = await repository.GetSupplierMasterAsync(pid.CustomerCode, pid.SupplierID);

                var address = supplier.CompanyName + "," +
                                        supplier.Suburb;
                PrintText(1145, 281,
                                        ((address.Length > 35) ? address.Substring(0, 35) : address),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);

                address = supplier.CompanyName + "," +
                                        supplier.StreetAddress + " " +
                                        supplier.Suburb + " " +
                                        supplier.PostCode + " " +
                                        supplier.Country;
                PrintText(2325, 1227, address,
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print Gross Weight
                // Create decimal format mask

                // Print Gross Weight
                PrintText(750, 360, "GROSS WEIGHT (KG)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(700, 381, partMaster.GrossWeightTT.ToString(DecimalFormat(uomDecimal.DecimalNum)),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Description
                // Print Description
                PrintText(1175, 665, "DESCRIPTION",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 690,
                                        ((partMaster.Description.Length > 35) ? partMaster.Description.Substring(0, 35) : partMaster.Description.ToString()),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print IM7
                // Print IM7
                PrintText(1175, 765, "IM7 No.",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 800,
                                        ((inbound.RefNo.Length > 20) ? inbound.RefNo.Substring(0, 20) : inbound.RefNo.ToString()),
                                        NORMAL_FONT.XM_P24Wx24H,
                                        EXPANDFACTOR.Multiple_3,
                                        EXPANDFACTOR.Multiple_4);
                #endregion

                #region Print Date
                // Print Date
                PrintText(1175, 950, "DATE",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                if (!string.IsNullOrEmpty(pid.ControlCode2))
                    PrintText(1160, 968, "P" + DateTime.Parse(pid.ControlCode2).ToString("yyMMdd"),
                                            NORMAL_FONT.M_NP13Wx20H,
                                            EXPANDFACTOR.Multiple_2,
                                            EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Inbound Job No.
                // Print Inbound Job No.
                PrintText(1175, 1045, "JOB NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1155, 1068, pid.InJobNo,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Inbound Date.
                // Print Inbound Date.
                PrintText(1175, 1140, "DATE RECEIVED",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 1166, pid.InboundDate.ToString("dd MMM yyyy"),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintText(1175, 1227, "Odette Ver.1 Rev.4",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print Batch No.
                // Print Batch No.
                PrintText(575, 1045, "BATCH NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(555, 1070, pid.ControlCode1,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print PID
                // Print PID
                PrintText(575, 1140, "PID",
                                        NORMAL_FONT.XM_P24Wx24H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(555, 1172, pid.PID,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintReverseImage(586, 1127, 562, 113);
                #endregion


                EndPrint();
            }
            else if (type == ILabelFactory.LabelType.SMALLER)
            {
                StartPrint();
                #region Skeleton
                int intVOffset = 2;
                int intHOffset = 10;
                PrintBox(15 + intVOffset, 50 + intHOffset, 6, 6, 1170, 580);
                PrintLine(190 + intVOffset, 415 + intHOffset, 3, ORIENTATION.Horizontal, 820); // 02
                PrintLine(370 + intVOffset, 50 + intHOffset, 5, ORIENTATION.Horizontal, 1170); // 02
                // desc
                PrintLine(415 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 03
                //date
                PrintLine(475 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 04
                //job
                PrintLine(540 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 05

                PrintLine(15 + intVOffset, 390 + intHOffset, 3, ORIENTATION.Vertical, 360);        // 08
                PrintLine(190 + intVOffset, 870 + intHOffset, 3, ORIENTATION.Vertical, 180);
                PrintLine(420 + intVOffset, 535 + intHOffset, 3, ORIENTATION.Vertical, 180);   // 08
                #endregion

                #region QR Code
                var QRCode = "<S1>" +
                KeepString(pid.PID, 20) + Convert.ToString(DELIMITER) +
                KeepString(pid.ProductCode, 30) + Convert.ToString(DELIMITER) +
                KeepString(partMaster != null ? partMaster.ProductCode2 : "", 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.InboundDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.Qty.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), 25) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlDate.HasValue ? pid.ControlDate.Value.ToString("ddMMyyyy") : "", 8) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode1, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode2, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.ControlCode3, 30) + Convert.ToString(DELIMITER) +
                KeepString(pid.Version.ToString(), 4) + Convert.ToString(DELIMITER) +
                KeepString(pid.SupplierID, 10) + Convert.ToString(DELIMITER) +
                KeepString(pid.CustomerCode, 10) + Convert.ToString(DELIMITER);

                PrintQRCode(50, 100, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                PrintText(35, 420, productCode1Name + ":", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);

                #region ProductCode
                if (pid.ProductCode.Length < 15)
                {
                    PrintSmoothText(60, 420,
                            pid.ProductCode,
                            SMOOTH_FONT.XB_P48Wx48HBold,
                            AUTO_SMOOTH.Enable,
                            EXPANDFACTOR.Multiple_2,
                            EXPANDFACTOR.Multiple_3);

                }
                else
                {
                    PrintSmoothText(60, 420,
                            pid.ProductCode,
                            SMOOTH_FONT.XB_P48Wx48HBold,
                            AUTO_SMOOTH.Enable,
                            EXPANDFACTOR.Multiple_2,
                            EXPANDFACTOR.Multiple_2);
                }
                #endregion

                #region QTY
                PrintText(210, 420, "Quantity (" + uom.Name + "s): " + (pid.ParentID != null ? "[SPLIT]" : ""),
                                                       NORMAL_FONT.XS_P17Wx17H,
                                                       EXPANDFACTOR.Multiple_1,
                                                       EXPANDFACTOR.Multiple_1);
                PrintSmoothText(240, 420, pid.Qty.ToString(DecimalFormat(uomDecimal.DecimalNum)),
                                                             SMOOTH_FONT.XB_P48Wx48HBold,
                                                             AUTO_SMOOTH.Enable,
                                                             EXPANDFACTOR.Multiple_2,
                                                             EXPANDFACTOR.Multiple_3);
                #endregion

                #region  Version
                // Print version
                PrintText(350, 100,
                    "Version: " + pid.Version,
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                #endregion

                #region  Step 4.17   Supplier ID
                // Print Supplier ID
                PrintText(210, 890,
                    "Supplier ID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);

                PrintSmoothText(230, 900,
                    pid.SupplierID,
                    SMOOTH_FONT.XL_P48Wx48H,
                    AUTO_SMOOTH.Enable,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_2);

                if (Convert.ToBoolean(pid.IsVMI))
                    PrintSmoothText(325, 900,
                        "VMI",
                        SMOOTH_FONT.XB_P48Wx48HBold,
                        AUTO_SMOOTH.Enable,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print description
                /* test bielany 415 */
                PrintText(380, 75, partMaster.Description,
                                                    NORMAL_FONT.S_NP8Wx15H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion

                #region Print Date Received
                PrintText(425, 70, "Date Received:",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);
                PrintText(445, 75, pid.InboundDate.ToString("dd MMM yyyy"),
                                                    NORMAL_FONT.S_NP8Wx15H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion

                #region Print Job No
                if (!string.IsNullOrEmpty(pid.InJobNo))
                {
                    PrintText(485, 70, "Job No:",
                                                        NORMAL_FONT.XS_P17Wx17H,
                                                        EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);
                    PrintText(505, 75, pid.InJobNo,
                                                        NORMAL_FONT.S_NP8Wx15H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                #endregion

                #region Print ControlCodes 1, Control Codes 2
                // Print Control Code 1
                PrintText(425, 620, controlCode1Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);

                if (!string.IsNullOrEmpty(pid.ControlCode1))
                {
                    PrintText(445, 620, pid.ControlCode1,
                                                        NORMAL_FONT.S_NP8Wx15H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                // Print Control Code 2
                PrintText(485, 620, controlCode2Name + ": ", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);

                if (!string.IsNullOrEmpty(pid.ControlCode2))
                {
                    PrintText(505, 620, pid.ControlCode2,
                                                        NORMAL_FONT.S_NP8Wx15H,
                                                        EXPANDFACTOR.Multiple_2,
                                                        EXPANDFACTOR.Multiple_2);
                }

                #endregion

                #region Print PID   

                if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                {
                    PrintText(545, 70, "PID:",
                        NORMAL_FONT.XS_P17Wx17H,
                        EXPANDFACTOR.Multiple_1,
                        EXPANDFACTOR.Multiple_1);
                    PrintText(565, 75, pid.PID,
                        NORMAL_FONT.S_NP8Wx15H,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_2);
                }
                else
                {
                    PrintText(658, 73, "PID:",
                        NORMAL_FONT.XS_P17Wx17H,
                        EXPANDFACTOR.Multiple_1,
                        EXPANDFACTOR.Multiple_1);
                    PrintText(693, 73,
                        pid.PID.Substring(0, pid.PID.Length - 6),
                        NORMAL_FONT.S_NP8Wx15H,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_2);
                    PrintText(678, 73 + (31 * (pid.PID.Length - 6)), pid.PID.ToString().Substring(pid.PID.Length - 6), NORMAL_FONT.S_NP8Wx15H, EXPANDFACTOR.Multiple_2, EXPANDFACTOR.Multiple_3);
                }

                #endregion

                #region Step 4.15 Print Customer Name
                PrintText(545, 620, "Customer:", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);
                PrintText(565, 620, pid.CustomerCode, NORMAL_FONT.S_NP8Wx15H, EXPANDFACTOR.Multiple_2, EXPANDFACTOR.Multiple_2);
                #endregion

                #region Print Owner
                if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                {
                }
                else
                {
                    PrintText(35, 1450, "TTK LOGISTICS (THAILAND) CO., LTD.", NORMAL_FONT.XM_P24Wx24H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1, true);

                    PrintReverseImage(30, 1388, 735, 30);
                }

                #endregion
                EndPrint();
            }

        }

        public async override Task AddLabel(StorageDetailGroup group, ILabelFactory.LabelType type)
        {

            var inventoryControl = await repository.GetInventoryControlAsync("GPD");
            var productCode1Name = await LabelOrDefault(inventoryControl.PC1Type, "Product Code 1");
            var productCode2Name = await LabelOrDefault(inventoryControl.PC2Type, "Product Code 2");
            var controlCode1Name = await LabelOrDefault(inventoryControl.CC1Type, "Control Code 1");
            var controlCode2Name = await LabelOrDefault(inventoryControl.CC2Type, "Control Code 1");




            if (type == ILabelFactory.LabelType.SMALL)
            {
                StartPrint();
                int intVOffset = 2;
                int intHOffset = 10;
                PrintBox(15 + intVOffset, 50 + intHOffset, 6, 6, 1170, 735);
                PrintLine(190 + intVOffset, 400 + intHOffset, 3, ORIENTATION.Horizontal, 820); // 02
                PrintLine(380 + intVOffset, 50 + intHOffset, 5, ORIENTATION.Horizontal, 1170); // 02
                PrintLine(480 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 03
                PrintLine(560 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 04
                PrintLine(640 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 05

                PrintLine(15 + intVOffset, 400 + intHOffset, 3, ORIENTATION.Vertical, 370);        // 08
                PrintLine(190 + intVOffset, 870 + intHOffset, 3, ORIENTATION.Vertical, 190);
                PrintLine(480 + intVOffset, 600 + intHOffset, 3, ORIENTATION.Vertical, 265);   // 08

                #region Step 4.8 : Print QR Code
                #region QR Code
                var QRCode = "<SG>" +
                KeepString(group.GroupID, 20) + Convert.ToString(DELIMITER) +
                KeepString(group.Name, 30) + Convert.ToString(DELIMITER) +
                KeepString("", 30) + Convert.ToString(DELIMITER) +
                KeepString(group.CreatedDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER);


                PrintQRCode(50, 100, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                #region Step 4.9 : Print Product Code
                //Print Product Code
                PrintText(35, 420, productCode1Name + ":",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);



                PrintSmoothText(60, 420,
                    group.Name,
                    SMOOTH_FONT.XB_P48Wx48HBold,
                    AUTO_SMOOTH.Enable,
                    EXPANDFACTOR.Multiple_2,
                    EXPANDFACTOR.Multiple_3);



                #endregion

                #region Step 4.10 : Print Quantity
                // Print quantity
                PrintText(210, 420, "Quantity : ",
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);



                // Create decimal format mask

                PrintSmoothText(240, 420, "1",
                                                         SMOOTH_FONT.XB_P48Wx48HBold,
                                                         AUTO_SMOOTH.Enable,
                                                         EXPANDFACTOR.Multiple_2,
                                                         EXPANDFACTOR.Multiple_3);

                #endregion

                #region  Step 4.16  Print Version
                // Print version
                PrintText(350, 100,
                    "Version: " + "1",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                #endregion

                #region  Step 4.16  Print LABEL COUNT
                // Print version

                #endregion

                #region  Step 4.17   Supplier ID
                // Print Supplier ID
                PrintText(210, 890,
                    "Supplier ID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);

                PrintSmoothText(230, 900,
                    "TTESA",
                    SMOOTH_FONT.XL_P48Wx48H,
                    AUTO_SMOOTH.Enable,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_2);


                #endregion

                #region Step 4.11 : Print description
                PrintText(395, 70, "Description" + ":",
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);
                PrintText(420, 75, "PID Group",
                                                   NORMAL_FONT.M_NP13Wx20H,
                                                   EXPANDFACTOR.Multiple_2,
                                                   EXPANDFACTOR.Multiple_2);
                #endregion

                #region Step 4.12 : Print Date Received
                PrintText(495, 70, "Date Created:",
                                                   NORMAL_FONT.XS_P17Wx17H,
                                                   EXPANDFACTOR.Multiple_1,
                                                   EXPANDFACTOR.Multiple_1);
                PrintText(520, 75, group.CreatedDate.ToString("dd MMM yyyy"),
                                                    NORMAL_FONT.M_NP13Wx20H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion



                #endregion

                #region Step 4.14 : Print ControlCodes 1, Control Codes 2
                // Print Control Code 1
                PrintText(495, 620, controlCode1Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);



                // Print Control Code 2
                PrintText(575, 620, controlCode2Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);



                #endregion

                #region Print PID

                //System.Configuration.AppSettingsReader AppRd = new System.Configuration.AppSettingsReader();

                PrintText(655, 70, "GID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                PrintText(680, 75, group.GroupID,
                    NORMAL_FONT.M_NP13Wx20H,
                    EXPANDFACTOR.Multiple_2,
                    EXPANDFACTOR.Multiple_2);

                #endregion

                #region Step 4.15 Print Customer Name
                PrintText(655, 620, "Customer:",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);
                PrintText(680, 620, "GPD",
                                                    NORMAL_FONT.M_NP13Wx20H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);
                #endregion

                #region Print Owner

                #endregion

                EndPrint();
            }
            else if (type == ILabelFactory.LabelType.BIG)
            {
                StartEnlargePrint();

                // draw the boxes and lines for the skeleton of the label
                PrintBox(20, 80, 3, 3, 1165, 2330);
                PrintLine(410, 345, 3, ORIENTATION.Horizontal, 303);
                PrintLine(590, 935, 3, ORIENTATION.Horizontal, 305);
                //PrintLine(770, 935, 3, ORIENTATION.Horizontal, 95);
                PrintLine(770, 345, 3, ORIENTATION.Horizontal, 95);
                PrintLine(1190, 80, 3, ORIENTATION.Horizontal, 365);
                PrintLine(1190, 650, 3, ORIENTATION.Horizontal, 595);

                PrintLine(20, 245, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 340, 3, ORIENTATION.Vertical, 1170);
                PrintLine(410, 440, 3, ORIENTATION.Vertical, 1940);
                PrintLine(20, 648, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 750, 3, ORIENTATION.Vertical, 1170);
                PrintLine(1190, 844, 3, ORIENTATION.Vertical, 1160);
                PrintLine(20, 935, 3, ORIENTATION.Vertical, 1170);
                PrintLine(20, 1030, 3, ORIENTATION.Vertical, 2330);
                PrintLine(20, 1125, 3, ORIENTATION.Vertical, 1170);

                // XXX move
                sb.Append(Convert.ToString(Convert.ToChar(ESC)) + "%" + ((int)PRINTROTATION.Rotate90CCW).ToString());


                #region Print QR Code
                // QR Code
                // no need for partial number

                var QRCode = "<SG>" +
                KeepString(group.GroupID, 20) + Convert.ToString(DELIMITER) +
                KeepString(group.Name, 30) + Convert.ToString(DELIMITER) +
                KeepString("", 30) + Convert.ToString(DELIMITER) +
                KeepString(group.CreatedDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER);

                PrintQRCode(345, 375, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                #region Print Receiver
                // Print Customer Name
                PrintText(2325, 95, "RECEIVER",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2300, 115, "ELECTROLUX",
                                        SMOOTH_FONT.WL_NP28Wx52H,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);


                PrintSmoothText(2300, 175, "GPD",
                                        SMOOTH_FONT.WL_NP28Wx52H,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print Advice Notice No.
                // Print ASN No.
                PrintText(2325, 260, "ADVICE NOTE NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2300, 295, "",
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Part No.
                // Print Part No.
                PrintText(2325, 460, "PART NO (P)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2100, 445, "GROUP",
                                        SMOOTH_FONT.XB_P48Wx48HBold,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_2);
                PrintBarCode(2300, 528, 4, 110, "P", "GROUP");
                #endregion

                #region Print Quantity
                // Create decimal format mask

                // Print Quantity
                PrintText(2325, 662, "QUANTITY (Q)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintSmoothText(2100, 651, "1",
                                        SMOOTH_FONT.XB_P48Wx48HBold,
                                        AUTO_SMOOTH.Enable,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_2);
                PrintBarCode(2300, 729, 4, 110, "Q", "1");
                #endregion

                #region Print Supplier Code
                // Print Supplier Code
                PrintText(2325, 862, "SUPPLIER CODE (V)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2100, 852, "TTESA",
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintBarCode(2300, 911, 4, 110, "V", "TTESA");
                #endregion

                #region Print Serial No
                // Print Serial No
                PrintText(2325, 1045, "SERIAL NO (S)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(2100, 1038, GenerateSerialNo(group.GroupID),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintBarCode(2300, 1105, 4, 110, "S", GenerateSerialNo(group.GroupID));
                #endregion

                #region Print Supplier Name & Address                    
                // Print Supplier Name & Address
                PrintText(1175, 260, "SUPPLIER'S NAME & ADDRESS",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);




                PrintText(1145, 281, "",
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);


                PrintText(2325, 1227, "",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                #endregion

                #region Print Gross Weight
                // Create decimal format mask

                // Print Gross Weight
                PrintText(750, 360, "GROSS WEIGHT (KG)",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(700, 381, "-",
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print Description
                // Print Description
                PrintText(1175, 665, "DESCRIPTION",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 690,
                                        "PID Group",
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                #endregion

                #region Print IM7
                // Print IM7
                PrintText(1175, 765, "IM7 No.",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 800,
                                        "",
                                        NORMAL_FONT.XM_P24Wx24H,
                                        EXPANDFACTOR.Multiple_3,
                                        EXPANDFACTOR.Multiple_4);
                #endregion

                #region Print Date
                // Print Date
                PrintText(1175, 950, "DATE",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);

                #endregion

                #region Print Inbound Job No.
                // Print Inbound Job No.
                PrintText(1175, 1045, "JOB NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);

                #endregion

                #region Print Inbound Date.
                // Print Inbound Date.
                PrintText(1175, 1140, "DATE CREATED",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(1150, 1166, group.CreatedDate.ToString("dd MMM yyyy"),
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);

                #endregion

                #region Print Batch No.
                // Print Batch No.
                PrintText(575, 1045, "BATCH NO",
                                        NORMAL_FONT.XS_P17Wx17H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);

                #endregion

                #region Print PID
                // Print PID
                PrintText(575, 1140, "PID",
                                        NORMAL_FONT.XM_P24Wx24H,
                                        EXPANDFACTOR.Multiple_1,
                                        EXPANDFACTOR.Multiple_1);
                PrintText(555, 1172, group.GroupID,
                                        NORMAL_FONT.M_NP13Wx20H,
                                        EXPANDFACTOR.Multiple_2,
                                        EXPANDFACTOR.Multiple_3);
                PrintReverseImage(586, 1127, 562, 113);
                #endregion


                EndPrint();
            }
            else if (type == ILabelFactory.LabelType.SMALLER)
            {
                StartPrint();
                #region Skeleton
                int intVOffset = 2;
                int intHOffset = 10;
                PrintBox(15 + intVOffset, 50 + intHOffset, 6, 6, 1170, 580);
                PrintLine(190 + intVOffset, 415 + intHOffset, 3, ORIENTATION.Horizontal, 820); // 02
                PrintLine(370 + intVOffset, 50 + intHOffset, 5, ORIENTATION.Horizontal, 1170); // 02
                // desc
                PrintLine(415 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 03
                //date
                PrintLine(475 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 04
                //job
                PrintLine(540 + intVOffset, 50 + intHOffset, 3, ORIENTATION.Horizontal, 1170); // 05

                PrintLine(15 + intVOffset, 390 + intHOffset, 3, ORIENTATION.Vertical, 360);        // 08
                PrintLine(190 + intVOffset, 870 + intHOffset, 3, ORIENTATION.Vertical, 180);
                PrintLine(420 + intVOffset, 535 + intHOffset, 3, ORIENTATION.Vertical, 180);   // 08
                #endregion

                #region QR Code
                var QRCode = "<SG>" +
                KeepString(group.GroupID, 20) + Convert.ToString(DELIMITER) +
                KeepString(group.Name, 30) + Convert.ToString(DELIMITER) +
                KeepString("", 30) + Convert.ToString(DELIMITER) +
                KeepString(group.CreatedDate.ToString("ddMMyyyy"), 8) + Convert.ToString(DELIMITER);

                PrintQRCode(50, 100, 4, QRCode, QRCode.Length, printer.Type);
                #endregion

                PrintText(35, 420, productCode1Name + ":", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);

                #region ProductCode

                PrintSmoothText(60, 420,
                        group.GroupID,
                        SMOOTH_FONT.XB_P48Wx48HBold,
                        AUTO_SMOOTH.Enable,
                        EXPANDFACTOR.Multiple_2,
                        EXPANDFACTOR.Multiple_3);



                #endregion

                #region QTY
                PrintText(210, 420, "Quantity: ",
                                                       NORMAL_FONT.XS_P17Wx17H,
                                                       EXPANDFACTOR.Multiple_1,
                                                       EXPANDFACTOR.Multiple_1);
                PrintSmoothText(240, 420, "1",
                                                             SMOOTH_FONT.XB_P48Wx48HBold,
                                                             AUTO_SMOOTH.Enable,
                                                             EXPANDFACTOR.Multiple_2,
                                                             EXPANDFACTOR.Multiple_3);
                #endregion

                #region  Version
                // Print version
                PrintText(350, 100,
                    "Version: " + "1",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                #endregion

                #region  Step 4.17   Supplier ID
                // Print Supplier ID
                PrintText(210, 890,
                    "Supplier ID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);

                PrintSmoothText(230, 900,
                    "TTESA",
                    SMOOTH_FONT.XL_P48Wx48H,
                    AUTO_SMOOTH.Enable,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_2);


                #endregion

                #region Print description
                /* test bielany 415 */
                PrintText(380, 75, "PID Group",
                                                    NORMAL_FONT.S_NP8Wx15H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion

                #region Print Date Received
                PrintText(425, 70, "Date Created:",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);
                PrintText(445, 75, group.CreatedDate.ToString("dd MMM yyyy"),
                                                    NORMAL_FONT.S_NP8Wx15H,
                                                    EXPANDFACTOR.Multiple_2,
                                                    EXPANDFACTOR.Multiple_2);

                #endregion





                #region Print ControlCodes 1, Control Codes 2
                // Print Control Code 1
                PrintText(425, 620, controlCode1Name + ": ",
                                                    NORMAL_FONT.XS_P17Wx17H,
                                                    EXPANDFACTOR.Multiple_1,
                                                    EXPANDFACTOR.Multiple_1);



                // Print Control Code 2
                PrintText(485, 620, controlCode2Name + ": ", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);



                #endregion




                PrintText(545, 70, "GID:",
                    NORMAL_FONT.XS_P17Wx17H,
                    EXPANDFACTOR.Multiple_1,
                    EXPANDFACTOR.Multiple_1);
                PrintText(565, 75, group.GroupID,
                    NORMAL_FONT.S_NP8Wx15H,
                    EXPANDFACTOR.Multiple_2,
                    EXPANDFACTOR.Multiple_2);





                #region Step 4.15 Print Customer Name
                PrintText(545, 620, "Customer:", NORMAL_FONT.XS_P17Wx17H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1);
                PrintText(565, 620, "GPD", NORMAL_FONT.S_NP8Wx15H, EXPANDFACTOR.Multiple_2, EXPANDFACTOR.Multiple_2);
                #endregion

                #region Print Owner
                if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                {
                }
                else
                {
                    PrintText(35, 1450, "TTK LOGISTICS (THAILAND) CO., LTD.", NORMAL_FONT.XM_P24Wx24H, EXPANDFACTOR.Multiple_1, EXPANDFACTOR.Multiple_1, true);

                    PrintReverseImage(30, 1388, 735, 30);
                }

                #endregion
                EndPrint();
            }

        }

        public override Task AddLabel(Location location)
        {
            StartPrint();
            PrintBox(10, 65, 6, 6, 1160, 745);
            PrintLine(360, 65, 5, ORIENTATION.Horizontal, 1160);

            if(location.Code.Length >= 8)
                PrintSmoothText(40, 130, location.Code.Trim(),
                    SMOOTH_FONT.XB_P48Wx48HBold, AUTO_SMOOTH.Enable, EXPANDFACTOR.Multiple_3, EXPANDFACTOR.Multiple_7);
            else
                PrintSmoothText(40, 80, location.Code.Trim(),
                    SMOOTH_FONT.XB_P48Wx48HBold, AUTO_SMOOTH.Enable, EXPANDFACTOR.Multiple_5, EXPANDFACTOR.Multiple_7);

            var locTypeInt = location.Type switch
            {
                LocationType.Normal => 0,
                LocationType.Quarantine => 1,
                LocationType.CrossDock => 2,
                LocationType.Standby => 3,
                _ => 0,
            };

            var QRCode = "<L1>" +
            KeepString(location.Code, 15) + Convert.ToString(DELIMITER) +
            KeepString(location.WHSCode, 7) + Convert.ToString(DELIMITER) +
            KeepString(locTypeInt.ToString(), 1) + Convert.ToString(DELIMITER);

            PrintQRCode(400, 850, 10, QRCode, QRCode.Length, printer.Type);

            var locTypeString = location.Type switch
            {
                LocationType.Normal => "LOCATION",
                LocationType.Quarantine => "QUARANTINE",
                LocationType.CrossDock => "CROSSDOCK",
                LocationType.Standby => "STANDBY",
                _ => "",
            };

            PrintSmoothText(400, 130, locTypeString, SMOOTH_FONT.XB_P48Wx48HBold, AUTO_SMOOTH.Enable, EXPANDFACTOR.Multiple_2, EXPANDFACTOR.Multiple_2);
            PrintSmoothText(500, 130, "WHS: ", SMOOTH_FONT.XB_P48Wx48HBold, AUTO_SMOOTH.Enable, EXPANDFACTOR.Multiple_2, EXPANDFACTOR.Multiple_2);
            PrintSmoothTextWithoutHV(location.WHSCode, SMOOTH_FONT.XB_P48Wx48HBold, AUTO_SMOOTH.Enable, EXPANDFACTOR.Multiple_3, EXPANDFACTOR.Multiple_4);

            EndPrint();
            return Task.CompletedTask;
        }

        #region LoadInventoryControl
        public string GenerateSerialNo(string p_strPID)
        {
            string l_strSerialNo;
            string l_strMonth = p_strPID.Substring(p_strPID.Length - 8, 2);
            string l_strRunningNo = p_strPID.Substring(p_strPID.Length - 6);

            l_strSerialNo = (int.Parse(l_strMonth) + 12).ToString() + Convert.ToInt32(l_strRunningNo, 16).ToString("0000000");
            return l_strSerialNo;
        }
        #endregion


    }
}