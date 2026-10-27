using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    private readonly List<AugmentData> _activeAugments = new();

    private Character _player;
    private SpellCaster _spellCaster;

    private void Awake()
    {
        _player = GetComponent<Character>();
        _spellCaster = GetComponent<SpellCaster>();
    }
    public bool AddAugment(AugmentData augment)
    {
        if (augment == null)
        {
            Debug.LogWarning("Impossible d'activer cet augment : donnée ou effet manquant.");
            return false;
        }

        switch (augment._effectType)
        {
            case AugmentEffectType.DoubleSort:
                _spellCaster.ActiverDoubleSort();
                break;
        }

        _activeAugments.Add(augment);
        return true;
    }
}
