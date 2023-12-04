using TMPro;
using UnityEngine;

public class AbilityMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _abilityName;

    public void SetAbilityName(string name)
    {
        _abilityName.text = name;
    }
}
