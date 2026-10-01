using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QTEManager : MonoBehaviour
{
    [SerializeField] private SpellCaster _spellCaster;
    [SerializeField] private Character _player;
    private int _spellIndex;

    [Header("input")]
    [SerializeField] private InputActionAsset _inputActions;
    private InputActionMap _playerMap;
    private InputActionMap _qteMap;
    private InputAction[] _qteActions;

    [Header("QTE")]
    [SerializeField] private float _timerPerKey;
    [SerializeField] private float _keyDisplayDelay;
    [SerializeField] private float _timeScale;

    [Header("UI")]
    [SerializeField] private UIManager _uiManager;

    private int _currentStep = 0;
    private bool _qteActive = false;

    private float[] _keyTimers = new float[3];
    private bool[] _keyVisible = new bool[3];
    private float[] _keyDisplayTimers = new float[3];

    private int[] _qteSequence = new int[3];

    [Header("Touche")]
    private string[] _keyNames =
    {
        "Z","Q", "S", "D"
    };

    private void Awake()
    {
        _playerMap = _inputActions.FindActionMap("Player");
        _qteMap = _inputActions.FindActionMap("UI");

        _qteActions = new InputAction[]
        {
            _qteMap.FindAction("qte_Z"),
            _qteMap.FindAction("qte_Q"),
            _qteMap.FindAction("qte_S"),
            _qteMap.FindAction("qte_D")
        };
    }

    public void SetPlayer(Character player)
    {
        _player = player;
    }
    public void SetSpellCaster(SpellCaster spellCaster)
    {
        _spellCaster = spellCaster;
    }

    private void Update()
    {
        if (!_qteActive)
            return;

        GererApparitionTouche();
        GererTimerTouche();
        VerifierTouche();
    }

    private void GererApparitionTouche()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_keyVisible[i])
                continue;

            if (_keyDisplayTimers[i] > 0f)
            {
                _keyDisplayTimers[i] -= Time.unscaledDeltaTime;

                if (_keyDisplayTimers[i] <= 0f)
                {
                    AfficherTouche(i);
                }
            }

        }
    }

    private void GererTimerTouche()
    {
        for (int i =0; i < 3; i++)
        {
            if (!_keyVisible[i])
                continue;

            _keyTimers[i] -= Time.unscaledDeltaTime;

            _uiManager.UpdateTimerTouche(i, _keyTimers[i], _timerPerKey);

            if (_keyTimers[i] <= 0f)
            {
                EchouerQTE();
                return;
            }
        }
    }
    private void VerifierTouche()
    {
        if (!_keyVisible[_currentStep])
            return;

        InputAction expectedAction = _qteActions[_qteSequence[_currentStep]];

        if (expectedAction.WasPressedThisFrame())
        {
            _uiManager.CacherTouche(_currentStep);

            _keyVisible[_currentStep] = false;

            _currentStep++;

            if (_currentStep >= 3)
            {
                ReussirQTE();
            }

            return;
        }

        for (int i = 0; i < _qteActions.Length; i++)
        {
            if (i == _qteSequence[_currentStep])
                continue;


            if (_qteActions[i].WasPressedThisFrame())
            {
                EchouerQTE();
                return;
            }
        }
    }

    public void DemarrerQTE(SpellCaster spellCaster, int spellIndex)
    {
        _spellCaster = spellCaster;
        _spellIndex = spellIndex;

        _currentStep = 0;
        _qteActive = true;

        GenererSequence();

        for (int i = 0; i < 3; i++)
        {
            _keyTimers[i] = 0f;
            _keyVisible[i] = false;

            _keyDisplayTimers[i] = _keyDisplayDelay * i;
        }

        _playerMap.Disable();
        _qteMap.Enable();

        Time.timeScale = _timeScale;

        _uiManager.AfficherUI(_keyNames[_qteSequence[0]], _keyNames[_qteSequence[1]], _keyNames[_qteSequence[2]]);

        AfficherTouche(0);
    }

    private void AfficherTouche(int index)
    {
        _keyVisible[index] = true;

        _keyTimers[index] = _timerPerKey;

        _uiManager.AfficherTouche(index);
    }

    private void GenererSequence()
    {
        int firstKey = UnityEngine.Random.Range(0, _qteActions.Length);

        int secondKey = UnityEngine.Random.Range(0, _qteActions.Length);

        while (secondKey == firstKey)
        {
            secondKey = UnityEngine.Random.Range(0, _qteActions.Length);
        }

        int thirdKey = UnityEngine.Random.Range(0, _qteActions.Length);

        while (thirdKey == firstKey || thirdKey == secondKey)
        {
            thirdKey = UnityEngine.Random.Range(0, _qteActions.Length);
        }

        _qteSequence[0] = firstKey;
        _qteSequence[1] = secondKey;
        _qteSequence[2] = thirdKey;
    }

    private void ReussirQTE()
    {
        _qteActive = false;

        Time.timeScale = 1f;

        _qteMap.Disable();
        _playerMap.Enable();

        _uiManager.CacherUI();

        _spellCaster.LancerSort(_spellIndex, true);
        _player.ConsommerRage();
    }

    private void EchouerQTE()
    {
        _qteActive = false;

        Time.timeScale = 1f;

        _qteMap.Disable();
        _playerMap.Enable();

        _uiManager.CacherUI();

        _spellCaster.LancerSort(_spellIndex, false);
        _player.ConsommerRage();

    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        _qteMap.Disable();
        _playerMap.Enable();
    }
}
