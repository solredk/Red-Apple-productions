using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class HighLight : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Color color = Color.white;
    private List<Material> materials;

    private void Awake()
    {
        materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(new List<Material>(renderer.materials));

        }
    }

    public void ToggleHighLight(bool val)

    {
        if (val)
        {
            foreach (Material material in materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);

            }

        }

        else
        {
            foreach (Material material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
