using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{

    private readonly List<AugmentEffect> _activeAugments = new();

    public bool AddAugment(AugmentData augment)
    {
        if (augment == null || augment._effect == null)
        {
            Debug.LogWarning("Impossible d'activer cet augment : donnée ou effet manquant.");
            return false;
        }

        Character player = GetComponent<Character>();

        if (player == null)
        {
            Debug.LogError("Character introuvable sur le Player.");
            return false;
        }

        AugmentEffect effect = Instantiate(augment._effect);
        effect.Apply(player);

        _activeAugments.Add(effect);

        return true;
    }
}
