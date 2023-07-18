using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Google;
using System.Threading.Tasks;
using Firebase.Extensions;

public class GoogleSocialLogin : MonoBehaviour
{
    private string GOOGLE_WEB_KEY = "319589117169-kek8ko4rastfs54o9j6o33k2mmbh1q7c.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;

    [System.Serializable]
    private class LoginRequestBody
    {
        public string name;
        public string email;
    }

    [System.Serializable]
    private class LoginResponseBody
    {
        public string name;
        public string email;
        public string _id;
    }



    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GOOGLE_WEB_KEY,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        
    }

    public void Login()
    {
        // SceneManager.LoadScene("Home");
        Debug.Log("click google social login button");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthFinished);
        
    }
    private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }
                user = auth.CurrentUser;
                Debug.Log("username : " + user.DisplayName);
                Debug.Log("email: " + user.Email);
                PostLogin(user.DisplayName, user.Email);
            });
        }
    }

    private void PostLogin(string name, string email)
    {
        string url = "http://172.10.5.110/user/login";
        LoginRequestBody body = new LoginRequestBody();
        body.email = email;
        body.name = name;
        string str = JsonUtility.ToJson(body);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        LoginResponseBody responseBody = JsonUtility.FromJson<LoginResponseBody>(json);
        UserInfo.email = responseBody.email;
        UserInfo.name = responseBody.name;
        UserInfo.id = responseBody._id;
        Toast.Show(UserInfo.email + " " + UserInfo.name + " " + UserInfo.id);
        SceneManager.LoadScene("Home");
    }
}
