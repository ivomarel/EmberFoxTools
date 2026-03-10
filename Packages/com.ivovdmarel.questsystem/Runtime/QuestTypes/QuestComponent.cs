using System.Text;
using UnityEngine;

public abstract class QuestComponent : MonoBehaviour
{
    protected Quest quest => _quest == null ? _quest = GetComponent<Quest>() : _quest;
    private Quest _quest;

    public abstract void OnQuestEnable();
    
    public abstract void OnQuestDisable();

    public abstract void OnQuestUpdate();

}