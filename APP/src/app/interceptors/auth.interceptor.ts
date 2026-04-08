import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { from, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  let accessToken = auth.getAccessToken();

  let authReq = req;

  if (accessToken) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`
      }
    });
  }

  return next(authReq).pipe(
    catchError(error => {
      if (error.status === 401) {
        return from(auth.refresh()).pipe(
          switchMap(newToken => {
            if (!newToken) {
              auth.logout();
              return throwError(() => error);
            }

            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
            });

            return next(retryReq);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
