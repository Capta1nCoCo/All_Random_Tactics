using UnityEngine;
using Zenject;

public class UnitComboAttack : MonoBehaviour, ICurrentUnitUser
{
    private ART_Inputs _inputs;
    private UnitSpecificAnimations _animations;

    private bool _hasTarget;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs)
    {
        _inputs = inputs;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _animations = unit.getUnitSpecificAnimations;
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

    private void Update()
    {
        ComboAttack();
    }

    private void ComboAttack()
    {
        if (_animations != null)
        {
            if (_inputs.getLightAttack && _animations.getIsOpportunityWindow)
            {
                ProgressLightCombo();
            }
        }
    }

    private void ProgressLightCombo()
    {
        _inputs.setLightAttack = false;
        if (_hasTarget)
        {
            ApplyLightAttack();
        }
    }

    private void ApplyLightAttack()
    {
        _animations.ApplyLightAttackAnimation();
    }
}
