using System;
using Unity.VisualScripting;
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
    [SerializeField] LayerMask clickableLayers;
    float _lookRotationSpeed = 8f;

    [Header("Statistiques")]
    [SerializeField] private int _maxHealth;
    private int _currentHealth;
    [SerializeField] private int _maxRage;
    private int _currentRage = 0;
    [SerializeField] private int _experience = 0;
    [SerializeField] private int _maxExperience;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public int CurrentRage => _currentRage;
    public int MaxRage => _maxRage;
    public int CurrentExperience => _experience;
    public int MaxExperience => _maxExperience;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");

        _currentHealth = _maxHealth;
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100, clickableLayers))
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

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lookRotationSpeed);
        }
    }

    public void AjouterRage(int montant)
    {
        _currentRage += montant;

        if(_currentRage > _maxRage)
        {
            _currentRage = _maxRage;
        }
    }

    public bool PeutFaireQTE()
    {
        return _currentRage >= _maxRage;
    }

    public void ConsommerRage()
    {
        _currentRage -= _maxRage;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if(_maxHealth <= 0)
        {
            Death();
        }
    }

    public void GainExperience(int amount)
    {
        _experience += amount;

        if (_experience > _maxExperience)
        {
            _experience = _maxExperience;
        }

        Debug.Log("XP : " + _experience + "/" + _maxExperience);
    }

    private void Death()
    {
        Debug.Log("Le joueur est mort");
    }
}
