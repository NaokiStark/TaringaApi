using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ClassLibrary1
{
    public class LoginArgs{
        public string Result;
    }
    public class TaringaApi
    {
        string user;
        string passwd;
        string userKey;
        string userid;
        public bool isLoggedIn;
        public string wsPath;
        public HTTPRequests request;

        public delegate void onLogin(object sender, LoginArgs e);
        public event onLogin Login;

        public void toLogin(LoginArgs e){
            if (Login!=null)
                Login(this,e);
        }


        public TaringaApi(string user, string password){

            this.user = user;
            this.passwd = password;

            request = new HTTPRequests();
            request.onRecieveAsync += (rr, re) =>
            {
                switch(re.type){
                    case "login":
                try
                {
                    JObject jo = JObject.Parse(re.response);
                    string Result = "";
                    switch (jo.SelectToken("status").ToString())
                    {
                        case "0":
                            Result = "Datos incorrectos";
                            break;
                        case "1":
                            Result = "true";
                             string html = request.getRequest("http://www.taringa.net/mi");
                            getKeys(html);
                            getWebSocketPath(html);
                            break;
                        case "2":
                            Result = "Esta cuenta se encuentra suspendida.";
                            break;
                    }
                    LoginArgs e = new LoginArgs();
                    e.Result = Result;

                    toLogin(e);
                }
                catch
                {
                    LoginArgs e = new LoginArgs();
                    e.Result = "false";

                    toLogin(e);
                }
                break;
                    case "shout":

                break;


            }

            };
            
        }


        public void loginString(string user, string passwr)
        {
            this.user = user;
            this.passwd = passwr;
            login();                

        }
       
        public void login()
        {
             request.postRequest("https://www.taringa.net/registro/login-submit.php", "connect=&redirect=%2F&nick=" + this.user + "&pass=" + this.passwd,"login");
            
        }

        public string sendShout(string shout,int attach_type=0 ,string attach_url="", int privacy=1) 
        {
            request.postRequest("http://www.taringa.net/ajax/shout/add", "key=" + this.userKey + "&body=" + System.Uri.EscapeDataString(shout) + "&privacy=" + privacy.ToString() + "&attachment_type=" + attach_type + "&attachment=" + attach_url,"shout",true);

        

            /*if (ress.Contains("0: "))
            {
                return "No se pudo enviar el shout :|";
            }
            */
            return getLastShoutUrl(userid);
            
        }

        private bool getWebSocketPath(string html)
        {
            string ip;
            string port;
            string wskey;

            Match m = default(Match);
            string HRefPattern = "[\"\"\']host[\"\"\']:[\"\"\']\\s*(?:[\"\"\'](?<1>[^\"\"\']*)[\"\"\']|(?<1>\\S+))[\"\"\'],";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                ip = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return false;
            }
            HRefPattern = "\"port\":((?<1>[^\"\"']*))}";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                port = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return false;
            }

            HRefPattern = "notifications\\(([\"\"\'](?<1>[^\"\"\']*)[\"\"\'])";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                wskey = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return false;
            }
            this.wsPath = "ws://" + ip + ":" + port+"/ws/" + wskey.Replace("'",""); ;
            return true;
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
    public class dResponse:IDisposable
    {
        public string response;
        public StreamReader StrRe;
        public string type;
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
