using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private SpellCaster _spellCaster;
    [SerializeField] private Character _character;

    [Header("Augment Inventory")]
    [SerializeField] private int _baseAugmentCapacity = 6;
    [SerializeField] private readonly List<AugmentData> _activeAugments = new();
    private int _additionalAugmentCapacity = 0;
    public IReadOnlyList<AugmentData> ActiveAugments => _activeAugments;
    public int AugmentCapacity => _baseAugmentCapacity + _additionalAugmentCapacity;
    public int AugmentCount => _activeAugments.Count;
    public bool IsInventoryFull => AugmentCount >= AugmentCapacity;

    public void SetQTEManager(QTEManager qteManager)
    {
        _qteManager = qteManager;
    }

    private void Awake()
    {
        _spellCaster = GetComponent<SpellCaster>();
        _character = GetComponent<Character>();
    }

    public bool AddAugment(AugmentData augment)
    {
        if (augment == null)
        {
            Debug.LogWarning("Impossible d'activer cet augment : donnée ou effet manquant.");
            return false;
        }

        if (IsInventoryFull)
        {
            Debug.Log("Inventaire d'augments plein");
            return false;
        }

        switch (augment._effectType)
        {
            case AugmentEffectType.DoubleSort:
                _spellCaster.ActiverDoubleSort();
                break;

            case AugmentEffectType.IamSpeed:
                _qteManager.ActiverIamSpeed();
                break;

            case AugmentEffectType.DontTouchMe:
                _character.ActiverDontTouchMe();
                break;
        }

        _activeAugments.Add(augment);
        return true;
    }

    public bool RemoveAugment(AugmentData augment)
    {
        if (augment == null || !_activeAugments.Contains(augment))
            return false;

        switch (augment._effectType)
        {
            case AugmentEffectType.DoubleSort:
                _spellCaster.DesactiverDoubleSort();
                break;

            case AugmentEffectType.IamSpeed:
                _qteManager.DesactiverIamSpeed();
                break;

            case AugmentEffectType.DontTouchMe:
                _character.DesactiverDontTouchMe();
                break;
        }

        _activeAugments.Remove(augment);
        return true;
    }
}
