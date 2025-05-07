using UnityEngine;

public class PickupAndDrop : MonoBehaviour
{
    public GameObject Camera;
    float maxPickupDistance = 5;
    GameObject itemHolding;
    bool isHolding = false;
    public float holdDistance = 2.5f;

    public float verticalOffsetY = 0f; // You can tweak this in Inspector
    public float maxDownY = -0.5f;     // Lowest Y offset allowed

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            Pickup();
        }
        if (Input.GetKeyDown("q"))
        {
            Drop();
        }

    }

    void Pickup()
    {
        if (isHolding || itemHolding != null) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, maxPickupDistance))
        {
            if (hit.transform.tag == "Item")
            {
                itemHolding = hit.transform.gameObject;

                foreach (var c in hit.transform.GetComponentsInChildren<Collider>()) if (c != null) c.enabled = false;
                foreach (var r in hit.transform.GetComponentsInChildren<Rigidbody>()) if (r != null) r.isKinematic = true;

                itemHolding.transform.SetParent(Camera.transform);

                verticalOffsetY = Mathf.Clamp(verticalOffsetY, maxDownY, 1f);
                itemHolding.transform.localPosition = new Vector3(0, verticalOffsetY, holdDistance);
                itemHolding.transform.localRotation = Quaternion.identity;

                isHolding = true;
            }
        }
    }

    void Drop()
    {
        if (!isHolding || itemHolding == null) return;

        itemHolding.transform.SetParent(null);

        foreach (var c in itemHolding.transform.GetComponentsInChildren<Collider>()) if (c != null) c.enabled = true;
        foreach (var r in itemHolding.transform.GetComponentsInChildren<Rigidbody>()) if (r != null) r.isKinematic = false;

        RaycastHit hitDown;
        Physics.Raycast(transform.position, -Vector3.up, out hitDown);
        itemHolding.transform.position = hitDown.point + new Vector3(transform.forward.x, 0, transform.forward.z);

        itemHolding = null;
        isHolding = false;

    }
}