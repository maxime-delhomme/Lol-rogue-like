using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellManager : MonoBehaviour
{

    [SerializeField] private SpellData spell1;
    [SerializeField] private SpellData spell2;
    [SerializeField] private SpellData spell3;
    [SerializeField] private SpellData spell4;

    private SpellData[] _spell;

    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction[] _spellActions;

    private void Awake()
    {
        _spellActions = new InputAction[]
            {
            _inputActions.FindActionMap("Player").FindAction("FirstSpell"),
            _inputActions.FindActionMap("Player").FindAction("SecondSpell"),
            _inputActions.FindActionMap("Player").FindAction("ThirdSpell"),
            _inputActions.FindActionMap("Player").FindAction("FourthSpell")
            };

        _spell = new SpellData[] { spell1, spell2, spell3, spell4 };

        ;
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
        _inputActions.FindActionMap("Player").FindAction("FirstSpell").performed += OnFirstSpell;
        _inputActions.FindActionMap("Player").FindAction("SecondSpell").performed += OnSecondSpell;
        _inputActions.FindActionMap("Player").FindAction("ThirdSpell").performed += OnThirdSpell;
        _inputActions.FindActionMap("Player").FindAction("FourthSpell").performed += OnFourthSpell;

    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").FindAction("FirstSpell").performed -= OnFirstSpell;
        _inputActions.FindActionMap("Player").FindAction("FirstSpell").performed -= OnFirstSpell;
        _inputActions.FindActionMap("Player").FindAction("SecondSpell").performed -= OnSecondSpell;
        _inputActions.FindActionMap("Player").FindAction("ThirdSpell").performed -= OnThirdSpell;
        _inputActions.FindActionMap("Player").FindAction("FourthSpell").performed += OnFourthSpell;
        _inputActions.FindActionMap("Player").Disable();

    }

    private void Update()
    {

    }

    private void OnFirstSpell(InputAction.CallbackContext context)
    {
        switch(spell1.numberOfClickNecessary)
        {
            case 1:

                break;
            case 2:

                break;
        }
        Debug.Log("Q hit");
        //spell1.OnSpellCast();

        
    }
    private void OnSecondSpell(InputAction.CallbackContext context)
    {

    }
    private void OnThirdSpell(InputAction.CallbackContext context)
    {
    }
    private void OnFourthSpell(InputAction.CallbackContext context)
    {

    }
   

}
