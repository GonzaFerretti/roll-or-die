using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] 
    private Image healthBar;

    [SerializeField] private RectTransform weaponIconsContainer;

    [SerializeField] 
    private WeaponIcon[] WeaponIcons;

    [SerializeField] private WeaponIcon WeaponIconPrefab;
    [SerializeField] private int weaponIconQty = 6;

    private void Awake()
    {
        WeaponIcons = new WeaponIcon[weaponIconQty];
        for (int i = 0; i < WeaponIcons.Length; i++)
        {
            WeaponIcon newIcon = Instantiate(WeaponIconPrefab, weaponIconsContainer);
            RectTransform trans = newIcon.GetComponent<RectTransform>();
            trans.anchorMax = Vector2.zero;
            trans.anchorMin = Vector2.zero;
            trans.anchoredPosition = new Vector2(56 + i * 75f, 72.8f);
            WeaponIcons[i] = newIcon;
        }
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canvas.worldCamera)
        {
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "UI";
        }
    }

    public void TriggerRestart()
    {
        GameObject.Find("Game Manager").GetComponent<Manager>().Restart();
    }

    public void UpdateIcons(Weapon[] weapons)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponIcons[i].SetWeapon(weapons[i]);
        }
    }

    public void UpdateActiveWeapon(int index)
    {
        for (int i = 0; i < WeaponIcons.Length; i++)
        {
            WeaponIcons[i].SetActiveWeapon(i == index);
        }
    }

    public void SetHPBar(float current, float max)
    {
        if (healthBar)
        {
            healthBar.fillMethod = Image.FillMethod.Horizontal;
            healthBar.fillAmount = current / max;
        }
    }
}
