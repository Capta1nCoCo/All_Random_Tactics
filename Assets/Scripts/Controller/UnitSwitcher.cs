using UnityEngine;

public class UnitSwitcher : MonoBehaviour
{
    [SerializeField] private UnitArmature[] _unitArmatures;

    private UnitArmature _newUnitArmature;

    private int _currentUnitIndex = 0;

    public delegate void InitUnitMethod(UnitArmature newUnitArmature);

    private InitUnitMethod InitNewUnit;
    private ART_Inputs _inputs;

    public UnitArmature getNewUnitArmature { get { return _newUnitArmature; } }

    public void Init(ART_Inputs inputs, InitUnitMethod InitUnitArmature)
    {
        _inputs = inputs;
        InitNewUnit = InitUnitArmature;
        _newUnitArmature = _unitArmatures[_currentUnitIndex];
    }

    private void Update()
    {
        SwitchUnit();
    }

    public void SwitchUnit()
    {
        if (_unitArmatures.Length > 0)
        {
            ChooseNext();
            ChoosePrev();
        }
    }

    private void ChooseNext()
    {
        if (_inputs.getNextUnit)
        {
            _inputs.setNextUnit = false;
            if (_currentUnitIndex < _unitArmatures.Length - 1)
            {
                _currentUnitIndex++;
            }
            else
            {
                _currentUnitIndex = 0;
            }
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }

    private void ChoosePrev()
    {
        if (_inputs.getPrevUnit)
        {
            _inputs.setPrevUnit = false;
            if (_currentUnitIndex > 0)
            {
                _currentUnitIndex--;
            }
            else
            {
                _currentUnitIndex = _unitArmatures.Length - 1;
            }
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }
}
