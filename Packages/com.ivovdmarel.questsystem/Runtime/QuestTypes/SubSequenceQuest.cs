using UnityEngine;

public class SubSequenceQuest : Quest
{
    public QuestSequence subSequence;
    public string subSequenceTag;

    private bool subSequencePassed;
    
    public override void OnQuestEnable()
    {
        base.OnQuestEnable();
        if (subSequence == null)
        {
            subSequence = GameObject.FindGameObjectWithTag(subSequenceTag)?.GetComponent<QuestSequence>();
            if (subSequence == null)
            {
                Debug.LogError($"No object with tag {subSequenceTag} and QuestSequence component found");
                return;
            }
        }
        
        subSequence.SequencePassed.AddListener(OnSubSequencePassed);
        subSequence.Init();
    }

    public override void OnQuestDisable()
    {
        base.OnQuestDisable();  
        if (subSequence != null)
        {
            subSequence.SequencePassed.RemoveListener(OnSubSequencePassed);
        }
    }

    private void OnSubSequencePassed()
    {
        subSequencePassed = true;
    }

    protected override bool IsCompleted() => subSequencePassed;
}
