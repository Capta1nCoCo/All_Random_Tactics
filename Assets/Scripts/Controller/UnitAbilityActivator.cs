using UnityEngine;
using Zenject;

public class UnitAbilityActivator : MonoBehaviour, ICurrentUnitUser
{
    [SerializeField] private float _abilityRadius = 2f;

    private TargetFinder _unitTargetFinder;

    private ART_Inputs _inputs;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs)
    {
        _inputs = inputs;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _unitTargetFinder = unit.getTargetFinder;
    }

    // TODO: Replace with Ability UI
    private void Update()
    {
        if (_unitTargetFinder != null)
        {
            if (_inputs.getEsc && !_unitTargetFinder.gameObject.activeSelf)
            {
                _inputs.setEsc = false;
                FindTargetsInRadius(_abilityRadius);
            }
        }
    }

    public void FindTargetsInRadius(float radius)
    {
        _unitTargetFinder.transform.localScale = new Vector3(radius, _unitTargetFinder.transform.localScale.y, radius);
        _unitTargetFinder.gameObject.SetActive(true);
    }
}
