using System.Runtime.CompilerServices;
using UnityEngine;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;

    void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }


    void Update()
    {
        
    }
}
