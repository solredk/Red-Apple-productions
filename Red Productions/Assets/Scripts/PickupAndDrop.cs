using UnityEngine;

public class PickupAndDrop : MonoBehaviour
{
    public GameObject Camera;
    public float maxPickupDistance = 5;
    public float holdDistance = 2.5f;
    public float verticalOffsetY = 0f;
    public float maxDownY = -0.5f;
    public float scrollSpeed = 1f; // Adjust this to control scroll speed

    private GameObject itemHolding;
    private bool isHolding = false;

    void Start()
    {
        if (Camera == null)
        {
            Camera = GetComponentInChildren<Camera>(true)?.gameObject;
        }
    }

    public void Pickup()
    {
        if (isHolding || itemHolding != null) return;

        RaycastHit hit;
        Debug.DrawRay(Camera.transform.position, Camera.transform.forward * maxPickupDistance, Color.red, 1f);

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, maxPickupDistance))
        {
            Debug.Log("Raycast Hit");

            if (hit.transform.CompareTag("Item"))
            {
                itemHolding = hit.transform.gameObject;

 

                itemHolding.transform.SetParent(Camera.transform);

                verticalOffsetY = Mathf.Clamp(verticalOffsetY, maxDownY, 1f);
                itemHolding.transform.localPosition = new Vector3(0, verticalOffsetY, holdDistance);
                itemHolding.transform.localRotation = Quaternion.identity;

                isHolding = true;
            }
        }
    }

    public void Drop()
    {
        if (!isHolding || itemHolding == null) return;

        itemHolding.transform.SetParent(null);



        // Project the item onto the ground
        RaycastHit hitDown;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitDown))
        {
            itemHolding.transform.position = hitDown.point + new Vector3(transform.forward.x, 0, transform.forward.z);
        }
        else
        {

            itemHolding.transform.position = transform.position + transform.forward * holdDistance;
        }

        itemHolding = null;
        isHolding = false;
    }


    public void AdjustHoldDistance(float scrollDelta)
    {
        if (isHolding && itemHolding != null)
        {
            holdDistance += scrollDelta * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, 0.5f, 10f); 
            itemHolding.transform.localPosition = new Vector3(0, verticalOffsetY, holdDistance);
        }
    }
}