using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public int _health;
    public float _attackRange;
    public int _damage;
    public float _attackDelay;
    public float _speedRotation;
    public float _moveSpeed;

    [Header("Drop")]
    public int _rage;
    public GameObject _expOrbPrefab;
    public int _expOrbCount;
}
