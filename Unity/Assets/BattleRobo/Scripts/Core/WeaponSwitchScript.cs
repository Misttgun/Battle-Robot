using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;

public class WeaponSwitchScript : MonoBehaviour
{
    public int selectedWeapon;

    // Use this for initialization
    private void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    private void Update()
    {
        int prevSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }

        if (prevSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }


    private void SelectWeapon()
    {
        int index = 0;
        foreach (Transform weapon in transform)
        {
            if (index == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            index++;
        }
    }
}