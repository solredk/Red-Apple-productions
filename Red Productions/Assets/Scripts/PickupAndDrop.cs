using UnityEngine;

public class PickupAndDrop : MonoBehaviour
{


    public GameObject Camera;
    float maxPickupDistance = 5;
    GameObject itemHolding;
    bool isHolding = false;

    void Start()
    {

    }

    // Update is called once per frame
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
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, maxPickupDistance))
        {
            if (hit.transform.tag == "Item")
            {
                itemHolding = hit.transform.gameObject;
                foreach (var c in hit.transform.GetComponentsInChildren<Collider>()) if (c != null) c.enabled = false;
                foreach (var r in hit.transform.GetComponentsInChildren<Rigidbody>()) if (r != null) r.isKinematic = true;
                itemHolding.transform.parent = transform;
                itemHolding.transform.localPosition = Vector3.zero;
                itemHolding.transform.localEulerAngles = Vector3.zero;

                isHolding = true;
            }
        }

    }

    void Drop()
    {
        itemHolding.transform.parent = null;
        foreach (var c in itemHolding.transform.GetComponentsInChildren<Collider>()) if (c != null) { c.enabled = true; }
        foreach (var r in itemHolding.transform.GetComponentsInChildren<Rigidbody>()) if (r != null) { r.isKinematic = false; }
        RaycastHit hitDown;
        Physics.Raycast(transform.position, -Vector3.up, out hitDown);
        itemHolding.transform.position = hitDown.point + new Vector3(transform.forward.x, 0, transform.forward.z);
    }
}


