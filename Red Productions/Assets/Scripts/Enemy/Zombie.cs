using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;

    [SerializeField] private Enemybehavior enemyBehavior;

    [SerializeField] private NavMeshAgent agent;
        
    [SerializeField] private Animator zombieAnimator;

    [SerializeField] private EnemyHealth enemyHealth;

    [SerializeField] private Collider zombieCollider;

    [SerializeField] private AudioSource zombieAttackSound;
    [SerializeField] private AudioSource[] zombieWalkSounds;

    private int walkIndex;

    private GameObject closestPlayer;

    private List<GameObject> players = new List<GameObject>();

    private float lastAttackTime;
    private float counter;

    private void Awake()
    {
        walkIndex = Random.Range(0, zombieWalkSounds.Length);
        zombieCollider.enabled = false;
        agent.updateRotation = true;

        // Find all players in the scene and add them to the list
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(foundPlayers);
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        agent.speed = enemyBehavior.speed;
        attackCooldown = enemyBehavior.attackCooldown;
        damage = enemyBehavior.damage;
    }

    private void Update()
    {
        if (enemyHealth.isDead)
        {
            zombieWalkSounds[walkIndex].Stop();
            agent.isStopped = true;
            return; 
        }

        float movementSpeed = agent.velocity.magnitude;

        zombieAnimator.SetFloat("speed", movementSpeed);        
        
        if (!zombieCollider.enabled )
        {
            counter += Time.deltaTime;
            if (counter >= 2f) 
            {
                zombieWalkSounds[walkIndex].Stop();
                zombieCollider.enabled = true;
                counter = 0f; 
            }
        }

        if (closestPlayer != null && agent != null)
        {
            float distance = Vector3.Distance(transform.position, closestPlayer.transform.position);

            if (distance <= attackRange)
            {
                zombieWalkSounds[walkIndex].Play();
                agent.isStopped = true;
                AttackPlayer();
            }
            else
            {
                zombieWalkSounds[walkIndex].Stop();
                agent.isStopped = false;
                agent.SetDestination(closestPlayer.transform.position);
            }
        }

        FindClosestPlayer();
    }

    private void FindClosestPlayer()
    {
        if (enemyHealth.isDead) return;

        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject player in players)
        {
            PlayerState state = player.GetComponent<PlayerHealth>().playerState;
            if (state == PlayerState.dead)
            {
                continue; 
            }

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = player;
            }
        }

        closestPlayer = nearest;
    }

    private void AttackPlayer()
    {
        if (enemyHealth.isDead) return;
        zombieAttackSound.Play();
        zombieAnimator.SetTrigger("attack");
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayerHealth playerHealth = closestPlayer.GetComponent<PlayerHealth>();
            if (closestPlayer != null)
            {
                playerHealth.TakeDamage(damage);

            }

            lastAttackTime = Time.time;
        }
    }
}


