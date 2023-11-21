using UnityEngine;
using Zenject;

public class UnitAttack : MonoBehaviour, ICurrentUnitUser
{
    private ART_Inputs _inputs;
    private UnitSpecificAnimations _animations;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs)
    {
        _inputs = inputs;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _animations = unit.getUnitSpecificAnimations;
    }

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (_animations != null)
        {
            if (_inputs.getLightAttack)
            {
                ProcessLightAttack();
            }
        }
        else
        {
            _inputs.setLightAttack = false;
        }
    }

    private void ProcessLightAttack()
    {
        _inputs.setLightAttack = false;
        if (!_animations.getInAnimation || _animations.getIsOpportunityWindow)
        {
            _animations.ApplyLightAttackAnimation();
        }
    }
}
