using System;
using System.Collections.Generic;

public class TeamSelection
{
    //unit, commad
    public static List<(Type, Type)> teams = new List<(Type, Type)>()
    {
        (typeof(CustomUnit2), typeof(CustomCommand2)), //team1
        (typeof(CustomUnit2), typeof(CustomCommand2))  //team2
    };
    public static List<string> teamNames = new List<string>()
    {
        "Team1Name",
        "Team2Name"
    };
}