import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // שליפת הטוקן לפי המפתח המדויק מה-AuthService שלך
  const token = localStorage.getItem('auth_token');

  if (token) {
    // שכפול הבקשה והוספת ה-Bearer Token
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }

  return next(req);
};