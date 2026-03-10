using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class QuestSequence : MonoBehaviour
{
    public static QuestSequence current;

    public UnityEvent SequenceStarted;
    public UnityEvent SequenceEnded;
    public UnityEvent SequencePassed;

    public Quest currentQuest
    {
        get
        {
            if (quests.Length > index)
            {
                return quests[index];
            }

            return null;
        }
    }


    public bool autoStart;

    public bool isActive;

    private Quest[] quests => _quests != null && _quests.Length > 0 ? _quests : _quests = GetComponentsInChildren<Quest>();

    private Quest[] _quests;

    //Needs to be saved later.
    private int index;

    public float delayBetweenQuests = 0f;

    protected virtual void Start()
    {
        if (autoStart)
        {
            //Debug.Log($"Auto starting quest sequence {name}", this);
            Init();
        }
    }


    private void Update()
    {
        if (!isActive)
            return;
    }

    public void Init()
    {
        if (isActive)
            return;

        foreach (var quest in quests)
        {
            quest.state = QuestState.Inactive;
        }

        current = this;
        index = 0;
        isActive = true;
        SequenceStarted?.Invoke();

        EnableCurrentQuest();
    }

    public void CompleteQuest(Quest quest)
    {
        StartCoroutine(StartNewQuestAfterDelay());
    }

    private IEnumerator StartNewQuestAfterDelay()
    {
        if (delayBetweenQuests > 0)
        {
            yield return new WaitForSeconds(delayBetweenQuests);    
        }
        index++;
        EnableCurrentQuest();
    }

    private void EnableCurrentQuest()
    {
        if (index < quests.Length)
        {
            quests[index].OnQuestEnable();
        }
        else
        {
            Complete();
        }
    }

    public virtual void Complete()
    {
        if (!isActive)
            return;

        SequencePassed?.Invoke();

        End();
    }

    public void End()
    {
        if (!isActive)
            return;

        isActive = false;
        if (current == this)
        {
            //In some cases there was still a quest active (when the sequence was canceled)
            currentQuest?.OnQuestDisable();
            current = null;
        }

        SequenceEnded?.Invoke();
    }

    public float GetProgression()
    {
        int qs = quests.Length - 1;
        if (qs <= 0)
            return 1;
        return (float) index / (qs);
    }
}