using UnityEngine;

public class UnitAbilityActivationArea : MonoBehaviour, ICurrentUnitUser
{
    private TargetFinder _unitTargetFinder;

    public void SetCurrentUnit(UnitArmature unit)
    {
        _unitTargetFinder = unit.getTargetFinder;
    }

    public void HideAbilityActivationArea()
    {
        SetActiveTargetFinder(false);
    }

    public void ShowAbilityActivationArea(float abilityRadius)
    {
        if (_unitTargetFinder != null)
        {
            FindTargetsInRadius(abilityRadius);
        }
    }

    private void FindTargetsInRadius(float radius)
    {
        _unitTargetFinder.transform.localScale = new Vector3(radius, _unitTargetFinder.transform.localScale.y, radius);
        if (!_unitTargetFinder.gameObject.activeSelf)
        {
            SetActiveTargetFinder(true);
        }
    }

    private void SetActiveTargetFinder(bool value)
    {
        if (_unitTargetFinder != null)
        {
            _unitTargetFinder.gameObject.SetActive(value);
        }
    }
}
