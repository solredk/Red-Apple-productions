using UnityEngine;
using UnityEngine.Rendering;

public class TomatoLauncher : MonoBehaviour
{
    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private TomatoLauncherStats tomatoData;

    
    [SerializeField] private float fireRate;
    private float CooldownTimer;

    public bool isShooting;

    [SerializeField] private int damageOutput;

    private void Start()
    {
        fireRate = tomatoData.fireRate;
       // damageOutput = tomatoData.damageOutput;
    }

    private void Update()
    {
        if (isShooting && CooldownTimer <= 0)
        {
            Shoot();
        }

        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
        }
    }

    private void Upgrade()
    {
        tomatoData.fireRate = fireRate += 1;
        tomatoData.damageOutput = damageOutput += 1;
    }
    private void Shoot()
    {
        Instantiate(tomatoPrefab, launchPoint.position, launchPoint.rotation);
        CooldownTimer = fireRate;
        Debug.Log (CooldownTimer);
    }
}
