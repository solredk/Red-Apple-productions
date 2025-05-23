using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        base.OnInspectorGUI();
        if (interactable.useEvents)
        {
            if (interactable.gameObject.GetComponent<InteractionEvent>() == null)
                interactable.gameObject.AddComponent<InteractionEvent>();
        }
        else
        {
             if (interactable.gameObject.GetComponent<InteractionEvent>() != null)
                DestroyImmediate(interactable.gameObject.GetComponent<InteractionEvent>());
        }
    }

}
