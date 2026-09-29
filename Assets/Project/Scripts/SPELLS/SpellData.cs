using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public GameObject _prefab;


    [Header("Stats")]
    public int _damage;
    public float _speed;
    public float _impactRadius;
    public float _range;
    public float _cooldown;
}
