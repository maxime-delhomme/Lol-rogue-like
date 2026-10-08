using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class SpellCaster : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _previewPrefab;
    private Character _player;

    [Header("Spells")]
    [SerializeField] private SpellData[] _spells;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private LayerMask _groundLayer;
    private GameObject _previewObject;


    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction[] _spellActions;
    private InputAction _cancelSpellAction;
    private InputAction _qteAction;

    [Header("manage")]
    private float[] _cooldownTimers;
    private int[] _charges;
    private int _pendingSpellIndex = -1;
    private Vector3 _pendingTargetPosition;
    private bool _isPreviewing = false;
    private bool _useQte = false;
    private bool[] _improvedActive;
    private float[] _improvedTimers;


    private void Awake()
    {
        _player = GetComponent<Character>();

        _spellActions = new InputAction[]
        {
            _inputActions.FindActionMap("Player").FindAction("FirstSpell"),
            _inputActions.FindActionMap("Player").FindAction("SecondSpell"),
            _inputActions.FindActionMap("Player").FindAction("ThirdSpell"),
            _inputActions.FindActionMap("Player").FindAction("FourthSpell")
        };
        _cancelSpellAction = _inputActions.FindActionMap("Player").FindAction("CancelSpell");
        _qteAction = _inputActions.FindActionMap("Player").FindAction("QTE");

        _cooldownTimers = new float[_spells.Length];
        _charges = new int[_spells.Length];

        for (int i = 0; i < _spells.Length; i++)
        {
            _charges[i] = _spells[i]._maxCharges;
        }

        _improvedActive = new bool[_spells.Length];
        _improvedTimers = new float[_spells.Length];
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }

    public void SetQTEManager(QTEManager qteManager)
    {
        _qteManager = qteManager;
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void SetUIManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    private void Update()
    {
        for (int i = 0; i < _cooldownTimers.Length; i++)
        {
            if (_charges[i] < _spells[i]._maxCharges)
            {
                _cooldownTimers[i] -= Time.deltaTime;

                if (_cooldownTimers[i] <= 0f)
                {
                    _charges[i]++;

                    if (_charges[i] < _spells[i]._maxCharges)
                    {
                        _cooldownTimers[i] = _player.CalculateSpellCooldown(_spells[i]._cooldown);
                    }
                    else
                    {
                        _cooldownTimers[i] = 0f;
                    }
                }
            }

            float cooldown = _player.CalculateSpellCooldown(_spells[i]._cooldown);
            _uiManager.UpdateCooldownSpell(i, _cooldownTimers[i], cooldown);
            _uiManager.UpdateSpellCharges(i, _charges[i], _spells[i]._maxCharges);
        }

        if (_qteAction.WasPressedThisFrame() && _player.PeutFaireQTE())
        {
            _useQte = !_useQte;
            Debug.Log(_useQte);
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

        if (_isPreviewing && _cancelSpellAction.WasPressedThisFrame())
        {
            AnnulerPreview();
            return;
        }

        if (_isPreviewing && _previewObject != null)
        {
            SpellData spellData = _spells[_pendingSpellIndex];

            if ((_useQte || _improvedActive[_pendingSpellIndex]) && spellData._improvedSpell != null)
            {
                spellData = spellData._improvedSpell;
            }

            int range = _player.CalculateSpellRange(spellData._range);

            _pendingTargetPosition = GetMousePosition(range);

            if (spellData._width > 0f)
            {
                Vector3 direction = _pendingTargetPosition - _launchPoint.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    direction.Normalize();

                    _previewObject.transform.position = _launchPoint.position + direction * (spellData._range * 0.5f);

                    _previewObject.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            else
            {
                _previewObject.transform.position = _pendingTargetPosition;
            }

        }

        for (int i = 0; i < _spells.Length; i++)
        {
            if (!_improvedActive[i])
                continue;

            _improvedTimers[i] -= Time.deltaTime;
            
            if (_improvedTimers[i] <= 0f)
            {
                _improvedActive[i] = false;
                _improvedTimers[i] = 0f;
            }
        }

    }

    private Vector3 GetMousePosition(int range)
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
                position.y = hit.point.y;
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

        if (_charges[index] <= 0f)
        {
            return;
        }

        _pendingSpellIndex = index;
        _isPreviewing = true;

        SpellData spellData = _spells[index];

        if ((_useQte || _improvedActive[index]) && spellData._improvedSpell != null)
        {
            spellData = spellData._improvedSpell;
        }

        _previewObject = Instantiate(_previewPrefab);

        if (spellData._width > 0f)
        {
            _previewObject.transform.localScale = new Vector3(spellData._width, 1f, spellData._range);
        }
        else
        {
            _previewObject.transform.localScale = Vector3.one * spellData._impactRadius * 2f;
        }
    }

    public void AnnulerPreview()
    {
        _isPreviewing = false;
        _pendingSpellIndex = -1;

        if (_previewObject != null)
        {
            Destroy(_previewObject);
            _previewObject = null;
        }
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

        if (_previewObject != null)
        {
            Destroy(_previewObject);
            _previewObject = null;
        }

        if (_useQte && _player.PeutFaireQTE())
        {
            _qteManager.DemarrerQTE(this, index);
            _useQte = false;
        }
        else
        {
            LancerSort(index, false);
            _useQte = false;
        }

        _pendingSpellIndex = -1;
    }

    public void LancerSort(int index, bool improved)
    {

        SpellData spellData = _spells[index];

        bool useImproved = improved || _improvedActive[index];

        if (improved && spellData._improvedSpell != null)
        {
            _improvedActive[index] = true;
            _improvedTimers[index] = spellData._improvedDuration;
        }

        if (useImproved && spellData._improvedSpell != null)
        {
            spellData = spellData._improvedSpell;
        }

        Vector3 targetPosition = _pendingTargetPosition;

        Vector3 direction = targetPosition - _launchPoint.position;
        direction.y = 0f;
        direction.Normalize();

        GameObject spellObject = Instantiate(spellData._prefab, _launchPoint.position, Quaternion.identity);


        ISpell spell = spellObject.GetComponent<ISpell>();

        if (spell == null)
        {
            Destroy(spellObject);
            return;
        }


        SpellCastContext context = new SpellCastContext
        {
            _caster = transform,
            _direction = direction,
            _targetPosition = targetPosition,
            _improved = useImproved,
            _data = spellData
        };

        spell.Initialiser(context);

        _charges[index]--;

        if (_charges[index] < _spells[index]._maxCharges && _cooldownTimers[index] <= 0f)
        {
            _cooldownTimers[index] = _player.CalculateSpellCooldown(_spells[index]._cooldown);
        }
    }
}
