using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("Reference")]
    private LevelUpManager _levelUpManager;

    [Header("Input")]
    [SerializeField] private InputActionAsset InputActions;
    private InputAction m_moveAction;

    [Header("Move")]
    NavMeshAgent _agent;
    [SerializeField] LayerMask _clickableLayers;

    [Header("Data")]
    [SerializeField] private CharacterData _data;
    private int _currentHealth;
    private int _currentRage;
    private int _currentExperience = 0;
    private int _experienceRequired;
    private int _level = 1;
    private int _pendingLevelUps;

    [Header("BonusStats")]
    private int _bonusMaxHealth;
    private float _bonusLifeSteal;
    private int _bonusArmor;
    private int _bonusDamage;
    private float _bonusCriticalChance;
    private float _bonusCriticalDamage;
    private float _damageAugments = 1f;
    private int _bonusRange;
    private float _bonusAbilityHaste;
    private float _bonusMoveSpeed;
    private float _bonusLuck;
    private float _bonusXpGain;
    private int _bonusAttractionRange;



    [Header("ViewPublic")]
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _data._maxHealth + _bonusMaxHealth;
    public float LifeSteal => _data._lifeSteal + _bonusLifeSteal;
    public int Armor => _data._armor + _bonusArmor;
    public int Damage => _data._damage;
    public float DamageAugments => _damageAugments;
    public int BonusDamage => _bonusDamage;
    public float CriticalChance => _data._critChance + _bonusCriticalChance;
    public float CriticalDamage => _data._critDamage + _bonusCriticalDamage;
    public int Range => _data._range + _bonusRange;
    public float AbilityHaste => _data._abilityHaste + _bonusAbilityHaste;
    public float MoveSpeed => _data._moveSpeed + _bonusMoveSpeed;
    public float Luck => _data._luck + _bonusLuck;
    public float XpGain => _data._xpGain + _bonusXpGain;
    public int AttractionRange => _data._attractionRange + _bonusAttractionRange;
    public int CurrentRage => _currentRage;
    public int MaxRage => _data._maxRage;
    public int CurrentExperience => _currentExperience;
    public int ExperienceRequired => _experienceRequired;
    public int CurrentLevel => _level;
    public int PendingLevelUps => _pendingLevelUps;


    private void Awake()
    {
        _levelUpManager = GetComponent<LevelUpManager>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _data._moveSpeed;

        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");

        _currentHealth = _data._maxHealth;
        _experienceRequired = CalculateExperienceRequired(_level);
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        if(m_moveAction.WasPressedThisFrame())
        {
            ClickToMove();
        }

        FaceTarget();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100, _clickableLayers))
        {
            _agent.destination = hit.point;
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = _agent.destination - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _data._lookRotationSpeed);
        }
    }

    public bool PeutFaireQTE()
    {
        return _currentRage >= _data._maxRage;
    }

    public void AjouterRage(int montant)
    {
        _currentRage += montant;

        if(_currentRage > _data._maxRage)
        {
            _currentRage = _data._maxRage;
        }
    }

    public void ConsommerRage()
    {
        _currentRage -= _data._maxRage;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(1, damage - Armor);

        _currentHealth -= damageTaken;

        if(_currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log("Le joueur est mort");
    }

    private int CalculateExperienceRequired(int level)
    {
        float experience = _data._baseExperience * Mathf.Pow(level, _data._experienceExponent) + _data._experienceLinearBonus * level;
        return Mathf.RoundToInt(experience);
    }

    public void GainExperience(int amount)
    {
        amount = CalculateExprienceGain(amount);

        _currentExperience += amount;

        _experienceRequired = CalculateExperienceRequired(_level);

        while (_currentExperience >= _experienceRequired)
        {
            _currentExperience -= _experienceRequired;
            _level++;

            _pendingLevelUps++;

            _experienceRequired = CalculateExperienceRequired(_level);
        }

        if (_pendingLevelUps > 0)
        {
            _levelUpManager.StartLevelUp();
        }
    }

    public void ConsumePendingLevelUp()
    {
        _pendingLevelUps--;
    }


    public void ApplyBonus(LevelUpBonusData bonus)
    {
        switch (bonus._type)
        {
            case LevelUpBonusType.MaxHealth:
                _bonusMaxHealth += Mathf.RoundToInt(bonus._value);
                _currentHealth += Mathf.RoundToInt(bonus._value);
                break;

            case LevelUpBonusType.LifeSteal:
                _bonusLifeSteal += bonus._value;
                break;

            case LevelUpBonusType.Armor:
                int armorBonus = Mathf.RoundToInt(bonus._value);

                _bonusArmor += armorBonus;
                break;

            case LevelUpBonusType.Damage:
                int damageBonus = Mathf.RoundToInt(bonus._value);
                _bonusDamage += damageBonus;
                break;

            case LevelUpBonusType.CriticalChance:
                _bonusCriticalChance += bonus._value;
                break;

            case LevelUpBonusType.Range:
                int rangeBonus = Mathf.RoundToInt(bonus._value);

                _bonusRange += rangeBonus;
                break;

            case LevelUpBonusType.AbilityHaste:
                _bonusAbilityHaste += bonus._value;
                break;

            case LevelUpBonusType.MoveSpeed:
                _bonusMoveSpeed += bonus._value;
                _agent.speed = MoveSpeed;
                break;

            case LevelUpBonusType.Luck:
                _bonusLuck += bonus._value;
                break;

            case LevelUpBonusType.xpGain:
                _bonusXpGain += bonus._value;
                break;

            case LevelUpBonusType.AttractionRange:
                int attractionRangeBonus = Mathf.RoundToInt(bonus._value);
                _bonusAttractionRange += attractionRangeBonus;
                break;
        }
    }

    public int CalculateSpellDamage(int spellDamage)
    {
        float damage = (Damage + spellDamage + BonusDamage) * DamageAugments;

        bool isCritical = UnityEngine.Random.Range(0f, 100f) < CriticalChance;

        if (isCritical)
        {
            damage *= 1f + (CriticalDamage / 100f);
        }

        return Mathf.RoundToInt(damage);
    }

    public int CalculateSpellRange(int spellRange)
    {
        return Range + spellRange;
    }

    public float CalculateSpellCooldown(float spellCooldown)
    {
        return spellCooldown / (1f + AbilityHaste / 100f);
    }

    public int CalculateExprienceGain(int amount)
    {
        float experience = amount * (1f + XpGain / 100f);

        return Mathf.RoundToInt(experience);
    }

    public void ApplyLifeSteal(int damage)
    {
        int healAmount = Mathf.RoundToInt(damage * LifeSteal / 100f);

        Heal(healAmount);
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;

        if (_currentHealth > MaxHealth)
        {
            _currentHealth = MaxHealth;
        }
    }
}
