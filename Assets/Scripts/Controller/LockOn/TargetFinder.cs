using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    private UnitArmature _unitArmature;

    public void Init(UnitArmature unit)
    {
        _unitArmature = unit;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ResetTargets();
    }

    private void ResetTargets()
    {
        _unitArmature.getAvaliableTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        AddTarget(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveTarget(other.transform);
    }

    private void AddTarget(Transform target)
    {
        _unitArmature.getAvaliableTargets.Add(target);
        Debug.Log($"Target {target.name} is in reach!");
    }

    private void RemoveTarget(Transform target)
    {
        _unitArmature.getAvaliableTargets.Remove(target);
        Debug.Log($"Target {target.name} is out of reach!");
    }
}
