using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundItem : MonoBehaviour
{
    public new string name;
    public ObjectData pickupItem;

    private PlayerData data;

    private bool isPickedUp;

    public GameObject Parent;
    public TextMeshProUGUI triggerText;

    private void Start()
    {
        data = FindObjectOfType<PlayerData>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            triggerText.gameObject.SetActive(true);
            triggerText.text = "Нажми F чтобы собрать " + name;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.F) && !isPickedUp)
            {
                bool itemExist = false;
                for(int i = 0; i < data.weapon.Length; i++)
                {
                    if(data.weapon[i] != null && data.weapon[i].itemName == pickupItem.itemName)
                    {
                        data.weapon[i].quantity ++;
                        itemExist = true;
                        break;
                    }
                }

                if(!itemExist)
                {
                    for (int i = 0; i < data.weapon.Length; i++)
                    {
                        if(data.weapon[i] == null)
                        {
                            data.weapon[i] = pickupItem;
                            isPickedUp = true;
                            break;
                        }
                    }
                }
                if (itemExist || isPickedUp)
                {
                    isPickedUp = true;
                    Destroy(Parent, 0.1f);
                    data.ReloadWeapon();
                    triggerText.gameObject.SetActive(false); 
                }
            }
        }        
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPickedUp = false;
            triggerText.gameObject.SetActive(false); 
        }
    }
}
