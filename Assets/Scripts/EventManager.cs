
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<SkillInput> OnSkillInputReceived = new();

    public static UnityEvent OnSequenceSuccess = new();

    public static UnityEvent OnSequenceFailed = new();
}
