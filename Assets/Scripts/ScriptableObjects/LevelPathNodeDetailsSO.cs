using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Level/LevelPathNodeDetails"), Serializable]
public class LevelPathNodeDetailsSO : ScriptableObject
{
    [SerializeField] public List<PathNodeDetails> LevelPathNodeDetails;
    [SerializeField] public List<PathNodeDetails> ReturnLevelPathNodeDetails;

    [Serializable]
    public class PathNodeDetails
    {
        // TODO: add definition of what failure is, the event/s it changes into, their information
        public HEventType.HType EventType;
        [TextArea(2, 4)]
        public string StartEventTextOverride;
        [TextArea(2, 4)]
        public string FinishEventTextOverride;
        // what is the aux event text for?
        public List<string> AuxiliaryEventText;
        public string EventNameOverride;
        public string EventVerbOverride;
        public string EventSubjectOverride;
        public int Difficulty;
        public bool GuaranteeFail;
        public bool IsExtractionNode;
        public int NumEnemies;
    }
}
