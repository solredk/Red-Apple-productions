using UnityEngine;

public class Ammo : Interactable
{
    [SerializeField] Material material;
    protected override void Interact(GameObject playerGameObject)
    {
        base.Interact(playerGameObject);
        if (GetComponent<Renderer>().material != material) 
        {
            GetComponent<Renderer>().material = material;
            useEvents = false;
            promptMessage = "empty";
        }
    }
}
