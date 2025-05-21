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
    [SerializeField] private int playerIndex;

    public Gamepad gamepad;

    public bool isShooting;
    public bool controllerActive;

    private  float rumbleDuration = 0.2f; 
    private float fireRate;
    private int damage;

    private float CooldownTimer;

    private void Start()
    {
        //getting the stats to the launcher from the scriptable object
        fireRate = tomatoData.fireRate;
        damage = tomatoData.damageOutput;
    }

    private void Update()
    {
        //shoot after the cooldown
        if (isShooting && CooldownTimer <= 0)
            Shoot();

        //counting down the cooldown timer
        if (CooldownTimer > 0)
            CooldownTimer -= Time.deltaTime;
    }

    private void Shoot()
    {
        //instantiate the tomato prefab at the launch point with the rotation of the launch point and putting it in an variabel
        GameObject projectile = Instantiate(tomatoPrefab, launchPoint.position, launchPoint.rotation);
        TomatoProjectile projectileComponent = projectile.GetComponent<TomatoProjectile>();

        //setting the stats to the projectile from the launcher with the criptable object
        projectileComponent.DamageOutput = damage;
        projectileComponent.playerIndex = playerIndex;

        //shake the screen
        screenRumble.TriggerShake(.1f, 0.1f);
        
        if (controllerActive && gamepad != null)
            // Start rumble on the controller
            controllerRumble.StartRumble(0.5f, 0.5f, rumbleDuration, gamepad);

        //reseting the cooldown with tfire rate from the stats
        CooldownTimer = fireRate;
    }
}
