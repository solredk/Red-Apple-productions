using UnityEngine;
using TMPro;
using System.Collections;

public class TestGenerate : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;

    void Start()
    {
        StartCoroutine(Generate());
    }

    void Update()
    {
    }

    IEnumerator Generate()
    {
        while (true)
        {
           
            GameObject popup = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity);
            TextMeshProUGUI temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            temp.text = Random.Range(0, 100).ToString();
            Destroy(popup, 3f);
            yield return new WaitForSeconds(5f);
        }
    }
}