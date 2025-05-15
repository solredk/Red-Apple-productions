using UnityEngine;
using UnityEngine.InputSystem;


public class TomatoLauncher : MonoBehaviour
{
    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private TomatoLauncherStats tomatoData;

    [SerializeField] private ScreenRumble screenRumble;
    [SerializeField] private ControllerRumble controllerRumble;


    [SerializeField] private float fireRate;
    private float CooldownTimer;

    public Gamepad gamepad;

    public bool isShooting;

    [SerializeField] private int damage;

    private float rumbleDuration = 0.2f; 


    private void Start()
    {
        fireRate = tomatoData.fireRate;
        damage = tomatoData.damageOutput;
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
        tomatoData.damageOutput = damage += 1;
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(tomatoPrefab, launchPoint.position, launchPoint.rotation);
        projectile.GetComponent<TomatoProjectile>().DamageOutput = damage;

        screenRumble.TriggerShake(.1f, 0.1f);
        if (gamepad != null)
        {
            Debug.Log(gamepad);
            controllerRumble.StartRumble(0.5f, 0.5f, rumbleDuration, gamepad);
        }

        CooldownTimer = fireRate;
    }
}
