using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisk : EnemyShared
{
    public GameObject Weapons;
    public Rigidbody2D rb;
    public float changeDirTimer;
    private bool test;
    private Vector2 check;
    private GameObject CurrentLeftWeapon;
    private GameObject CurrentRightWeapon;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        changeDirTimer = 2.5f;
        test = false;
        check = new Vector2(0, 300);
        StartEnemy(500f);
        CalculateVelocity(!test ? check : -check);
        InitializeFighter();
        TestDiskInit();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
        if (!isFrozen && !isSFBFreeze)
        {
            CalculateVelocity(!test ? check : -check);
            /*CalculateVelocity(new Vector2(0, 0));*/
        }
        if (changeDirTimer > 0f)
        {
            changeDirTimer -= Time.deltaTime;
        } else
        {
            changeDirTimer = 5f;
            test = !test;
        }
    }

    private void TestDiskInit()
    {
        FighterAttachedWeapon faw = gameObject.AddComponent<FighterAttachedWeapon>();
        faw.enabled = false;
        faw.Fighter = gameObject;
        faw.FighterModel = gameObject;
        FighterModelShared fms = gameObject.AddComponent<FighterModelShared>();
        fms.LeftWeaponPos = new Vector2(-10, 0);
        fms.RightWeaponPos = new Vector2(10, 0);
        bool alreadyLeft = false;
        bool alreadyRight = false;
        for (int i = 0; i < Weapons.transform.childCount; i++)
        {
            if (alreadyLeft && alreadyRight)
            {
                break;
            }
            if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals("pulsecannon"))
            {
                alreadyLeft = true;
                LeftWeapon = Weapons.transform.GetChild(i).gameObject;
            }
            if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals("pulsecannon"))
            {
                alreadyRight = true;
                RightWeapon = Weapons.transform.GetChild(i).gameObject;
            }
        }
        CurrentLeftWeapon = Instantiate(LeftWeapon, transform.position, Quaternion.identity);
        CurrentRightWeapon = Instantiate(RightWeapon, transform.position, Quaternion.identity);
        CurrentLeftWeapon.transform.localScale = new Vector2(LeftWeapon.transform.localScale.x * 2f, LeftWeapon.transform.localScale.y * 2f);
        CurrentRightWeapon.transform.localScale = new Vector2(RightWeapon.transform.localScale.x * 2f, RightWeapon.transform.localScale.y * 2f);
        LeftWeapon.SetActive(true);
        Weapons LW = LeftWeapon.GetComponent<Weapons>();
        LW.isLeftWeapon = true;
        LW.Fighter = gameObject;
        LW.Aim = FindObjectOfType<GameController>().Player;
        LW.WeaponPosition = null;
        RightWeapon.SetActive(true);
        Weapons RW = RightWeapon.GetComponent<Weapons>();
        RW.Fighter = gameObject;
        RW.Aim = FindObjectOfType<GameController>().Player;
        RW.WeaponPosition = null;
        faw.LeftWeapon = CurrentLeftWeapon;
        faw.RightWeapon = CurrentRightWeapon;
        faw.enabled = true;
    }
}
