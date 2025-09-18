using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandData
{
    public readonly int teamId;
    public Dictionary<int, UnitData> unitDataList;
    public int yellowCardCount = 0;

    public CommandData(int teamId)
    {
        this.teamId = teamId;
        unitDataList = new Dictionary<int, UnitData>();
    }
}

