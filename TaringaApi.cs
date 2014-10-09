using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace ClassLibrary1
{
    public class TaringaApi
    {
        string user;
        string passwd;
        string userKey;
        string userid;
        HTTPRequests request;

        public TaringaApi(string user, string password){

            this.user = user;
            this.passwd = password;

            request = new HTTPRequests();
            
        }

        public string login()
        {
            string logg = request.postRequest("https://www.taringa.net/registro/login-submit.php", "connect=&redirect=%2F&nick="+this.user+"&pass="+this.passwd+"&=");
            getKeys(request.getRequest("http://www.taringa.net/mi"));

            return logg;
        }

        public string sendShout(string shout,int attach_type=0 ,string attach_url="", int privacy=1) 
        {
            string ress = request.postRequest("http://www.taringa.net/ajax/shout/add", "key=" + this.userKey + "&body=" + System.Uri.EscapeDataString(shout) + "&privacy=" + privacy.ToString() + "&attachment_type=" + attach_type + "&attachment=" + attach_url);

            if (ress.Contains("0: "))
            {
                return "No se pudo enviar el shout :|";
            }

            return getLastShoutUrl(userid);
            
        }

        private bool getKeys(string html)
        {
            Match m = default(Match);
            string HRefPattern = "user_key:\\s*([\"\"\'](?<1>[^\"\"\']*)[\"\"\'])";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    this.userKey = m.Groups[1].Value.Replace("'", "");
                    m = m.NextMatch();
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            HRefPattern = "user:\\s*[\"\"\'](?<1>[^\']*)[\"\"\']";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                this.userid = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public string getLastShoutUrl(string id)
        {
            string unprocessed = request.getRequest("http://api.taringa.net/shout/user/view/" + id);

            string processed;

            JArray processing = JArray.Parse(unprocessed);
            
            string lastShout = (string)processing.First.ToString();
            JObject lastSh = JObject.Parse(lastShout);

            processed = (string)lastSh.SelectToken("canonical");

            return processed;
        }

        public string User { get { return this.user; } }
        public string Password { get { return this.passwd; } }

        private string getUserNameByNick(string NickName)
        {
            string u = request.getRequest("http://api.taringa.net/user/nick/view/" + NickName);
            return u;
        }
        private string getUserNameById(int id)
        {
            string u = request.getRequest("http://api.taringa.net/user/view/" + id);
            return u;
        }

        public string getRank(string Nickname)
        {
            string u = getUserNameByNick(Nickname);
            JObject p = JObject.Parse(u);
            try
            {
                string rnk = (string)p.SelectToken("range").SelectToken("name");
                return rnk;
            }
            catch (Exception)
            {
                string codeN = (string)p.SelectToken("code");
                string msgg = (string)p.SelectToken("message");
                return String.Format("Response: {0}, Message: {1}", codeN, msgg);
            }
            return "false";
        }
        public string getRank(int id)
        {
            string u = getUserNameById(id);
            JObject p = JObject.Parse(u);
            try
            {
                string rnk = (string)p.SelectToken("range").SelectToken("name");
                return rnk;
            }
            catch (Exception)
            {
                string codeN = (string)p.SelectToken("code");
                string msgg = (string)p.SelectToken("message");
                return String.Format("Response: {0}, Message: {1}", codeN, msgg);
            }
            return "false";
        }
        public bool isBanned(string username) 
        {
            return false;
        }

    }
}
