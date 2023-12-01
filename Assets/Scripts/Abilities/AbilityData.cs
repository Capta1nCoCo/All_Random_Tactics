using System;
using UnityEngine;

[Serializable]
public struct AbilityData
{
    [SerializeField] private string _name;
    [SerializeField] private float _activationRadius;

    public string getName { get { return _name; } }
    public float getActivationRadius { get { return _activationRadius; } }
}