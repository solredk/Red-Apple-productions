using UnityEngine;

public class PickupAndDrop : MonoBehaviour
{
    [SerializeField] public float scrollSpeed = 1f; 

    private Camera Camera;

    private float verticalOffsetY = 0f;
    private float maxDownY = -0.5f;
    private float maxPickupDistance = 5;
    private float holdDistance = 2.5f;
    private float minHoldDistance = 0.5f;

    private GameObject itemHolding;

    private bool isHolding = false;


    void Start()
    {
        if (Camera == null)
            Camera = GetComponentInChildren<Camera>();
    }

    public void Pickuppp()
    {
        if (isHolding || itemHolding != null) return;

        RaycastHit hit;
        Debug.DrawRay(Camera.transform.position, Camera.transform.forward * maxPickupDistance, Color.red, 1f);

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, maxPickupDistance))
        {
            Debug.Log(hit, transform.gameObject);

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
     
        RaycastHit hitDown;
        Vector3 dropPosition = transform.position + transform.forward * holdDistance; 

        if (Physics.Raycast(dropPosition, -Vector3.up, out hitDown))
            itemHolding.transform.position = hitDown.point; 

        else
            itemHolding.transform.position = dropPosition;

        itemHolding = null;
        isHolding = false;
    }

    public void AdjustHoldDistance(float scrollDelta)
    {
        if (isHolding && itemHolding != null)
        {
            holdDistance += scrollDelta * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, 10f);
            itemHolding.transform.localPosition = new Vector3(0, verticalOffsetY, holdDistance);
        }
    }
}