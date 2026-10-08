using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public GameObject _prefab;
    public GameObject _projectilePrefab;


    [Header("Stats")]
    public int _range;
    public int _damage;
    public float _impactRadius;
    public float _width;
    public float _speed;
    public float _cooldown;
    public int _maxCharges;
    public float _projectileCount;
    public float _projectileDelay;
    public float _impactDelay;
    public float _damageInterval;
    public float _duration;

    [Header("Normal")]
    public SpellData _normalSpell;
    [Header("Improved")]
    public SpellData _improvedSpell;
    public float _improvedDuration;
}
