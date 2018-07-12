using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;


namespace BattleRobo
{
    public class DatabaseRequester : MonoBehaviour
    {
        [SerializeField]
        private bool isAuthenticationServerEnabled;

        [SerializeField]
        private string ip;

        [SerializeField]
        private string port;

        [SerializeField]
        private bool useHttps;

        private static DatabaseRequester instance;

        // Sets the instance reference
        private void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /*
        public void AsyncQuery(string query)
        {
            // - create query (use stringBuilder ?)
            string url = useHttps ? "https://" : "http://";
            url += ip + ":" + port + "/" + query;

            // - don't wait for response
            WWW www = new WWW(url);
        }*/

        /*
        public void SyncQuery(string query, out int status, out string res)
        {
            // - create query (use stringBuilder ?)
            string url = useHttps ? "https://" : "http://";
            url += ip + ":" + port + "/" + query;

            WWW www = new WWW(url);

            // - wait response
            while (!www.isDone) ;

            Debug.Log(www.error);
            if (www.responseHeaders["STATUS"].Contains("200"))
                status = 200;

            else
                status = 400;

            res = www.text;
        }*/

        public void AsyncQuery(string query)
        {
            // - create query (use stringBuilder ?)
            string url = useHttps ? "https://" : "http://";
            url += ip + ":" + port + "/" + query;

            // - don't wait for response
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
 
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            
            // - fire and forget pattern
            ThreadPool.QueueUserWorkItem(o=>{ request.GetResponse(); });
            
        }

        public void SyncQuery(string query, out int status, out string res)
        {
            string url = useHttps ? "https://" : "http://";
            url += ip + ":" + port + "/" + query;
            
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
 
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            Stream dataStream = response.GetResponseStream ();
            StreamReader reader = new StreamReader (dataStream);
            string responseFromServer = reader.ReadToEnd ();
 
            status = (int) response.StatusCode;
            res = responseFromServer;
            Debug.Log("STATUS : " + status + " - " + res + "URL USED : " + url);
        }
        
        private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        {
            // all Certificates are accepted
            return true;
        }

        public void EnableAuthenticationServer(bool enable)
        {
            isAuthenticationServerEnabled = enable;
        }

        public static DatabaseRequester GetInstance()
        {
            return instance;
        }

        // HANDLE DISCONNECTION FROM PLAYER
        // - Handle ALT F4
        public void OnApplicationQuit()
        {
            var playerToken = PlayerInfoScript.GetInstance().GetDBToken();
            DatabaseRequester.GetInstance().AsyncQuery("logout?token=" + playerToken);
        }

        public void Quit()
        {
            var playerToken = PlayerInfoScript.GetInstance().GetDBToken();
            DatabaseRequester.GetInstance().AsyncQuery("logout?token=" + playerToken);
        }
    }
}
