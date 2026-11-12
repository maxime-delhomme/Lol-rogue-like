using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera _camera;
    public static Camera CharacterCamera;
    public static Character CurrentCharacter;
    [SerializeField] private SpellData _dontTouchMeSpellData;
    private QTEManager _qteManager;
    private SpellCaster _spellCaster;

    [Header("Input")]
    [SerializeField] private InputActionAsset InputActions;
    private InputAction m_moveAction;
    private InputAction m_InteractionAction;


    [Header("Move")]
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;
    private bool _isDashing;
    private Quaternion _targetRotation;


    [Header("Interaction")]
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private LayerMask _interactionLayer;
    private Outline _currentOutline;


    [Header("Data")]
    [SerializeField] private CharacterData _data;
    private int _currentHealth;
    private int _bonusMaxHealth = 0;
    private int _currentRage;
    private int _currentGold;
    private int _currentExperience = 0;
    private int _experienceRequired;
    private int _level = 1;
    private int _skillPoints = 100;
    private float _damageGrowth = 0.1f;
    private float _healthGrowth = 0.02f;

    [Header("AugmentBool")]
    private bool _dontTouchMeActif = false;

    [Header("Augment")]
    [SerializeField] private float _qteMoveSpeedMultiplier;
    [SerializeField] private float _qteMoveSpeedDuration;
    private float _currentMoveSpeedMultiplier = 1f;
    private Coroutine _qteMoveSpeedCoroutine;

    #region pointers
    [Header("ViewPublic")]
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => Mathf.RoundToInt(_data._maxHealth * (1f + _healthGrowth * (_level - 1) * (_level - 1)));
    public float LifeSteal => _data._lifeSteal;
    public int Armor => _data._armor;
    public int Damage => Mathf.RoundToInt(_data._damage * (1f + _damageGrowth * (_level - 1) * (_level - 1)));
    public float DamageAugments => 1f;
    public float CriticalChance => _data._critChance;
    public float CriticalDamage => _data._critDamage;
    public int Range => _data._range;
    public float AbilityHaste => _data._abilityHaste;
    public float MoveSpeed => _data._moveSpeed * _currentMoveSpeedMultiplier;
    public float XpGain => _data._xpGain;
    public int AttractionRange => _data._attractionRange;
    public int CurrentRage => _currentRage;
    public int MaxRage => _data._maxRage;
    public int CurrentGold => _currentGold;
    public int CurrentExperience => _currentExperience;
    public int ExperienceRequired => _experienceRequired;
    public int CurrentLevel => _level;
    public int SkillPoints => _skillPoints;
    public bool IsDashing => _isDashing;
    #endregion
    public static event Action<Vector3> OnPointerClick;

    private void Awake()
    {
        if (!CurrentCharacter)
            CurrentCharacter = this;

        if (_camera == null)
        {
            _camera = Camera.main;
            CharacterCamera = _camera;

        }

        _rigidbody = GetComponent<Rigidbody>();
        _targetRotation = _rigidbody.rotation;

        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");
        m_InteractionAction = InputActions.FindActionMap("Player").FindAction("Interaction");

        _currentHealth = _data._maxHealth;
        _experienceRequired = CalculateExperienceRequired(_level);

        _spellCaster = GetComponent<SpellCaster>();
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    public void SetQTEManager(QTEManager qteManager)
    {
        _qteManager = qteManager;
    }

    private void Update()
    {
        Vector2 moveInput = m_moveAction.ReadValue<Vector2>();

        _moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (_moveDirection.sqrMagnitude > 0.01f && !_isDashing)
        {
            _targetRotation = Quaternion.LookRotation(_moveDirection);
        }

        if (m_InteractionAction.WasPressedThisFrame())
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactionLayer))
                return;

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable == null)
                return;

            float distance = Vector3.Distance(transform.position, hit.collider.transform.position);

            if (distance > _interactionRange)
                return;

            interactable.Interact();
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing)
            return;

        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, _targetRotation, Time.fixedDeltaTime * _data._lookRotationSpeed));

        Vector3 movement = _moveDirection * MoveSpeed;

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x = movement.x;
        velocity.z = movement.z;

        _rigidbody.linearVelocity = velocity;
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

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        _currentGold += amount;
        //Debug.Log($"Or : {_currentGold}");
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(1, damage - Armor);

        _currentHealth -= damageTaken;

        if(_currentHealth <= 0)
        {
            Death();
        }

        if (_dontTouchMeActif)
        {
            Debug.Log("aie");
            int spellIndex = _spellCaster.GetSpellIndex(_dontTouchMeSpellData);

            if (spellIndex >= 0)
            {
                _spellCaster.SetTargetPosition(transform.position);
                _qteManager.DemarrerQTE(_spellCaster, spellIndex);
            }
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
            int previousMaxHealth = MaxHealth;

            _level++;

            int newMaxHealth = MaxHealth;
            int healthIncrease = newMaxHealth - previousMaxHealth;

            _currentHealth += healthIncrease;

            _skillPoints++;

            _experienceRequired = CalculateExperienceRequired(_level);
        }
    }

    public bool SpendSkillPoints(int amount)
    {
        if (amount > 0 && _skillPoints >= amount)
        {
            _skillPoints -= amount;
            return true;
        }

        return false;
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

    public void AddMaxHealth(int amount)
    {
        if (amount <= 0)
            return;

        _bonusMaxHealth += amount;
        _currentHealth += amount;
    }

    public void StartDash()
    {
        _isDashing = true;
    }

    public void EndDash()
    {
        _isDashing = false;
    }

    public void ActivateQteMoveSpeedAugment()
    {
        if (_qteMoveSpeedCoroutine != null)
            StopCoroutine(_qteMoveSpeedCoroutine );

        _currentMoveSpeedMultiplier = _qteMoveSpeedMultiplier;

        _qteMoveSpeedCoroutine = StartCoroutine(QteMoveSpeedCoroutine());
    }

    private IEnumerator  QteMoveSpeedCoroutine()
    {
        yield return new WaitForSeconds(_qteMoveSpeedDuration);

        _currentMoveSpeedMultiplier = 1f;
        _qteMoveSpeedCoroutine = null;
    }

    public void ActiverDontTouchMe()
    {
        _dontTouchMeActif = true;
    }

    public void DesactiverDontTouchMe()
    {
        _dontTouchMeActif = false;
    }
}
