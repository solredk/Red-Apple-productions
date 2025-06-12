using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;


    [SerializeField] private bool enteredlevel;

    [SerializeField] private Enemybehavior enemyBehavior;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Animator zombieAnimator;

    [SerializeField] private EnemyHealth enemyHealth;

    [SerializeField] private Collider zombieCollider;

    float counter;
    private float lastAttackTime;

    private GameObject closestPlayer;

    private List<GameObject> players = new List<GameObject>();


    private void Awake()
    {
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
        if (!zombieCollider.enabled )
        {
            counter += Time.deltaTime;
            if (counter >= 2f) 
            {
                zombieCollider.enabled = true;
                counter = 0f; 
            }
        }
        if (enemyHealth.isDead)
        {
            agent.isStopped = true;
            return; 
        }

        float movementSpeed = agent.velocity.magnitude;
        zombieAnimator.SetFloat("speed", movementSpeed);

        FindClosestPlayer();


        if (closestPlayer != null && agent != null)
        {
            float distance = Vector3.Distance(transform.position, closestPlayer.transform.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                AttackPlayer();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(closestPlayer.transform.position);
            }
        }
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
        zombieAnimator.SetTrigger("attack");
        if (Time.time - lastAttackTime >= attackCooldown)
        {

            // Check of speler een health script heeft
            PlayerHealth playerHealth = closestPlayer.GetComponent<PlayerHealth>();
            //if the closestplayer is not null, then make the player take damage
            if (closestPlayer != null)
            {
                playerHealth.TakeDamage(damage);
            }

            lastAttackTime = Time.time;
        }
    }
}


