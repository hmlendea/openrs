namespace OpenRS.Net
{
    public enum LoginCode
    {
        ServerTimeOut = -1,
        LoginSuccess = 0,
        ReconnectSuccess = 1,
        InvalidCredentials = 2,
        UsernameAlreadyLoggedIn = 3,
        ClientUpdated = 4,
        SessionRejected = 5,
        AccountBanned = 6,
        ProfileDecodeFailure = 7,
        TooManyConnections = 8,
        AccountAlreadyLoggedIn = 9,
        LoginComplete = 99
    }
}
