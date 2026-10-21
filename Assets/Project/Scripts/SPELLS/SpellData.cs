using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public GameObject _prefab;
    public GameObject _projectilePrefab;

    [Header("Properties")]

    public SpellTypes spellTypes;

    [System.Flags]
    public enum SpellTypes
    {
        None = 0,
        Projectile = 1 << 1,  // Équivaut à 1
        Area = 1 << 2,    // Équivaut à 2
        Staff = 1 << 3,  // Équivaut à 4
        Shield = 1 << 4 // Équivaut à 8
    }

    public int numberOfClickNecessary = 1;
    private int numberOfClick = 0;

    public Vector3[] ClicksPositions;


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
    public int _penetration;
    public bool _BounceOnWalls;


    [Header("Normal")]
    public SpellData _normalSpell;
    [Header("Improved")]
    public SpellData _improvedSpell;
    public float _improvedDuration;

    private void Awake()
    {
        ClicksPositions = new Vector3[numberOfClickNecessary];
    }

    public bool  OnSpellCast(bool isCast)
    {
        numberOfClickNecessary ++;
        if (numberOfClick > numberOfClickNecessary)
        {

        }
        Debug.Log("Q casted");
        Character.CharacterCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        GameObject spellObject = Instantiate(_prefab, Character.CurrentCharacter.transform.position, Quaternion.identity);
        return true;
    }
}
