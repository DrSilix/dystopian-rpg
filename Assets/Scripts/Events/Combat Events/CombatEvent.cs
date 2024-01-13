/*
 * woa boy here we go, this is gonna be a doozy...
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CombatEvent : BaseEvent
{
    public CombatRound CombatRound { get; private set; }
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
        CombatRound = new CombatRound(Crew, EnemyCrew, roundNumber);

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
        if (CombatRound.RoundComplete())
        {
            // TODO: handle another enemy crew joining in
            roundNumber++;
            GameLog.Instance.PostMessageToLog($"Round {roundNumber} has begun");
            CombatRound = new CombatRound(Crew, EnemyCrew, roundNumber);
        }
        CombatRound.StepRound();
        int status = CombatRound.HasSomeoneWon();
        if (status == 1) {
            //GameLog.Instance.PostMessageToLog($"Any remaining enemies are fleeing");
            hasSucceededRouting = true;
        }
        if (status == 2)
        {
            GameLog.Instance.PostMessageToLog($"The remaining members of your crew are running away");
            hasFailedFleeing = true;
        }
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
