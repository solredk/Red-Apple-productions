using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    
    public bool useEvents;
    public string promptMessage;

    public void BaseInteract(GameObject playerGameObject)
    {
        if (useEvents && TryGetComponent(out InteractionEvent interactionEvent))
        {
            interactionEvent.OnInteract.Invoke();
        }
        Interact(playerGameObject);
    }


    protected virtual void Interact(GameObject playerGameObject)
    {
        
    }
}
