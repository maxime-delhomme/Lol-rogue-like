using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private Character _player;
    [SerializeField] private UIManager _uiManager;

    [Header("Spells")]
    [SerializeField] private SpellData[] _spells;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _previewPrefab;
    private GameObject _previewObject;


    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction[] _spellActions;
    private float[] _cooldownTimers;
    private int _pendingSpellIndex = -1;
    private Vector3 _pendingTargetPosition;
    private bool _isPreviewing = false;
    private void Awake()
    {
        _spellActions = new InputAction[]
        {
            _inputActions.FindActionMap("Player").FindAction("FirstSpell"),
            _inputActions.FindActionMap("Player").FindAction("SecondSpell"),
            _inputActions.FindActionMap("Player").FindAction("ThirdSpell"),
            _inputActions.FindActionMap("Player").FindAction("FourthSpell")
        };

        _cooldownTimers = new float[_spells.Length];

        _player = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        for (int i = 0; i < _cooldownTimers.Length; i++)
        {
            if (_cooldownTimers[i] > 0f)
            {
                _cooldownTimers[i] -= Time.deltaTime;
            }

            _uiManager.UpdateCooldownSpell(i, _cooldownTimers[i], _spells[i]._cooldown);
        }

        for (int i = 0; i < _spellActions.Length; i++)
        {
            if (_spellActions[i].WasPressedThisFrame())
            {
                LancerPreview(i);
            }

            if (_spellActions[i].WasReleasedThisFrame())
            {
                SelectionnerSort(i);
            }
        }


        if (_isPreviewing)
        {
            _pendingTargetPosition = GetMousePosition(_spells[_pendingSpellIndex]._range);
        }

        _previewObject.transform.position = _pendingTargetPosition;
    }
    private Vector3 GetMousePosition(float range)
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _groundLayer))
        {
            Vector3 position = hit.point;

            Vector3 offset = position - transform.position;
            offset.y = 0f;

            if (offset.magnitude > range)
            {
                offset = offset.normalized * range;
                position = transform.position + offset;
            }

            return position;
        }

        return transform.position;
    }

    private void LancerPreview(int index)
    {
        if (index >= _spells.Length)
        {
            return;
        }

        if (_cooldownTimers[index] > 0f)
        {
            return;
        }

        _pendingSpellIndex = index;
        _isPreviewing = true;

        SpellData spellData = _spells[index];

        _previewObject = Instantiate(_previewPrefab);
        _previewObject.transform.localScale = Vector3.one * spellData._impactRadius * 2f;


    }

    private void SelectionnerSort(int index)
    {
        if (!_isPreviewing)
        {
            return;
        }

        if (index != _pendingSpellIndex)
        {
            return;
        }

        _isPreviewing = false;

        Destroy(_previewObject);
        _previewObject = null;

        if (_player.PeutFaireQTE())
        {
            _qteManager.DemarrerQTE(this, index);
        }
        else
        {
            LancerSort(index, false);
        }
    }

    public void LancerSort(int index, bool improved)
    {
        SpellData spellData = _spells[index];

        Vector3 targetPosition = _pendingTargetPosition;

        Vector3 direction = targetPosition - _launchPoint.position;
        direction.y = 0f;
        direction.Normalize();

        GameObject spellObject = Instantiate(spellData._prefab, _launchPoint.position, Quaternion.identity);

        ISpell spell = spellObject.GetComponent<ISpell>();

        if (spell == null)
        {
            Debug.LogError("Le prefab du sort ne possède pas de composant ISpell.");
            Destroy(spellObject);
            return;
        }


        SpellCastContext context = new SpellCastContext
        {
            _caster = transform,
            _direction = direction,
            _targetPosition = targetPosition,
            _improved = improved,
            _data = spellData
        };

        spell.Initialiser(context);

        _cooldownTimers[index] = spellData._cooldown;
    }
}
