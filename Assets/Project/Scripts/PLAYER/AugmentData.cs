using UnityEngine;

public enum AugmentRarity
{
    Silver,
    Gold,
    Prismatic
}

[CreateAssetMenu(fileName = "NewAugment", menuName = "Game/Augment")]
public class AugmentData : ScriptableObject
{
    [Header("Informations")]
    public string _augmentName;
    [TextArea] public string _description;
    public AugmentRarity _rarity;

    [Header("Effect")]
    public AugmentEffectType _effectType;
    public float _effectValue;


    public int SkillPointCost
    {
        get
        {
            switch (_rarity)
            {
                case AugmentRarity.Silver:
                    return 1;

                case AugmentRarity.Gold:
                    return 2;

                case AugmentRarity.Prismatic:
                    return 3;

                default:
                    return 1;
            }
        }
    }
}

public enum AugmentEffectType
{
    DoubleSort,
    IamSpeed

}
