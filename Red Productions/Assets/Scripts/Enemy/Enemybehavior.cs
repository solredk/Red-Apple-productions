using UnityEngine;

[CreateAssetMenu(fileName = "Enemybehavior", menuName = "Scriptable Objects/Enemybehavior")]
public class Enemybehavior : ScriptableObject
{
    public int currentWave;
    public int damage;
    public int maxhealth;
    public float attackCooldown;
    public float speed;
}
