using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Net.Security;
using System.Reflection;
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
        private bool useHttps;


        private static String pseudo;
        private static string dbToken;

        // timeout in ms
        [SerializeField] 
        private int timeout;
        
        private static DatabaseRequester instance;
        private int httpPort = 8080;
        private int httpsPort = 4300;
        
        // Sets the instance reference
        private void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AsyncQuery(string query)
        {
            // - create query (use stringBuilder ?)
            string url = useHttps ? "https://" : "http://";
            var port = useHttps ? httpsPort : httpPort;
            url += ip + ":" + port + query;
            
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
 
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            
            try
            {
                // - don't wait for response
                request.GetResponse().Close();
            }

            catch (WebException e)
            {
                // - ignore this exception, due to the response.Close() launch even if the response in not fully received
                if (e.Status != WebExceptionStatus.ReceiveFailure)
                    Debug.Log("Exception occured when trying to reach " + url + " : " + e.Status);          
            } 
        }

        public void SyncQuery(string query, out int status, out string res)
        {
            string url = useHttps ? "https://" : "http://";
            var port = useHttps ? httpsPort : httpPort;
            url += ip + ":" + port + query;
            
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Timeout = timeout;
            
            try
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                var dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                status = (int) response.StatusCode;
                res = responseFromServer;
                response.Close();
            }
            
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    Debug.Log("Exception TIMEOUT occured when trying to reach : " + url);
                    status = 400;
                    res = "Timeout : Can't Battle Robo server";
                }

                else
                {
                    Debug.Log("Exception OTHER occured when trying to reach : " + url);
                    var stream = e.Response.GetResponseStream();
                    var reader = new StreamReader(stream);

                    status = 400;
                    res = reader.ReadToEnd();
                }                
            }   
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

        public void PingServer()
        {
            // - the corountine must be started in the DBRequester because it is never destroyed
            StartCoroutine(PingServerCorountine());
        }

        public IEnumerator PingServerCorountine()
        {
            while (true)
            {
                this.AsyncQuery("/is_alive?token=" + dbToken);
                yield return new WaitForSeconds(10);
            }
        }

        // HANDLE DISCONNECTION FROM PLAYER
        // - Handle ALT F4
        public void OnApplicationQuit()
        {
            DatabaseRequester.Logout();
        }

        public void Quit()
        {
            DatabaseRequester.Logout();
        }

        public static void SetPlayerStat(int kills, int win, string token)
        {
            string query = "/update_player?token=" + token + "&kill=" + kills + "&win=" + win;

            instance.AsyncQuery(query);
        }

        public static void Logout()
        {
            instance.AsyncQuery("/logout?token=" + dbToken);
        }

        public static void AddPlayer(string pseudo, string password, out int status, out string response)
        {
            string query = "/add_player?pseudo=" + pseudo + "&pass=" + password;

            instance.SyncQuery(query, out status, out response);
        }

        public static void Authenticate(string pseudo, string password, out int status, out string response)
        {
            string query = "/auth?pseudo=" + pseudo + "&pass=" + password;

            instance.SyncQuery(query, out status, out response);
        }

        public static void Leaderboard(out int status, out string response)
        {
            instance.SyncQuery("/leaderboard?token=" + dbToken, out status, out response);
        }


        public static String GetPlayerPseudo()
        {
            return pseudo;
        }

        public static void SetPseudo(String p)
        {
            pseudo = p;
        }

        public static void SetDBToken(string token)
        {
            DatabaseRequester.dbToken = token;
        }

        public static string GetDBToken()
        {
            return dbToken;
        }
    }
}
