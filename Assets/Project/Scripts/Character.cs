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
    float lookRotationSpeed = 8f;

    [SerializeField] private int rageMax;
    private int rageActuelle = 0;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
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

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    public void AjouterRage(int montant)
    {
        rageActuelle += montant;

        if(rageActuelle > rageMax)
        {
            rageActuelle = rageMax;
        }

        Debug.Log("Rage : " + rageActuelle + "/" + rageMax);
    }

    public bool PeutFaireQTE()
    {
        return rageActuelle >= rageMax;
    }

    public void ConsommerRage()
    {
        rageActuelle -= rageMax;
    }
}
