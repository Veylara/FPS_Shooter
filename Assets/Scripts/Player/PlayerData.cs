using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth = 200;
    public int curHealth;
    public TextMeshProUGUI healthText;
    public Animator animator;
    
    public ObjectData[] weapon;
    public Image [] slots;
    public Image [] backgroundSlots;

    public Color selectColor;
    public Color standartColor;

    public Image bulletsSlot;
    public ObjectData bullet;

    private GameObject selectedWeapon; 
    private WeaponHolder weaponHolder; 
    public int curWeapon = 0;

    public GameObject riflePrefab;
    public GameObject grenadePrefab;
    public GameObject pistolPrefab;

    public bool isdied = false;
    public GameObject winScreen;

    public UIManager manager;

    public TextMeshProUGUI loseCountText;
    public TextMeshProUGUI winCountText;

    public static int orcsDiedTotal = 0; 

    public int deathCount;
    public int winnerCount;
    private const string DeathCountPlayerPrefsKey = "PlayerDeathCount";
    private const string WinnerCountPlayerPrefsKey = "PlayerWinnerCount";
    private bool hasWon = false;

    void Start()
    {
        Time.timeScale = 1;
        healthSlider.maxValue = maxHealth;
        curHealth = maxHealth;
        healthSlider.value = curHealth;
        healthText.text = curHealth.ToString("F0");

        bulletsSlot.sprite = bullet.sprite;
        bulletsSlot.color = Color.white;

        ChangeWeapon();

        weaponHolder = FindObjectOfType<WeaponHolder>();

        if(weaponHolder != null)
        {
            ChangeWeapon();
        }
        if (PlayerPrefs.HasKey(DeathCountPlayerPrefsKey))
        {
            deathCount = PlayerPrefs.GetInt(DeathCountPlayerPrefsKey);
        }
        if (PlayerPrefs.HasKey(WinnerCountPlayerPrefsKey))
        {
            winnerCount = PlayerPrefs.GetInt(WinnerCountPlayerPrefsKey);
        }
        loseCountText.text = deathCount.ToString();
        winCountText.text = winnerCount.ToString();
    }

    private void Update() {
        if(Input.GetAxisRaw("Mouse ScrollWheel") >0)
        {
            if(curWeapon - 1 < 0)
            {
                curWeapon = weapon.Length-1;
            }
            else
            {
                curWeapon--;
            }
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if(curWeapon + 1 > weapon.Length -1)
            {
                curWeapon = 0;
            }
            else
            {
                curWeapon++;
            }
        }
        ChangeWeapon();

        if (orcsDiedTotal >= 2 && !hasWon)
        {
            winnerCount++;
            manager.WinScreen();
            PlayerPrefs.SetInt(WinnerCountPlayerPrefsKey, winnerCount); 
            hasWon = true;
        }
        loseCountText.text = deathCount.ToString();
        winCountText.text = winnerCount.ToString();

    }

    public void TakeDamage(int damage)
    {
        curHealth -= damage;
        curHealth = Mathf.Clamp(curHealth, 0 , maxHealth);
        healthSlider.value = curHealth;
        if(curHealth <= 0)
        {
            isdied = true;
            deathCount++;
            manager.DeathScreen();
            PlayerPrefs.SetInt(DeathCountPlayerPrefsKey, deathCount);
        }
        healthText.text = curHealth.ToString("F0");
    }

    public void Respawn()
    {
        orcsDiedTotal = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void Win() {
        orcsDiedTotal = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void ReloadWeapon()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            if(weapon[i] != null)
            {
                slots[i].sprite = weapon[i].sprite;
                if(weapon[i].quantity > 0)
                {
                    slots[i].color = Color.white;
                }
                else
                {
                    slots[i].color = Color.clear;
                    weapon[i] = null;
                }
            }
        }
    }

    public void ChangeWeapon()
    {
        for(int i = 0; i < backgroundSlots.Length; i++)
        { 
            if(i == curWeapon)
            {
                backgroundSlots[i].color = selectColor;
            }
            else
            {
                backgroundSlots[i].color = standartColor;

            }
        }

        DeactivateAllWeapons();

        if (curWeapon >= 0 && curWeapon < weapon.Length && weapon[curWeapon] != null)
        {
            weaponType currentWeaponType = weapon[curWeapon].weaponType;
            if(currentWeaponType == weaponType.Rifle)
            {
                riflePrefab.SetActive(true);
            }
            else if(currentWeaponType == weaponType.Grenade)
            {
                grenadePrefab.SetActive(true);
            }
            else if(currentWeaponType == weaponType.Pistol)
            {
                pistolPrefab.SetActive(true);
            }
        }
    }

    private void DeactivateAllWeapons()
    {
        riflePrefab.SetActive(false);
        pistolPrefab.SetActive(false);
        grenadePrefab.SetActive(false);
    }


}
