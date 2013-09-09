using System;
using System.Dynamic;

namespace Facebook
{
    public class Post
    {
        public static bool PostToWall(string accessToken, string domainName, string domain, string message, string link,
                                      string picture, string name, string caption, string description)
        {
            int i = 0;
            while (i < 3)
            {
                i++;
                try
                {
                    var client = new FacebookClient(accessToken);
                    dynamic parameters = new ExpandoObject();
                    parameters.message = message;
                    parameters.link = link;
                    parameters.picture = picture;
                    parameters.name = name;
                    parameters.caption = caption;
                    parameters.description = description;
                    parameters.actions = new
                        {
                            name = domainName,
                            link = domain,
                        };
                    parameters.privacy = new
                        {
                            value = "ALL_FRIENDS",
                        };
                    parameters.targeting = new
                        {
                            countries = "TR",
                            regions = "6,53",
                            locales = "6",
                        };
                    dynamic result = client.Post("me/feed", parameters);

                    i = 3;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
