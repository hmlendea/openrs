namespace OpenRS.Net.Enumerations
{
    public enum LoginCode
    {
        ServerTimeOut = -1,
        Code0 = 0, // TODO: Find its meaning
        Code1 = 1, // TODO: Find its meaning
        InvalidCredentials = 2,
        UsernameAlreadyLoggedIn = 3,
        ClientUpdated = 4,
        Code5 = 5, // TODO: Find its meaning
        AccountBanned = 6,
        ProfileDecodeFailure = 7,
        TooManyConnections = 8,
        AccountAlreadyLoggedIn = 9,
        Code99 = 99 // TODO: Find its meaning
    }
}
