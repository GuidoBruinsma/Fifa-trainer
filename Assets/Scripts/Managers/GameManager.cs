using Unity.Services.Authentication;

public static class GameManager
{
    public static void LogOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();
    }
}
