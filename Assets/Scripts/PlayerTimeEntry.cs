using System;
using System.Collections.Generic;

[Serializable]
public class PlayerTimeEntry
{
    public string playerName;
    public float time;
}

[Serializable]
public class PlayerTimeList
{
    public List<PlayerTimeEntry> playerTimes = new List<PlayerTimeEntry>();
}
