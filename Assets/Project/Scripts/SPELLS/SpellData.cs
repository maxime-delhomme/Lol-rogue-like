using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public GameObject _prefab;
    public GameObject _projectilePrefab;


    [Header("Stats")]
    public int _damage;
    public float _speed;
    public float _impactRadius;
    public int _range;
    public float _cooldown;
    public float _projectileCount;
    public float _projectileDelay;

    [Header("Improved")]
    public SpellData _improvedSpell;
}
