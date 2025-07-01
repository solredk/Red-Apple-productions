using UnityEngine;
using UnityEngine.InputSystem;

public class TomatoLauncher : MonoBehaviour
{
    [Header("Tomato Launcher")]
    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private TomatoLauncherStats tomatoData;
    
    [Header("rumble")]
    [SerializeField] private ScreenRumble screenRumble;
    [SerializeField] private ControllerRumble controllerRumble;

    [SerializeField] private Animator animator;

    public Gamepad gamepad;

    public bool isShooting;
    public bool controllerActive;

    private  float rumbleDuration = 0.2f; 

    private float fireRate;
    private int damage;

    private float CooldownTimer;

    private bool canShoot = true;


    private void Start()
    {
        //getting the stats to the launcher from the scriptable object
        fireRate = tomatoData.fireRate;
        damage = tomatoData.damageOutput;
    }

    private void Update()
    {
        if (!canShoot)
            return;

        //shoot after the cooldown
        if (isShooting && CooldownTimer <= 0)
        {
            animator.SetTrigger("Shoot");
            Shoot();
        }

        //counting down the cooldown timer
        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        AudioManager.Instance.PlayShootTomato();

        //instantiate the tomato prefab at the launch point with the rotation of the launch point and putting it in an variabel
        GameObject projectile = Instantiate(tomatoPrefab, launchPoint.position, launchPoint.rotation);
        TomatoProjectile projectileComponent = projectile.GetComponent<TomatoProjectile>();

        //setting the stats to the projectile from the launcher with the criptable object
        projectileComponent.DamageOutput = damage;

        //shake the screen
        screenRumble.TriggerShake(0.1f, 0.1f);

        if (controllerActive && gamepad != null)
        {
            controllerRumble.StartRumble(0.5f, 0.5f, rumbleDuration, gamepad);
        }

        CooldownTimer = fireRate;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }

    public void DisableShooting()
    {
        canShoot = false;
    }

}
