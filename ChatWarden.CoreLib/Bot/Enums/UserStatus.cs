namespace ChatWarden.CoreLib.Bot
{
    public enum UserStatus : byte
    {
        Unknown = 0,
        Banned = 1,
        Common = 10,
        Priveleged = 50,
        Admin = 100,
        SuperAdmin = 255,
    }
}
