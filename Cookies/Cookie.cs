using System.Collections.Specialized;
using Encryption;
using System;
using System.Web;
using System.Collections.Generic;
using System.Web.Security;

namespace Cookies
{
    public class CookieObject
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
    public class Cookie
    {
        //Giriş yapan kullanıcılara ait bilgiler kriptolu olarak client'ın bilgisayarına bırakılıyor.
        public static bool SetCookie(string cookieName, string password, List<CookieObject> parameters, DateTime expireDate)
        {
            try
            {
                var liParameters = new List<CookieObject>();
                foreach (var param in parameters)
                {
                    var encData = Crypto.Encrypt(password, param.Data);
                    var newParameter = new CookieObject
                    {
                        Name = param.Name,
                        Data = encData
                    };
                    liParameters.Add(newParameter);
                }

                var httpCookie = HttpContext.Current.Response.Cookies[cookieName];
                if (httpCookie != null)
                {
                    foreach (var parameter in liParameters)
                    {
                        httpCookie[parameter.Name] = parameter.Data;
                    }
                    httpCookie["expire"] = expireDate.ToString();
                    httpCookie.Expires = expireDate;
                    httpCookie.HttpOnly = true;
                }
                else
                {
                    var aCookie = new HttpCookie(cookieName);

                    foreach (var parameter in liParameters)
                    {
                        aCookie.Values[parameter.Name] = parameter.Data;
                    }
                    aCookie.Values["expire"] = expireDate.ToString();
                    aCookie.Expires = DateTime.MinValue;
                    aCookie.HttpOnly = true;

                    HttpContext.Current.Response.Cookies.Add(aCookie);

                    FixCookieExpireDate(cookieName, expireDate);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        //Cookie'den bilgileri getirir
        public static List<CookieObject> GetCookie(string cookieName, string password)
        {
            var liReturn = new List<CookieObject>();
            try
            {
                var cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie != null)
                {
                    var userInfoCookieCollection = cookie.Values;
                    var dtExpireDate = Convert.ToDateTime(HttpContext.Current.Server.HtmlEncode(userInfoCookieCollection["expire"]));
                    if (dtExpireDate > DateTime.Now.AddHours(1))
                    {
                        foreach (var par in userInfoCookieCollection)
                        {
                            var data = Crypto.Decrypt(password, HttpContext.Current.Server.HtmlEncode(userInfoCookieCollection[par.ToString()]));
                            var newData = new CookieObject
                            {
                                Name = par.ToString(),
                                Data = data
                            };
                            liReturn.Add(newData);
                        }
                    }
                }
            }
            catch
            {

            }
            return liReturn;
        }

        public static void SetTicket(string ticketName, DateTime expireDate, bool isPersistant, string data)
        {
            var ticket = new FormsAuthenticationTicket(1, ticketName, DateTime.Now, expireDate, isPersistant, data, FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = ticket.Expiration };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        //Çıkış yapmak için client'ın bilgisayarındaki tüm cookielerin geçerlilik tarihi geçmiş bir tarihe atanıyor.
        public static bool RemoveCookie(string cookieName)
        {
            try
            {
                var aCookie = new HttpCookie(cookieName) { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Current.Response.Cookies.Add(aCookie);
            }
            catch (Exception err)
            {
                return false;
            }

            return true;
        }

        private static void FixCookieExpireDate(string cookieName, DateTime expireDate)
        {
            var httpCookie = HttpContext.Current.Response.Cookies[cookieName];
            if (httpCookie != null)
            {
                httpCookie.Expires = expireDate;
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }
        }
    }
}
