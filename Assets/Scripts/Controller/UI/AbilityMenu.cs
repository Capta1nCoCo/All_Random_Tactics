using UnityEngine;
using Zenject;

public class AbilityMenu : MonoBehaviour
{
    [SerializeField] private GameObject _abilityMenuUI;
    [SerializeField] private float _abilityRadius = 2f;

    private ART_Inputs _inputs;
    private UnitAbilityActivator _activator;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs, UnitAbilityActivator activator)
    {
        _inputs = inputs;
        _activator = activator;
    }

    private void Start()
    {
        if (_abilityMenuUI.activeSelf)
        {
            _abilityMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (_abilityMenuUI != null)
        {
            if (_inputs.getEsc)
            {
                _inputs.setEsc = false;
                _abilityMenuUI.SetActive(true);
                _activator.ShowAbilityActivationArea(_abilityRadius);
            }
        }
    }
}
