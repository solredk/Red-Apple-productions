using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TestSmth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [SerializeField] GameObject prefabTest;
   [SerializeField] Transform spawnpoint;



    void Start()
    {
        StartCoroutine(SpawnTest());

    }


    void Update()
    {

    }

    IEnumerator SpawnTest()
    {
        yield return new WaitForSeconds(5);

        Instantiate(prefabTest, spawnpoint.position, Quaternion.identity);   
    }

}
