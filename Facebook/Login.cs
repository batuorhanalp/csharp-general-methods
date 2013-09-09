using System;

namespace Facebook
{
    public class Login
    {
        ////Global Facebook değişkenleri
        ///* HemenDene için App Id, Key Code, Return Url */
        //public const string clientId = "263604490332689";
        //public const string redirectUrl = "http://www.dandik.co/giris";
        //public const string clientSecret = "607872b0247c7118efd4e07cf7850179";
        //public const string scope = "email,user_birthday,status_update,user_location";
        //public const string strRedirectUrl = "https://graph.facebook.com/oauth/authorize?client_id=263604490332689&redirect_uri=http://www.hemendene.com/giris&scope=email,user_birthday,status_update,user_location";
        ///* HemenDene için App Id, Key Code, Return Url Bitti*/

        public static UserInfo UserLogin(string accessToken, string id, string name, string firstName, string lastName, string link, string birthday, string gender, string email, string verified, string photo, string location)
        {
            try
            {
                var dtBirthdate = Convert.ToDateTime("01.01.1800");
                var iGender = 1;

                if (gender != "")
                    {
                        if (gender == "female")
                        {
                            iGender = 2;
                        }
                        else if (gender == "male")
                        {
                            iGender = 3;
                        }
                    }
                try
                    {
                        string[] sb = birthday.Split('/');
                        dtBirthdate = Convert.ToDateTime(sb[1] + "/" + sb[0] + "/" + sb[2]);
                    }
                    catch (Exception)
                    { }

                var user = new UserInfo
                    {
                        FacebookId = Convert.ToDecimal(id),
                        FacebookName = name,
                        FirstName = firstName,
                        LastName = lastName,
                        Birthdate = dtBirthdate,
                        Email = email,
                        PictureUrl = photo,
                        Verified = Convert.ToBoolean(verified),
                        Gender = iGender,
                        FacebookUrl = link,
                        Location = Convert.ToDecimal(location)
                    };


                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
