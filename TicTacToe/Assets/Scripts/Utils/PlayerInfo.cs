﻿#region

using System;
using LitJson;

#endregion

public class PlayerInfo
{
    public string PlayerName = "";
    public string PlayerRating = "";
    public string ProfileId = "";
    public string Rank = "";
    public string Score = "";


    public PlayerInfo(JsonData jsonData)
    {
        PlayerName = SafeGet(jsonData, "playerName");

        if (PlayerName.Equals("")) PlayerName = SafeGet(jsonData, "name");

        Score = SafeGet(jsonData, "score");


        ProfileId = SafeGet(jsonData, "playerId");

        if (ProfileId.Equals("")) ProfileId = SafeGet(jsonData, "profileId");

        Rank = SafeGet(jsonData, "rank");

        PlayerRating = SafeGet(jsonData, "playerRating");
    }

    public PlayerInfo()
    {
    }

    private string SafeGet(JsonData jsonData, string key)
    {
        var returnValue = "";

        try
        {
            returnValue = jsonData[key].ToString();
        }
        catch (Exception e)
        {
        }

        return returnValue;
    }
}