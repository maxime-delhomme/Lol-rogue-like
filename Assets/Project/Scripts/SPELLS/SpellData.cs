using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    [Header("Stats")]
    public int _damage;
    public float _speed;
    public float _impactRadius;
    public float _range;
}
