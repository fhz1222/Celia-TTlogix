export class Error {
    detail: string;
    errors: {[key: string]: Array<string>};
    status: number;
    title: string;
    traceId: string;
    type: string;
}

export class ApiError {
    detail: string;
    status: number;
    title: string;
    traceId: string;
}