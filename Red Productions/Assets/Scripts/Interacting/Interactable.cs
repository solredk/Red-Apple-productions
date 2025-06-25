using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    public string promptMassage;

    public void BaseInteract()
    {
        if (useEvents)
        {
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        }
        Interact();
    }


    protected virtual void Interact()
    {
        
    }
}
