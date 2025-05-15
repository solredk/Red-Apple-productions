using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TomatoLauncher : MonoBehaviour
{
    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private TomatoLauncherStats tomatoData;

    [SerializeField] private ScreenRumble screenRumble;
    [SerializeField] private ControllerRumble controllerRumble;


    [SerializeField] private float fireRate;
    private float CooldownTimer;

    public bool isShooting;

    [SerializeField] private int damageOutput;
    [SerializeField] private PlayerLook playerLook;

    public bool controllerActive;
    public Gamepad gamepad;
    private float rumbleDuration = 0.2f; // hoe lang de trilling duurt
    private float rumbleTimer;

    private void Start()
    {
        fireRate = tomatoData.fireRate;
        damageOutput = tomatoData.damageOutput;
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
        GameObject projectile =Instantiate(tomatoPrefab, launchPoint.position, launchPoint.rotation);
        projectile.GetComponent<TomatoProjectile>().DamageOutput = damageOutput;

        screenRumble.TriggerShake(.1f, 0.1f); 

        controllerRumble.StartRumble(0.5f, 0.5f, rumbleDuration);

        CooldownTimer = fireRate;
    }
}
