using System;
using UnityEngine;

[Serializable]
public struct AbilityData
{
    [SerializeField] private AbilityName _name;
    [SerializeField] [Range(0f, 50f)] private float _activationRadius;

    public float getActivationRadius { get { return _activationRadius; } }

    public string GetName()
    {
        switch (_name)
        {
            case AbilityName.BasicCombo:
                return Constants.AbilityNames.BasicCombo;
            case AbilityName.Charge:
                return Constants.AbilityNames.Charge;
            default:
                return "NonExistingAbilityName";
        }
    }
}