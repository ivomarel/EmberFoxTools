using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    public TMP_Text descriptionText;
    protected Quest quest;
    
    public void SetQuest(Quest quest)
    {
        this.quest = quest;
    }


}
