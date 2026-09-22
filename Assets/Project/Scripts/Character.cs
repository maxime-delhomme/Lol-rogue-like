using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    NewInputActions _input;
    NavMeshAgent _agent;

    [SerializeField] LayerMask clickableLayers;
    [SerializeField] private float _speed;
    [SerializeField] private int _health;
    [SerializeField] private int _level;
    [SerializeField] private int _exp;
    private int expRequired = 10;
    [SerializeField] private int _gold;

    float lookRotationSpeed = 8f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _input = new NewInputActions();
        AssignInputs();
    }

    void AssignInputs()
    {
        _input.Player.Movement.performed += ctx => ClickToMove();
        _input.Player.Spells.performed += ctx => PrimarySpells();
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    private void Update()
    {
        FaceTarget();
        EarnLevel();
    }
    void PrimarySpells()
    {
        _exp += 10;
        _gold += 10;
    }

    void EarnLevel()
    {

        if (_exp == expRequired)
        {
            _level += 1;
            _exp = 0;
            expRequired = expRequired * 2;
            Debug.Log(expRequired);
        }
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            _agent.destination = hit.point;
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (_agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }
}
