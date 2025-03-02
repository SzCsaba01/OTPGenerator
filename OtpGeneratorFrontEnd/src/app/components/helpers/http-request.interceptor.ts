import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";

export const HttpRequestInterceptor : HttpInterceptorFn = (req : HttpRequest<unknown>, next: HttpHandlerFn) => {
    req = req.clone({
        withCredentials: true
    });

    return next(req);
}