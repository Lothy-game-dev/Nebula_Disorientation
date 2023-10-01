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
    private bool doneInitWeapon;
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
        if (doneInitWeapon)
        {
            LeftWeapon.GetComponent<Weapons>().AIShootBullet();
            RightWeapon.GetComponent<Weapons>().AIShootBullet();
        }
    }

    private void TestDiskInit()
    {
        doneInitWeapon = false;
        FighterAttachedWeapon faw = gameObject.AddComponent<FighterAttachedWeapon>();
        faw.Fighter = gameObject;
        faw.FighterModel = gameObject;
        FighterModelShared fms = gameObject.AddComponent<FighterModelShared>();
        fms.LeftWeaponPos = new Vector2(-0.2f, 0);
        fms.RightWeaponPos = new Vector2(0.2f, 0);
        bool alreadyLeft = false;
        bool alreadyRight = false;
        for (int i = 0; i < Weapons.transform.childCount; i++)
        {
            if (alreadyLeft && alreadyRight)
            {
                break;
            }
            if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals("starblaster"))
            {
                alreadyLeft = true;
                CurrentLeftWeapon = Weapons.transform.GetChild(i).gameObject;
            }
            if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals("starblaster"))
            {
                alreadyRight = true;
                CurrentRightWeapon = Weapons.transform.GetChild(i).gameObject;
            }
        }
        LeftWeapon = Instantiate(CurrentLeftWeapon, transform.position, Quaternion.identity);
        RightWeapon = Instantiate(CurrentRightWeapon, transform.position, Quaternion.identity);
        LeftWeapon.transform.localScale = new Vector2(CurrentLeftWeapon.transform.localScale.x * 2f, CurrentLeftWeapon.transform.localScale.y * 2f);
        RightWeapon.transform.localScale = new Vector2(CurrentRightWeapon.transform.localScale.x * 2f, CurrentRightWeapon.transform.localScale.y * 2f);
        LeftWeapon.SetActive(true);
        Weapons LW = LeftWeapon.GetComponent<Weapons>();
        LW.isLeftWeapon = true;
        LW.Fighter = gameObject;
        LW.Aim = FindObjectOfType<GameController>().Player;
        LW.WeaponPosition = null;
        LW.tracking = false;
        LW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
        RightWeapon.SetActive(true);
        Weapons RW = RightWeapon.GetComponent<Weapons>();
        RW.Fighter = gameObject;
        RW.Aim = FindObjectOfType<GameController>().Player;
        RW.WeaponPosition = null;
        RW.tracking = false;
        RW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
        faw.LeftWeapon = LeftWeapon;
        faw.RightWeapon = RightWeapon;
        faw.AttachWeapon();
        StartCoroutine(WaitForDoneInit());
    }

    private IEnumerator WaitForDoneInit()
    {
        yield return new WaitForSeconds(5f);
        doneInitWeapon = true;
    }
}
