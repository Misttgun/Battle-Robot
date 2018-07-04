using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void AsyncQuery(string query)
    {
        // - create query (use stringBuilder ?)
        string url = useHttps ? "https://" : "http://";
        url += ip + ":" + port + "/" + query;

        // - don't wait for response
        WWW www = new WWW(url);
    }

    public void SyncQuery(string query, out int status, out string res)
    {
        // - create query (use stringBuilder ?)
        string url = useHttps ? "https://" : "http://";
        url += ip + ":" + port + "/" + query;

        WWW www = new WWW(url);

        // - wait response
        while (!www.isDone);

        Debug.Log(www.error);
        if (www.responseHeaders["STATUS"].Contains("200"))
            status = 200;

        else
            status = 400;

        res = www.text;
    }

    public void EnableAuthenticationServer(bool enable)
    {
        isAuthenticationServerEnabled = enable;
    }

    public static DatabaseRequester GetInstance()
    {
        return instance;
    }

}
