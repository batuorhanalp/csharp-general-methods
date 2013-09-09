using System.Collections.Specialized;
using Encryption;
using System;
using System.Web;
using System.Collections.Generic;
using System.Web.Security;

namespace Cookies
{
    public class Cookie
    {
        public static string strUserId = "-";
        public static string strAccessToken = "-";

        //Giriş yapan hemendene kullanıcılarına ait bilgiler kriptolu olarak client'ın bilgisayarına bırakılıyor.
        public static bool SetCookie(string cookieName, string password, string info1, string info2, string info3, DateTime expireDate)
        {
            try
            {
                string strEncryptedInfo1 = Crypto.Encrypt(password, info1);
                string strEncryptedInfo2 = Crypto.Encrypt(password, info2);
                string strEncryptedInfo3 = Crypto.Encrypt(password, info3);

                var httpCookie = HttpContext.Current.Response.Cookies[cookieName];
                if (httpCookie != null)
                {
                    httpCookie["i1"] = strEncryptedInfo1;
                    httpCookie["i2"] = strEncryptedInfo2;
                    httpCookie["i3"] = strEncryptedInfo3;
                    httpCookie["expire"] = expireDate.ToString();
                    httpCookie.Expires = expireDate;
                    httpCookie.HttpOnly = true;
                }
                else
                {
                    HttpCookie aCookie = new HttpCookie(cookieName);
                    aCookie.Values["i1"] = strEncryptedInfo1;
                    aCookie.Values["i2"] = strEncryptedInfo2;
                    aCookie.Values["i3"] = strEncryptedInfo3;
                    aCookie.Values["expire"] = expireDate.ToString();
                    aCookie.Expires = DateTime.MinValue;
                    aCookie.HttpOnly = true;

                    HttpContext.Current.Response.Cookies.Add(aCookie);

                    fixCookieExpireDate(cookieName, expireDate);
                }
            }
            catch (Exception err)
            {
                string strErrorInnerMessage = "-", strErrorSource = "-", strErrorMessage = "-";
                if (err.InnerException != null)
                {
                    strErrorInnerMessage = err.InnerException.Message;
                }
                if (err.Source != null)
                {
                    strErrorSource = err.Source;
                }
                if (err.Message != null)
                {
                    strErrorMessage = err.Message;
                }
                return false;
            }
            return true;
        }

        //Cookie'den bilgileri getirir
        public static Array GetCookie(string cookieName, string password)
        {
            var arrReturn = new List<string>();
            try
            {
                var cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie != null)
                {
                    NameValueCollection UserInfoCookieCollection = cookie.Values;
                    DateTime dtExpireDate = Convert.ToDateTime(HttpContext.Current.Server.HtmlEncode(UserInfoCookieCollection["expire"]));
                    if (dtExpireDate > DateTime.Now.AddHours(1))
                    {
                        arrReturn.Add(Crypto.Decrypt(password, HttpContext.Current.Server.HtmlEncode(UserInfoCookieCollection["i1"])));
                        arrReturn.Add(Crypto.Decrypt(password, HttpContext.Current.Server.HtmlEncode(UserInfoCookieCollection["i2"])));
                        arrReturn.Add(Crypto.Decrypt(password, HttpContext.Current.Server.HtmlEncode(UserInfoCookieCollection["i3"])));
                    }
                }
            }
            catch (Exception err)
            {
                string strErrorSource = "-", strErrorMessage = "-";
                if (err.InnerException != null)
                {
                }
                if (err.Source != null)
                {
                    strErrorSource = err.Source;
                }
                if (err.Message != null)
                {
                    strErrorMessage = err.Message;
                }
            }
            return arrReturn.ToArray();
        }

        public static void setTicket(string ticketName, DateTime expireDate, bool isPersistant, string data)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, ticketName, DateTime.Now, expireDate, isPersistant, data, FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

            cookie.Expires = ticket.Expiration;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        //Çıkış yapmak için client'ın bilgisayarındaki tüm cookielerin geçerlilik tarihi geçmiş bir tarihe atanıyor.
        public static bool RemoveCookie(string cookieName)
        {
            try
            {
                var aCookie = new HttpCookie(cookieName) {Expires = DateTime.Now.AddDays(-1)};
                HttpContext.Current.Response.Cookies.Add(aCookie);
            }
            catch (Exception err)
            {
                string strErrorInnerMessage = "-", strErrorSource = "-", strErrorMessage = "-";
                if (err.InnerException != null)
                {
                    strErrorInnerMessage = err.InnerException.Message;
                }
                if (err.Source != null)
                {
                    strErrorSource = err.Source;
                }
                if (err.Message != null)
                {
                    strErrorMessage = err.Message;
                }
                return false;
            }

            return true;
        }

        private static void fixCookieExpireDate (string cookieName, DateTime expireDate)
        {
                var httpCookie = HttpContext.Current.Response.Cookies[cookieName];
                httpCookie.Expires = expireDate;
                HttpContext.Current.Response.Cookies.Add(httpCookie);
        }
    }
}
