using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;

public class ManagerCard : MonoBehaviour
{
    [SerializeField] private TMP_Text fio;
    [SerializeField] private TMP_Text passport;
    [SerializeField] private TMP_Text phoneNumber;
    [SerializeField] private TMP_Text dateOfBirth;
    [SerializeField] private TMP_Text dateOfEmployment;

    [SerializeField] private TMP_Text infoText;
    private Manager manager;
    
    public static ManagerCard instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Open()
    {
        manager = CurrentLogin.instance.GetManager();

        fio.text = "FIO: " + manager.FIO;
        passport.text = "Passport: " + manager.Passport;
        phoneNumber.text = "Phone number: " + manager.PhoneNumber;
        dateOfBirth.text = "Date of birth: " + manager.DateOfBirth;
        dateOfEmployment.text = "Date of employment: " + manager.DateOfEmployment;
    }

    public void SeeAllPurchasedTours()
    {
        
    }

    public void RegisterNewClient()
    {
        
    }

    public void SeeClientsWhoPurchasedTheMostTours()
    {
        string query = "select * from ClientsWhoBoughtTheMostTours order by NumOfBoughtTours desc";

        Debug.Log("Clients who purchased the most tours");
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        infoText.text = "";
        
        while (response.Read())
        {
            string client = "";

            client += response["FIO"];
            client += " bought ";
            client += response["NumOfBoughtTours"];
            client += " tours";

            infoText.text += client + "\n";
        }
        

    }
    
}
