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
    private UnitAbilityActivationArea _activator;
    private LockOnSystem _lockOnSystem;
    private UnitSpecificAnimations _animations;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs, UnitAbilityActivationArea activator, LockOnSystem lockOnSystem)
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
        InitAbilityData();
    }

    private void InitAbilityData()
    {
        if (_abilityDatas != null)
        {
            const int tempAbilityIndex = 0;
            AbilityData abilityData = _abilityDatas[tempAbilityIndex];
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
            AbilityMenu();
            ActivateAbility();
        }
    }

    private void AbilityMenu()
    {
        if (_inputs.getEsc)
        {
            _inputs.setEsc = false;
            ToggleAbilityMenu();
        }
    }

    private void ToggleAbilityMenu()
    {
        if (!IsAbilityMenuUIActive())
        {
            OpenAbilityMenu();
        }
        else
        {
            CloseAbilityMenu();
        }
    }

    private void OpenAbilityMenu()
    {
        SetActiveUIElements(true);
        _activator.ShowAbilityActivationArea(_abilityRadius);
    }

    private void CloseAbilityMenu()
    {
        SetActiveUIElements(false);
        _activator.HideAbilityActivationArea();
        _lockOnSystem.ReleaseCurrentTarget();
    }

    private void ActivateAbility()
    {
        if (_animations != null)
        {
            if (_inputs.getLightAttack && IsAbilityMenuUIActive())
            {
                _inputs.setLightAttack = false;
                StartAbilityAnimation();
            }
        }
    }

    private void StartAbilityAnimation()
    {
        if (_hasTarget)
        {
            SetActiveUIElements(false);
            _activator.HideAbilityActivationArea();
            _animations.ApplyAbilityAnimation(_abilityName);
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
