using System;
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
    private int _bonusDamage;
    private float _bonusMoveSpeed;
    private float _damageAugments = 1f;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _data._maxHealth + _bonusMaxHealth;
    public float MoveSpeed => _data._moveSpeed + _bonusMoveSpeed;
    public int Damage => _data._damage;
    public float DamageAugments => _damageAugments;
    public int BonusDamage => _bonusDamage;
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
        _currentHealth -= damage;

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

            case LevelUpBonusType.MoveSpeed:
                _bonusMoveSpeed += bonus._value;
                _agent.speed = MoveSpeed;
                break;

            case LevelUpBonusType.Damage:
                int damageBonus = Mathf.RoundToInt(bonus._value);
                _bonusDamage += damageBonus;
                break;
        }
    }

    public int CalculateSpellDamage(int spellDamage)
    {
        float damage = (Damage + spellDamage + BonusDamage) * DamageAugments;
        Debug.Log(damage);

        return Mathf.RoundToInt(damage);


    }
}
