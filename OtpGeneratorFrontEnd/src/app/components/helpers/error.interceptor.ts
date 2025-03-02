import { inject } from '@angular/core';
import {
  HttpRequest,
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpHandlerFn,
  HttpStatusCode,
  HttpResponse,
  HttpEvent,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { ToastService } from '../services/toast.service';

export const ErrorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const toastService = inject(ToastService);
  return next(req).pipe(
    tap((event: any) => {
      if (
        event.status == HttpStatusCode.Ok &&
        event.body &&
        event.body.message
      ) {
        toastService.showMessage(event.body.message, 'success');
      }
    }),
    catchError((error: HttpErrorResponse) => {
      let errorMessage = {} as { message: string };
        try {
          const errorMessageObject =
            error.error instanceof Object
              ? error.error
              : JSON.parse(error.error);
          if (
            errorMessageObject.message != null &&
            errorMessageObject.message != '' &&
            errorMessageObject.message != undefined &&
            errorMessageObject.message.length > 0
          ) {
            errorMessage.message = errorMessageObject.message;
          } else {
            errorMessage.message = error.statusText;
          }
        } catch {
          errorMessage.message = error.statusText;
        }
        toastService.showMessage(errorMessage.message, 'error');
      return throwError(() => error);
    })
  );
};
