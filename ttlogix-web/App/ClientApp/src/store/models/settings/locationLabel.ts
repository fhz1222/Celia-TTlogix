export class LocationLabel {
    codes: Array<CodeCombo>;
    printer: string;
    copies: number;
}

export class CodeCombo {
    code: string;
    whsCode: string;
}

export class LocationQrCode {
    code: string;
    name: string;
}

export class PdfLocationLabel {
    code: string;
    whsCode: string;
    qrCode: string;
}