using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] bool m_selected = false;
    [SerializeField] private string selectableTag = "Item";
    [SerializeField] public Material highlightMaterial;

    private Material originalMaterial;
    private Transform currentSelection;

    void Start()
    {

    }

    void Update()
    {
  

    }
        public void HighLight()
        {
            if (currentSelection != null)
            {
                Renderer renderer = currentSelection.GetComponent<Renderer>();
                if (renderer != null)
                {
                m_selected = true;
                    renderer.material = originalMaterial;
                }
                currentSelection = null;
                originalMaterial = null;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform selection = hit.transform;


                if (selection.CompareTag(selectableTag))
                {
                    Renderer selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {

                    originalMaterial = selectionRenderer.material;
                    selectionRenderer.material = highlightMaterial;
                    currentSelection = selection;
                    m_selected = false;

                }
            }
        }
        }
    }


