using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam
{
    public string teamName;
    public float score;
    public Color teamColor;
    public bool ready;

    public List<Unit> units = new List<Unit>();

    public PlayerTeam()
    {
        teamName = "Team";
        teamColor = Random.ColorHSV();
    }
    public PlayerTeam(string teamName, Color teamColor)
    {
        this.teamName = teamName;
        this.teamColor = teamColor;
    }
    public PlayerTeam(string teamName, Color teamColor, params Unit[] units)
    {
        this.teamName = teamName;
        this.teamColor = teamColor;
        for (int i = 0; i < units.Length; i++)
        {
            this.units.Add(units[i]);
        }
    }
}
