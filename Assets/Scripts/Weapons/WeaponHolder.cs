using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public ObjectData[] weapon;
    private GameObject activeWeapon;

    void Start()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].groundItem.SetActive(false);
        }

        if (weapon.Length > 0)
        {
            SwitchWeapon(0);
        }
    }

    private void OnDisable() {
    if (activeWeapon != null)
            {
                activeWeapon.SetActive(false);
            }

            if (weapon.Length > 0)
            {
                SwitchWeapon(0);
            }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapon.Length)
        {
            if (activeWeapon != null)
            {
                activeWeapon.SetActive(false);
            }

            activeWeapon = weapon[weaponIndex].groundItem;
            activeWeapon.SetActive(true);

        }
    }
}
