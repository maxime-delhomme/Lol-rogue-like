using System;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    [Header("Spell")]
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private LayerMask _groundLayer;
    private Vector3 _pendingSpellDirection;

    [Header("Preview")]
    [SerializeField] private GameObject _prefabPreview;
    [SerializeField] private float _spellSize;

    [Header("QTE")]
    [SerializeField] private QTEManager _qteManager;

    [Header("Input")]
    [SerializeField] private InputActionAsset InputActions;
    private InputAction m_spellAction;
    private GameObject _previewInstance;
    private float _cooldownTimer = 0f;
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
        if(_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (_uiManager != null)
        {
            _uiManager.UpdateCooldownSpell(_cooldownTimer, _cooldown);
        }

        if (m_spellAction.IsPressed() && _cooldownTimer <= 0f)
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

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
        {
            Vector3 position = hit.point;

            if(_previewInstance == null)
            {
                _previewInstance = Instantiate(_prefabPreview,position,Quaternion.identity);

                _previewInstance.transform.localScale = Vector3.one * _spellSize;
            }

            _previewInstance.transform.position = position;
        }
    }

    private void CacherPreview()
    {
        if (_previewInstance != null)
        {
            Destroy(_previewInstance);
            _previewInstance = null;
        }
    }

    private void LancerSort()
    {
        if (_cooldownTimer > 0f)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
        {
            Vector3 destination = new Vector3(hit.point.x, _launchPoint.position.y, hit.point.z);

            Vector3 direction = (destination - _launchPoint.position).normalized;

            _pendingSpellDirection = direction;

            if (player.PeutFaireQTE())
            {
                player.ConsommerRage();

                _qteManager.DemarrerQTE(this);

                return;
            }

            LancerSort(false);
        }
    }

    public void LancerSort(bool improved)
    {
        Sort sort = Instantiate(_spellPrefab, _launchPoint.position, Quaternion.identity).GetComponent<Sort>();

        sort.Initialiser(_pendingSpellDirection, improved);

        _cooldownTimer = _cooldown;
    }
}
