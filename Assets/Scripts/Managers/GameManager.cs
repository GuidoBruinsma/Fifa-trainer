using Unity.Services.Authentication;

/// <summary>
/// Manages general game-wide functionality such as authentication.
/// </summary>
public static class GameManager
{
    /// <summary>
    /// Signs out the current user if they are signed in.
    /// </summary>
    public static void LogOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();
    }
}
