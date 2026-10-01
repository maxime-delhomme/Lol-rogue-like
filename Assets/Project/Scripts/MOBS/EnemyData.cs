using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Reference")]
    public EnemyRarity _rarity;
    public GameObject _prefab;

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

    public enum EnemyRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
