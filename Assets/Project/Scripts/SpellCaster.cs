using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
    [Header("Spell")]
    [SerializeField] private GameObject prefabSort;
    [SerializeField] private Transform pointDeLancement;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask layerSol;

    [Header("Preview")]
    [SerializeField] private GameObject prefabPreview;
    [SerializeField] private float rayonSort;

    [Header("QTE")]
    [SerializeField] private QTEManager qteManager;

    [Header("Input")]
    [SerializeField] private InputActionAsset InputActions;
    private InputAction m_spellAction;
    private GameObject previewInstance;
    private float cooldownRestant = 0f;
    private Character player;

    private void Awake()
    {
        m_spellAction = InputActions.FindActionMap("Player").FindAction("FirstSpell");
        player = GetComponent<Character>();
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
        if(cooldownRestant > 0f)
        {
            cooldownRestant -= Time.deltaTime;
        }

        if (m_spellAction.IsPressed() && cooldownRestant <= 0f)
        {
            AfficherPreview();
        }

        if (m_spellAction.WasReleasedThisFrame())

        {
            LancerSort();
            CacherPreview();
        }
    }

    private void AfficherPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerSol))
        {
            Vector3 position = hit.point;

            if(previewInstance == null)
            {
                previewInstance = Instantiate(prefabPreview,position,Quaternion.identity);

                previewInstance.transform.localScale = Vector3.one * rayonSort;
            }

            previewInstance.transform.position = position;
        }
    }

    private void CacherPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    private void LancerSort()
    {
        if (cooldownRestant > 0f)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 destination = new Vector3(hit.point.x, pointDeLancement.position.y, hit.point.z);

            Sort sort = Instantiate(prefabSort, pointDeLancement.position, Quaternion.identity).GetComponent<Sort>();

            sort.Initialiser(destination);

            cooldownRestant = cooldown;

            if (player.PeutFaireQTE())
            {
                player.ConsommerRage();

                qteManager.DemarrerQTE(sort);
            }
        }
    }
}
