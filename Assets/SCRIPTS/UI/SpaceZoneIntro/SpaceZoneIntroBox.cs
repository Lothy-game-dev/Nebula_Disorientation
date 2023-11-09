using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneIntroBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Board;
    #endregion
    #region NormalVariables
    public string ObjectName;
    public string Type;
    public List<string> ItemNameFinal;
    private GameObject NewBoard;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate Value
    // Group all function that serve the same algorithm
    public void GenerateValue()
    {
        ItemNameFinal = new List<string>();
        if (Type=="Ally")
        {
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetDataAlliesByName(ObjectName);
            string Power = (string)Data["Power"];
            string[] PowerNames = Power.Split("|");
            if (Power.Length != 0)
            {
                if (PowerNames.Length == 1)
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[0]));
                }
                else
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[0]));
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[1]));
                }
                ItemNameFinal.Add("--power--");
            }
            string Weapons = (string)Data["Weapons"];
            string[] WeaponNames = Weapons.Split("|");
            if (WeaponNames.Length == 1)
            {
                if (WeaponNames[0]=="Transport")
                {
                    ItemNameFinal.Add("Transporting");
                } else
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                }
            } else
            {
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[1]));
            }
            if (WeaponNames[0] != "Transport")
                ItemNameFinal.Add("--weapon--");
        } else if (Type=="Enemy")
        {
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetDataEnemyByName(ObjectName);
            string Power = (string)Data["Power"];
            string[] PowerNames = Power.Split("|");
            if (Power.Length != 0)
            {
                if (PowerNames.Length == 1)
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[0]));
                }
                else
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[0]));
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetPowerRealName(PowerNames[1]));
                }
                ItemNameFinal.Add("--power--");
            }
            string Weapons = (string)Data["Weapons"];
            string[] WeaponNames = Weapons.Split("|");
            if (WeaponNames.Length == 1)
            {
                if (WeaponNames[0] == "SuicideBombing")
                {
                    ItemNameFinal.Add("Suicide Bombing");
                }
                else
                {
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                    ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                }
            }
            else
            {
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[0]));
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(WeaponNames[1]));
            }
            if (WeaponNames[0] != "SuicideBombing")
                ItemNameFinal.Add("--weapon--");
        } else if (Type=="Warship")
        {
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetWSByName(ObjectName);
            string SupportWeapon = (string)Data["SupWeapon"];
            string[] SupportWeaponNames = SupportWeapon.Split("|");
            for (int i = 0; i < SupportWeaponNames.Length; i++)
            {
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(SupportWeaponNames[i]));
            }
            ItemNameFinal.Add("--support weapon--");
            string MainWeapon = (string)Data["MainWeapon"];
            string[] MainWeaponNames = MainWeapon.Split("|");
            for (int i=0; i< MainWeaponNames.Length; i++)
            {
                if (MainWeaponNames[i]=="CarrierHatch")
                {
                    ItemNameFinal.Add("<color=#ff0000>Carrier Hatch</color>");
                } else
                ItemNameFinal.Add("<color=#ff0000>" + MainWeaponNames[i] + "</color>");
            }
            ItemNameFinal.Add("--main weapon--");
        } else if (Type=="SpaceStation")
        {
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetSpaceStationByName(ObjectName);
            string SupportWeapon = (string)Data["SupWeapon"];
            string[] SupportWeaponNames = SupportWeapon.Split("|");
            for (int i = 0; i < SupportWeaponNames.Length; i++)
            {
                ItemNameFinal.Add(FindObjectOfType<AccessDatabase>().GetWeaponRealName(SupportWeaponNames[i]));
            }
            ItemNameFinal.Add("--support weapon--");
            string MainWeapon = (string)Data["MainWeapon"];
            string[] MainWeaponNames = MainWeapon.Split("|");
            for (int i = 0; i < MainWeaponNames.Length; i++)
            {
                if (MainWeaponNames[i] == "CarrierHatch")
                {
                    ItemNameFinal.Add("<color=#ff0000>Carrier Hatch</color>");
                }
                else
                    ItemNameFinal.Add("<color=#ff0000>" + MainWeaponNames[i] + "</color>");
            }
            ItemNameFinal.Add("--main weapon--");
        }
        NewBoard = Instantiate(Board, transform.position, Quaternion.identity);
        NewBoard.transform.localScale = new Vector3(NewBoard.transform.localScale.x * Camera.main.transform.localScale.x,
            NewBoard.transform.localScale.y * Camera.main.transform.localScale.y, NewBoard.transform.localScale.z);
        NewBoard.GetComponent<SpaceZoneIntroInfoBoard>().Data = ItemNameFinal;
        NewBoard.GetComponent<SpaceZoneIntroInfoBoard>().GenerateData();
    }
    #endregion
    #region
    private void OnMouseEnter()
    {
        NewBoard.SetActive(true);
    }

    private void OnMouseExit()
    {
        NewBoard.SetActive(false);
    }
    #endregion
}
