using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountPanel : MonoBehaviour
{
    [SerializeField] private GameObject managerCard;
    [SerializeField] private GameObject clientCard;
    [SerializeField] private GameObject directorCard;

    public static AccountPanel instance;
    // Start is called before the first frame update
    void Start()
    {
        managerCard.SetActive(false);
        clientCard.SetActive(false);
        directorCard.SetActive(false);
        instance = this;
    }

    public void Set(CurrentLogin.LoginType loginType)
    {
        managerCard.SetActive(false);
        clientCard.SetActive(false);
        directorCard.SetActive(false);
        switch (loginType)
        {
            case CurrentLogin.LoginType.MANAGER: managerCard.SetActive(true);
                ManagerCard.instance.Open();
                break;
            case CurrentLogin.LoginType.CLIENT: clientCard.SetActive(true);
               ClientCard.instance.Open();
                break;
            case CurrentLogin.LoginType.DIRECTOR: directorCard.SetActive(true);
                break;
        }
    }
    
}
