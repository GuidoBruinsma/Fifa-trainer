using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    //Username: test Password: _B123d$a
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI errorText;

    public TextMeshProUGUI playerUsernameText;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        //SignIn();
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

            playerUsernameText.text = AuthenticationService.Instance.PlayerName;
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

    public void SignOut() {
        AuthenticationService.Instance.SignOut();
    }

    public async void SignUp() {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            playerUsernameText.text = username + AuthenticationService.Instance.PlayerId;
            Debug.Log($"Seccessfully sign up {AuthenticationService.Instance.PlayerId}");
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

    public async void LogIn() {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);

            playerUsernameText.text = username + AuthenticationService.Instance.PlayerId;

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

    public void SetupEvents() {
        AuthenticationService.Instance.SignedIn += () => {
         
        };
    }
}
