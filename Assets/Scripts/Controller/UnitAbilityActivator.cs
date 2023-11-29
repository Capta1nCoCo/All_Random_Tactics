using UnityEngine;

public class UnitAbilityActivator : MonoBehaviour, ICurrentUnitUser
{
    private TargetFinder _unitTargetFinder;

    public void SetCurrentUnit(UnitArmature unit)
    {
        _unitTargetFinder = unit.getTargetFinder;
    }

    public void ShowAbilityActivationArea(float abilityRadius)
    {
        if (_unitTargetFinder != null)
        {
            if (!_unitTargetFinder.gameObject.activeSelf)
            {
                FindTargetsInRadius(abilityRadius);
            }
        }
    }

    private void FindTargetsInRadius(float radius)
    {
        _unitTargetFinder.transform.localScale = new Vector3(radius, _unitTargetFinder.transform.localScale.y, radius);
        _unitTargetFinder.gameObject.SetActive(true);
    }
}
