using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Quest))]
public abstract class QuestEvent : MonoBehaviour
{
    private Quest quest => _quest != null ? _quest : _quest = GetComponent<Quest>();
    private Quest _quest;


    private void OnEnable()
    {
        quest.QuestInititalized.AddListener(OnQuestInitalized);
    }

    protected abstract void OnQuestInitalized();
}