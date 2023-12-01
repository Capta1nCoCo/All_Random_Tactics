using System;
using UnityEngine;
using Zenject;

public class AbilityController : MonoBehaviour, ICurrentUnitUser
{
    [SerializeField] private AbilityMenuUI _abilityMenuUI;

    private AbilityData[] _abilityDatas;
    private string _abilityName;
    private float _abilityRadius;

    private bool _hasTarget;

    private ART_Inputs _inputs;
    private UnitAbilityActivator _activator;
    private LockOnSystem _lockOnSystem;
    private UnitSpecificAnimations _animations;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs, UnitAbilityActivator activator, LockOnSystem lockOnSystem)
    {
        _inputs = inputs;
        _activator = activator;
        _lockOnSystem = lockOnSystem;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _animations = unit.getUnitSpecificAnimations;
        try
        {
            _abilityDatas = unit.getUnitAbilities.getAbilityDatas;
        }
        catch (NullReferenceException)
        {
            _abilityDatas = null;
            Debug.Log($"{unit.name} has no abilities");
        }

        if (_abilityDatas != null)
        {
            // TEMP
            AbilityData abilityData = _abilityDatas[0];
            _abilityName = abilityData.getName;
            _abilityRadius = abilityData.getActivationRadius;
            _abilityMenuUI.SetAbilityName(_abilityName);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnLockOn += OnLockOn;
    }

    private void OnDisable()
    {
        GameEvents.OnLockOn -= OnLockOn;
    }

    private void OnLockOn(bool locked)
    {
        _hasTarget = locked;
    }

    private void Start()
    {
        if (IsAbilityMenuUIActive())
        {
            SetActiveUIElements(false);
        }
    }

    private void Update()
    {
        if (_abilityMenuUI != null)
        {
            if (_inputs.getEsc)
            {
                _inputs.setEsc = false;
                if (!IsAbilityMenuUIActive())
                {
                    SetActiveUIElements(true);
                    _activator.ShowAbilityActivationArea(_abilityRadius);
                }
                else
                {
                    SetActiveUIElements(false);
                    _activator.HideAbilityActivationArea();
                    _lockOnSystem.ReleaseCurrentTarget();
                }
            }

            if (_inputs.getLightAttack && IsAbilityMenuUIActive())
            {
                _inputs.setLightAttack = false;
                if (_hasTarget && _animations != null)
                {
                    SetActiveUIElements(false);
                    _activator.HideAbilityActivationArea();
                    _animations.ApplyAbilityAnimation(_abilityName);
                }
            }
        }
    }

    private bool IsAbilityMenuUIActive()
    {
        return _abilityMenuUI.gameObject.activeSelf;
    }

    private void SetActiveUIElements(bool value)
    {
        _abilityMenuUI.gameObject.SetActive(value);
    }
}
