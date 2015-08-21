using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;
namespace ClassLibrary1
{
    public class LoginArgs{
        public string Result;
    }
    public class TaringaApi
    {
        string user;
        string passwd;
        public string userKey;
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

        public delegate void onRealtimeRecieve(string data);
        public event onRealtimeRecieve onWsDataRecieve;

        public void RecieveWsData(string data)
        {
            if (onWsDataRecieve != null)
                onWsDataRecieve(data);
        }
        public void setTaringacookie(string hash)
        {
          //Nothing
            
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

        public string SyncLogin()
        {
            
            try
            {
                string e = request.oldPostRequest("https://www.taringa.net/registro/login-submit.php", "connect=&redirect=%2F&nick=" + this.user + "&pass=" + this.passwd, "login");
                JObject jo = JObject.Parse(e);
                string Result = "";
                switch (jo.SelectToken("status").ToString())
                {
                    case "0":
                        Result = "Datos incorrectos";
                        break;
                    case "1":
                        
                        string html = request.getRequest("http://www.taringa.net/mi");
                        getKeys(html);
                        getWebSocketPath(html);
                        
                            Result = "true";
                        break;
                    case "2":
                        Result = "suspended";
                        break;
                }
                
                return Result;
            }
            catch
            {
                return "false";
            }
        

        }
        public void AsyncLogin()
        {
             request.postRequest("https://www.taringa.net/registro/login-submit.php", "connect=&redirect=%2F&nick=" + this.user + "&pass=" + this.passwd,"login");
            
        }

        public string getLastCommentOfShoutId(string oid)
        {

            var r = request.getRequest("http://api.taringa.net/shout/comment/view?object_id="+oid);
            JArray ee = JArray.Parse(r);

            return ee.Last.SelectToken("id").ToString().Split('.')[1];           


        }
        public CommentComponent getComment(string oId,string comId, bool isresponse){
            var e = new CommentComponent();
            var r = this.request.oldPostRequest("http://www.taringa.net/ajax/comments/get", "key="+userKey+"&objectType=shout&objectId="+oId+"&commentId="+comId, "", true);        
            var reg = new Regex(@"data-signature=""([a-zA-Z0-9]+)""");
            MatchCollection matc = reg.Matches(r);
            e.signature = matc[0].Groups[1].Value;
            reg = new Regex(@"<div class=""comment-content"">\s*(.*?)\s*<\/div>");
            matc = reg.Matches(r);
            e.content = matc[matc.Count-1].Groups[1].Value;
            reg = new Regex(@"data-owner=\""([a-zA-Z0-9]+)""");
            matc = reg.Matches(r);
            e.owner = matc[0].Groups[1].Value; 
            e.id = comId;
            e.objectId = oId;
            e.cparent = "";
            e.idparent = "";

            return e;
        }

        public string sendShout(string shout,int attach_type=0 ,string attach_url="", int privacy=1) 
        {
           string result= request.oldPostRequest("http://www.taringa.net/ajax/shout/add", "key=" + this.userKey + "&body=" + System.Uri.EscapeDataString(shout) + "&privacy=" + privacy.ToString() + "&attachment_type=" + attach_type + "&attachment=" + attach_url,"shout",true);

           return (result.StartsWith("0"))?"0":"1";
            
        }
        public string getWebSocketPath()
        {
            return getWebSocketPath(request.getRequest("http://www.taringa.net/mi"));
        }
        private string getWebSocketPath(string html)
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
                return "false";
            }
            HRefPattern = "\"port\":((?<1>[^\"\"']*))}";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                port = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return "";
            }

            HRefPattern = "notifications\\(([\"\"\'](?<1>[^\"\"\']*)[\"\"\'])";
            try
            {
                m = Regex.Match(html, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                wskey = m.Groups[1].Value;
            }
            catch (Exception ex)
            {
                return "";
            }
            this.wsPath = "ws://" + ip + ":" + port+"/ws/" + wskey.Replace("'",""); ;
            return this.wsPath;
        }
        public void sendPing(){
            request.oldPostRequest("http://www.taringa.net/ajax/liveupdate/ping", "key=" + userKey, "", true);
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

        public string User { get { return this.user; } set { this.user = value; } }
        public string Password { get { return this.passwd; } set { this.passwd = value; } }

        private string getUserNameByNick(string NickName)
        {
            string u = request.getRequest("http://api.taringa.net/user/nick/view/" + NickName);
            return u;
        }
        public string getUserNameById(int id)
        {
            string u = request.getRequest("http://api.taringa.net/user/view/" + id);
            return u;
        }
        public string follow(string user)
        {
            string rr = getUserNameByNick(user);
            if (rr == "false")
                return "No se pudo seguir al usuario";
            JObject uData = JObject.Parse(rr);
            string uuid = uData["id"].ToString();
            return request.oldPostRequest("http://www.taringa.net/notificaciones-ajax.php", "key=" + userKey +"&type=user&obj="+uuid+"&action=follow", "", true);
        }
        public string unfollow(string user)
        {
            string rr = getUserNameByNick(user);
            if (rr == "false")
                return "No se pudo dejar de seguir al usuario";
            JObject uData = JObject.Parse(rr);
            string uuid = uData["id"].ToString();
            return request.oldPostRequest("http://www.taringa.net/notificaciones-ajax.php", "key=" + userKey + "&type=user&obj=" + uuid + "&action=unfollow", "", true);
        }
        public string getAvatarURL(string user)
        {
            string rr = getUserNameByNick(user);
            if (rr == "false")
                return "false";
            JObject uData = JObject.Parse(rr);
            string avatarURL = uData["avatar"]["big"].ToString();
            return avatarURL;
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

        public void StartWs()
        {
         
            
        }
        public string sendComment(string comment,string oId,string uId, string oparent ="", string iparent="")
        {
            string ip="";
            string op="";
            if (oparent != "" && oparent != "")
            {
                ip = "&parent=" + iparent;
                op = "&parentOwner=" + oparent;
            }



            string g = request.oldPostRequest("http://www.taringa.net/ajax/comments/add", "key=" + userKey + "&comment=" + comment + "&objectId="+oId+"&objectOwner="+uId+"&objectType=shout&show=true"+ip+op, "", true);
            return g;

        }
        public string Like_shout(string oId, string uId)
        {
            string g = request.oldPostRequest("http://www.taringa.net/ajax/shout/vote", "key=" + userKey + "&uuid=" + oId + "&owner=" + uId + "&score=1", "", true);
            return g;
        }

        /*Get Realtime shouts*/
        public shoutComponents getNewShouts()
        {
            string g = request.oldPostRequest("http://www.taringa.net/ajax/feed/fetch", "key=" + userKey + "&feedName=newsfeed", "", true);
            List<string> ids;
            List<string> ownerid;
            List<string> shoutid ;
            List<string> shoutContent;
            var Shouts = new shoutComponents();
            if (g != "false")
            {
                ids = ParseShoutsId(g);
                ownerid = parseShoutOwner(g);
                shoutid = parseShoutObjectId(g);
                shoutContent = parseShoutContent(g);
            }
            else
            {
                ids = new List<string>();
                ids.Add("no");
                ownerid = new List<string>();
                ownerid.Add("no");
                shoutid = new List<string>();
                shoutid.Add("no");
                shoutContent =new List<string>();
                shoutContent.Add("no");

            }
            Shouts = new shoutComponents();
            Shouts.objectId = shoutid;
            Shouts.objectOwner = ownerid;
            Shouts.shoutId = ids;
            Shouts.shoutContent = shoutContent;
            return Shouts;
        }

        private List<string> ParseShoutsId(string data)
        {
            var g= new List<string>();

            //Regex_: data-feed="([a-zA-Z0-9]+)"
            string pattern=@"data-feed=""([a-zA-Z0-9]+)""";
            var regex= new Regex(pattern);
            var matches = regex.Matches(data);

            foreach (Match j in matches)
            {
                g.Add(j.Groups[1].Value);
            }
            
            return g;
        }
        private List<string> parseShoutOwner(string data)
        {
            //Regex_: data-id="([0-9]+)"
            var g = new List<string>();

            string pattern = @"data-id=""([0-9]+)""";
            var regex = new Regex(pattern);
            var matches = regex.Matches(data);

            foreach (Match j in matches)
            {
                g.Add(j.Groups[1].Value);
            }

             return g;
        }

        public string getUserKey()
        {

            string r= request.getRequest("http://www.taringa.net/protocolo");

            return getData(@"user_key:\s*'([a-zA-Z0-9]+)'",r);

        }

        private string getData(string pattern,string data)
        {
            string result;
            var regex = new Regex(pattern);
            var matches = regex.Matches(data);
            result = matches[0].Groups[1].Value;
            return result;
        }

        private List<string> parseShoutObjectId(string data)
        {
            //Regex_: data-owner="([0-9]+)"
            var g = new List<string>();
            string pattern = @"data-owner=""([0-9]+)""";
            var regex = new Regex(pattern);
            var matches = regex.Matches(data);

            foreach (Match j in matches)
            {
                g.Add(j.Groups[1].Value);
            }
            return g;
        }
        private List<string> parseShoutContent(string data)
        {
            //Regex_: <p>(.*?)<\/p>
            var g = new List<string>();
            string pattern = @"<p>(.*?)<\/p>";
            var regex = new Regex(pattern);
            var matches = regex.Matches(data);

            foreach (Match j in matches)
            {
                g.Add(j.Groups[1].Value);
            }
            return g;
        }
    }
    public class shoutComponents
    {
        public List<string> objectId;
        public List<string> objectOwner;
        public List<string> shoutId;
        public List<string> shoutContent;
        public List<string> shoutLink;
    }

    public class CommentComponent{
        public string id;
        public string owner;
        public string objectId;
        public string signature;
        public string content;
        public string cparent;
        public string idparent;
        public string oparent;
        public string senderStr;
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
