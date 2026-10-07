using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
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



    [Header("ViewPublic")]
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _data._maxHealth;
    public float LifeSteal => _data._lifeSteal;
    public int Armor => _data._armor;
    public int Damage => _data._damage;
    public float DamageAugments => 1f;
    public float CriticalChance => _data._critChance;
    public float CriticalDamage => _data._critDamage;
    public int Range => _data._range;
    public float AbilityHaste => _data._abilityHaste;
    public float MoveSpeed => _data._moveSpeed;
    public float Luck => _data._luck;
    public float XpGain => _data._xpGain;
    public int AttractionRange => _data._attractionRange;
    public int CurrentRage => _currentRage;
    public int MaxRage => _data._maxRage;
    public int CurrentExperience => _currentExperience;
    public int ExperienceRequired => _experienceRequired;
    public int CurrentLevel => _level;


    private void Awake()
    {

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

            _experienceRequired = CalculateExperienceRequired(_level);
        }
    }

    public int CalculateSpellDamage(int spellDamage)
    {
        float damage = (Damage + spellDamage) * DamageAugments;

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
