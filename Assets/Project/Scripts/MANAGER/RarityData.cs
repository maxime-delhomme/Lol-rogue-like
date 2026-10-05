using UnityEngine;

[CreateAssetMenu(menuName = "Level Up/Rarity Data")]
public class RarityData : ScriptableObject
{
    [Header("Base Chances")]
    public float _commonChance = 70f;
    public float _uncommonChance = 20f;
    public float _rareChance = 7f;
    public float _epicChance = 2.5f;
}