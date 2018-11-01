using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWin : MonoBehaviour {

    public Team team;
    public void TriggerAWin()
    {
        if (team == Team.TEAM1)
            FindObjectOfType<FindWinner>().TriggerTeam2Win();
        else
            FindObjectOfType<FindWinner>().TriggerTeam1Win();
    }
}
