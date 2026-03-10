using UnityEngine;
using UnityEngine.Events;

public class Quest : MonoBehaviour
{
    [HideInInspector]
    public QuestState state = QuestState.Inactive;

    public UnityEvent QuestInititalized;
    public UnityEvent QuestCompleted;

    protected bool isDebugCompleted;

    public QuestSequence sequence
    {
        get
        {
            if (_questSequence == null)
            {
                _questSequence = GetComponentInParent<QuestSequence>();
                if (sequence == null)
                {
                    Debug.LogError("No QuestSequence found.");
                }
            }

            return _questSequence;
        }
    }

    [Tooltip("When this Quest is enabled, it will at any time check if its dependencies are met. If not, it will go back to the first un-met dependency Quest")]
    public Quest[] dependencies;

    private Quest currentDependency;

    private QuestSequence _questSequence;

    protected QuestComponent[] questComponents => GetComponentsInChildren<QuestComponent>();

    private string originalName;

    public bool isActiveSequenceQuest => sequence.currentQuest == this;

    protected virtual void Awake()
    {
        originalName = gameObject.name;
        state = QuestState.Inactive;
    }

    public virtual void OnQuestEnable()
    {
        state = QuestState.Active;
        gameObject.name = $"**{originalName}**";
        QuestInititalized?.Invoke();

        foreach (var questComponent in questComponents)
        {
            questComponent.OnQuestEnable();
        }
    }

    // Returns the deepest incomplete dependency reachable from *this* quest.
    // If everything is complete, returns null.
    Quest GetLastIncompleteDependency()
    {
        foreach (var dep in dependencies)
        {
            if (dep == null)
            {
                Debug.LogError($"Empty dependency slot found for {gameObject.name}", this);
                continue;
            }

            if (dep == this)
            {
                Debug.LogError($"Self-referencing dependency found for {gameObject.name}", this);
                continue;
            }

            if (!dep.gameObject.activeInHierarchy)
            {
                continue;
            }

            // Prefer going deeper first
            var nested = dep.GetLastIncompleteDependency();
            if (nested != null)
                return nested;

            // If nothing deeper is incomplete, consider this dep itself
            if (!dep.isDebugCompleted && !dep.IsCompleted())
                return dep;
        }
        return null;
    }

    private void PauseQuest()
    {
        OnQuestDisable();
        //We force the state to be 'WaitingForDependencies' instead of Inactive
        state = QuestState.WaitingForDependencies;
    }

    private void ContinueQuest()
    {
        currentDependency = null;
        OnQuestEnable();
    }

    protected virtual bool IsCompleted()
    {
        //Currently if ANY Quest is Completed, we consider it done
        foreach (var questComponent in questComponents)
        {
            if (questComponent is QuestObjective questGoal && questGoal.IsCompleted())
            {
                return true;
            }
        }
        return false;
    }

    private void PassIfCompleted()
    {
        if (IsCompleted())
        {
            Pass();
        }
    }

    private void Update()
    {
        if (state == QuestState.WaitingForDependencies)
        {
            if (currentDependency.IsCompleted())
            {
                ContinueQuest();
            }
        }

        if (state == QuestState.Active)
        {
            var dep = GetLastIncompleteDependency();
            if (dep != null)
            {
                PauseQuest();
                currentDependency = dep;
                currentDependency.OnQuestEnable();
                return;
            }

            foreach (var questComponent in questComponents)
            {
                questComponent.OnQuestUpdate();
            }

            PassIfCompleted();

            if (GetType() != typeof(SubSequenceQuest))
            {
#if UNITY_WEBGL
                    if (Keyboard.current.slashKey.wasPressedThisFrame)
                    {
                        Debug.Log($"Debug completing quest {name}", this);
                        Pass(true);
                    }
#endif
#if UNITY_ANDROID
                    var rightHand =
                        InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>(CommonUsages.RightHand.ToString());

                    if (rightHand != null)
                    {
                        var aButton = rightHand.TryGetChildControl<ButtonControl>("primaryButton");
                        if (aButton != null && aButton.wasPressedThisFrame)
                        {
                            Pass(true);
                        }
                    }
#endif
            }
        }
    }

    public virtual float GetProgression()
    {
        return -1;
    }

    public void Pass(bool isDebugPass = false)
    {
        if (state == QuestState.Active)
        {
            if (isDebugPass)
            {
                isDebugCompleted = true;
            }

            OnQuestDisable();
            state = QuestState.Passed;
            Complete();
        }
    }

    public void Fail(string reason)
    {
        if (state == QuestState.Active)
        {
            OnQuestDisable();
            state = QuestState.Failed;
            Complete();
        }
    }

    private void Complete()
    {
        sequence.CompleteQuest(this);
        QuestCompleted?.Invoke();
    }

    public virtual void OnQuestDisable()
    {
        StopAllCoroutines();
        gameObject.name = originalName;
        state = QuestState.Inactive;
        foreach (var questTarget in questComponents)
        {
            questTarget.OnQuestDisable();
        }
    }
}


public enum QuestState
{
    Active,
    WaitingForDependencies,
    Passed,
    Failed,
    Inactive
}