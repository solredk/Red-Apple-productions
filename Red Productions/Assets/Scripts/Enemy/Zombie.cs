using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private List<GameObject> players = new();

    private GameObject closestPlayer;

    [SerializeField] private NavMeshAgent agent;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;
    private float lastAttackTime;

    private void Update()
    {
        players.Clear();
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(foundPlayers);
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
        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject player in players)
        {
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
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Check of speler een health script heeft
            PlayerHealth playerHealth = closestPlayer.GetComponent<PlayerHealth>();
            if (closestPlayer != null)
            {
                playerHealth.TakeDamage(damage);
            }

            lastAttackTime = Time.time;
        }
    }
}


