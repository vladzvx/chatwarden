﻿namespace ChatWarden.Bot.State
{
    public enum UserStatus : byte
    {
        Common = 0,
        Priveleged = 50,
        Admin = 100,
        SuperAdmin = 255,
    }
}
