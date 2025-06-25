using UnityEngine;

public class Ammo : Interactable
{
    [SerializeField] Material material;
    protected override void Interact()
    {
        base.Interact();
        if (GetComponent<Renderer>().material != material) 
        {
            GetComponent<Renderer>().material = material;
            useEvents = false;
            promptMassage = "empty";
        }
    }
}
