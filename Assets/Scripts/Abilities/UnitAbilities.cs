using UnityEngine;

public class UnitAbilities : MonoBehaviour
{
    [SerializeField] private AbilityData[] _abilityDatas;

    public AbilityData[] getAbilityDatas {  get { return _abilityDatas; } }
}