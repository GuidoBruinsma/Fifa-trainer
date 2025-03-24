using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    //Username: test Password: _B123d$a
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Toggle rememberMe;

    public TextMeshProUGUI errorText;

    public TextMeshProUGUI playerUsernameText;

    public GameObject loginPanel;
    public GameObject startPanel;


    async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
        if (PlayerPrefs.HasKey("rememberMe"))
        {
            int state = PlayerPrefs.GetInt("rememberMe");
            if (state == 1)
                SignIn();
        }
    }
    private void Update()
    {
        
    }
    public async void SignIn()
    {
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            try
            {
                await SingInAnonymoulsy();

                Debug.Log($"Seccessfully sign up automatically {AuthenticationService.Instance.PlayerName}");
            }
            catch (AuthenticationException e)
            {
                errorText.text = e.Message;
            }
            catch (RequestFailedException e)
            {
                errorText.text = e.Message;
            }
        }
    }

    public async void SingInAnonymoulsyButton()
    {
        await SingInAnonymoulsy();
    }

    public async Task SingInAnonymoulsy()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PanelManager.OpenClosePanels(startPanel, loginPanel);

            if (PlayerPrefs.HasKey("username"))
            {
                string lastUsername = PlayerPrefs.GetString("username");
                playerUsernameText.text = lastUsername;
            }
            else
                playerUsernameText.text = AuthenticationService.Instance.PlayerName;

            SceneManager.LoadScene(1);
            Debug.Log(AuthenticationService.Instance.IsSignedIn);

        }
        catch (AuthenticationException e)
        {
            errorText.text = e.Message;
        }
        catch (RequestFailedException e)
        {
            errorText.text = e.Message;
        }
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        playerUsernameText.text = "Need to login";
    }

    int ConvertBoolToInt()
    {
        if (rememberMe.isOn)
            return 1;
        else
            return 0;
    }

    public async void SignUp()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            playerUsernameText.text = username + AuthenticationService.Instance.PlayerId;

            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetInt("rememberMe", ConvertBoolToInt());

        }
        catch (AuthenticationException e)
        {
            errorText.text = e.Message;
        }
        catch (RequestFailedException e)
        {
            errorText.text = e.Message;
        }
    }

    public async void LogIn()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            playerUsernameText.text = username + AuthenticationService.Instance.PlayerId;

            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);

            PlayerPrefs.SetInt("rememberMe", ConvertBoolToInt());

            if (!PlayerPrefs.HasKey("username"))
            {
                PlayerPrefs.SetString("username", username);
            }

            Debug.Log($"Seccessfully logged In {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException e)
        {
            errorText.text = e.Message;
        }
        catch (RequestFailedException e)
        {
            errorText.text = e.Message;
        }
    }

    public void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Seccessfully sign up {AuthenticationService.Instance.PlayerName}");
        }; AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log($"Seccessfully sign out {AuthenticationService.Instance.PlayerName}");
        };
    }
}
