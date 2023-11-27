using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private UnitArmature _unitArmature;

    private void OnDisable()
    {
        _unitArmature.getAvaliableTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform target = other.transform;
        _unitArmature.getAvaliableTargets.Add(target);
        Debug.Log($"Target {target.name} is in reach!");
    }

    private void OnTriggerExit(Collider other)
    {
        Transform target = other.transform;
        _unitArmature.getAvaliableTargets.Remove(target);
        Debug.Log($"Target {target.name} is out of reach!");
    }
}
