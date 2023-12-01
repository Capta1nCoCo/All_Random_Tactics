using UnityEngine;
using Zenject;

public class UnitAttack : MonoBehaviour, ICurrentUnitUser
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
        Attack();
    }

    private void Attack()
    {
        if (_animations != null && _inputs.getLightAttack && _animations.getIsOpportunityWindow)
        {
            ProcessLightAttack();
        }
        else if (_inputs.getLightAttack)
        {
            ResetLightAttackInput();
        }
    }

    private void ProcessLightAttack()
    {
        ResetLightAttackInput();
        if (_hasTarget)
        {
            ApplyLightAttack();
        }
    }

    private void ResetLightAttackInput()
    {
        _inputs.setLightAttack = false;
    }

    // TODO: check conditionals and remove deprecated
    private void ApplyLightAttack()
    {
        if (!_animations.getInAnimation || _animations.getIsOpportunityWindow)
        {
            _animations.ApplyLightAttackAnimation();
        }
    }
}
