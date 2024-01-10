/*
 * woa boy here we go, this is gonna be a doozy...
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CombatEvent : BaseEvent
{
    private CombatRound combatRound;
    private bool hasFailedFleeing;
    private bool hasSucceededRouting;
    private int roundNumber;
    public override void EventEnd()
    {
        base.EventEnd();
    }

    public override void EventStart(CrewController crew)
    {
        Crew = crew;
        roundNumber = 1;
        combatRound = new CombatRound(Crew, EnemyCrew, roundNumber);

        // TODO: handle no or dead enemies

        GameLog.Instance.PostMessageToLog($"Round {roundNumber} has begun");
    }

    public override bool HasFailed()
    {
        return hasFailedFleeing;
    }

    public override bool HasSucceeded()
    {
        return hasSucceededRouting;
    }

    public override string MyNameIs()
    {
        return "CombatEvent";
    }

    public override bool StepEvent()
    {
        if (combatRound.RoundComplete())
        {
            // TODO: handle another enemy crew joining in
            roundNumber++;
            GameLog.Instance.PostMessageToLog($"Round {roundNumber} has begun");
            combatRound = new CombatRound(Crew, EnemyCrew, roundNumber);
        }
        combatRound.StepRound();
        int status = combatRound.HasSomeoneWon();
        if (status == 1) hasSucceededRouting = true;
        if (status == 2) hasFailedFleeing = true;
        return true;
    }

    public override void SubStepOne()
    {
        base.SubStepOne();
    }

    public override void SubStepThree()
    {
        base.SubStepThree();
    }

    public override void SubStepTwo()
    {
        base.SubStepTwo();
    }
}
