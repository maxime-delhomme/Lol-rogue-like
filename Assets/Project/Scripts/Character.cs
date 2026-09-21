using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    const string IDLE = "idle";
    const string WALK = "Walk";
    
    NewInputActions _input;
    NavMeshAgent _agent;

    [SerializeField] LayerMask clickableLayers;
    [SerializeField] private float _speed;

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
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            _agent.destination = hit.point;
        }
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
    }

    private void FaceTarget()
    {
        Vector3 direction = (_agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }
}
