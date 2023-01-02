using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIPanelUnits : MonoBehaviour
{

    [SerializeField] private GameObject _unitIconsFolder;
    [SerializeField] private GameObject _resourceCountFolder;
    [SerializeField] private GameObject _unitTypeOrCountFolder;
    [SerializeField] private GameObject _singleUnitObject;
    [SerializeField] private GameObject _multipleUnitsObject;
    [SerializeField] private Transform _unitIcon;

    private ICollection<Unit> _selectedUnits;

    private void DisplayUnitsData(ICollection<Unit> _selectedUnits)
    {

        if (_selectedUnits.Count == 1)
        {
            _singleUnitObject.SetActive(true);

            _unitTypeOrCountFolder.GetComponent<TMP_Text>().text =
            _selectedUnits.ToList()[0].Name.ToString();

            Instantiate(_unitIcon, _unitIconsFolder.transform);
        }

        else
        {
            _multipleUnitsObject.SetActive(true);

            foreach (Unit unit in _selectedUnits)
            {
                Instantiate(_unitIcon, _unitIconsFolder.transform);
            }

            _unitTypeOrCountFolder.GetComponent<TMP_Text>().text =
            _selectedUnits.Count.ToString();
        }
    }



    private void Move()
    {

    }

    private void Harvest()
    {

    }

    private void Remove()
    {

    }
}
