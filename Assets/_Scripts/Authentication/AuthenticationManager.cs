using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages player authentication using Unity's Authentication and Core services.
/// Supports sign-up, login, logout, and anonymous sign-in, with "Remember Me" support.
/// </summary>
public class AuthenticationManager : MonoBehaviour
{

    /// <summary>
    /// Singleton instance of the AuthenticationManager.
    /// </summary>
    public static AuthenticationManager instance;

    //Username: test Password: _B123d$a
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Toggle rememberMe;

    public TextMeshProUGUI errorText;

    public GameObject loginPanel;
    public GameObject startPanel;

    /// <summary>
    /// Initializes Unity Services and sets up authentication event handlers.
    /// Auto signs-in if "Remember Me" was previously checked.
    /// </summary>
    async void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else { Destroy(this.gameObject); }



        await UnityServices.InitializeAsync();
        SetupEvents();
        if (PlayerPrefs.HasKey("rememberMe"))
        {
            int state = PlayerPrefs.GetInt("rememberMe");
            if (state == 1)
                SignIn();
        }
    }

    /// <summary>
    /// Signs in automatically if a valid session token exists.
    /// </summary>
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

    /// <summary>
    /// Called from a UI button to trigger anonymous sign-in.
    /// </summary>
    public async void SingInAnonymoulsyButton()
    {
        await SingInAnonymoulsy();
    }

    /// <summary>
    /// Signs in the player anonymously using Unity Authentication.
    /// </summary>
    public async Task SingInAnonymoulsy()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();


            if (PlayerPrefs.HasKey("username"))
            {
                string lastUsername = PlayerPrefs.GetString("username");
            }

            // SceneManager.LoadScene(1);
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

    /// <summary>
    /// Converts the rememberMe toggle to an integer for saving in PlayerPrefs.
    /// </summary>
    /// <returns>1 if toggle is on, otherwise 0.</returns>
    int ConvertBoolToInt()
    {
        if (rememberMe.isOn)
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// Signs up a new user using username and password.
    /// Saves user preferences if successful.
    /// </summary>
    public async void SignUp()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

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

    /// <summary>
    /// Logs in the user using credentials from input fields.
    /// Updates player name and stores preferences.
    /// </summary>
    public async void LogIn()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);

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

    /// <summary>
    /// Signs the user out and clears session data.
    /// </summary>
    public void LogOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut();

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

    /// <summary>
    /// Sets up event handlers for Unity Authentication lifecycle events.
    /// </summary>
    public void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            if (loginPanel != null && startPanel != null)
            {
                PanelManager.OpenClosePanels(startPanel, loginPanel, true);
                Debug.Log($"Successfully signed up {AuthenticationService.Instance.PlayerName}");
            }
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            PanelManager.OpenClosePanels(loginPanel, startPanel, true);
        };

        AuthenticationService.Instance.Expired += () =>
        {
            SceneManager.LoadScene(0);
            Debug.Log($"Seccessfully sign out {AuthenticationService.Instance.PlayerName}");
        };
    }
}
