using UnityEngine;

public class RarityManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private RarityData _rarityData;

    public BonusRarity GetRandomRarity(float luck)
    {
        float[] weights = GetWeights(luck);

        float totalWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        float randomValue = Random.Range(0f, totalWeight);

        for (int i = 0; i < weights.Length; i++)
        {
            randomValue -= weights[i];

            if (randomValue < 0f)
            {
                return (BonusRarity)i;
            }
        }

        return BonusRarity.Common;
    }

    private float[] GetWeights(float luck)
    {
        luck = Mathf.Max(0f, luck);

        float[] baseChances =
        {
            _rarityData._commonChance,
            _rarityData._uncommonChance,
            _rarityData._rareChance,
            _rarityData._epicChance,
        };

        float[] luckMultipliers =
        {
            1f,
            1f + luck / 100f,
            1f + (luck / 100f) * 2f,
            1f + (luck / 100f) * 3f,
            1f + (luck / 100f) * 4f
        };

        float[] weights = new float[baseChances.Length];

        for (int i = 0; i <weights.Length; i++)
        {
            weights[i] = baseChances[i] * luckMultipliers[i];
        }

        return weights;
    }
}
