using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;

public class DirectorCard : MonoBehaviour
{
    [SerializeField] private TMP_InputField fromDateIF;
    [SerializeField] private TMP_InputField toDateIF;
    [SerializeField] private TMP_Text profitsText;
    [SerializeField] private TMP_Text errorText;
    
    [SerializeField] private TMP_Text infoText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CalculateProfits()
    {
        string query = "select sum(TotalPrice) from PurchasedTours where DateOfPurchase between "+ fromDateIF.text+" and " + toDateIF.text;

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        if (!response.HasRows)
        {
            errorText.gameObject.SetActive(true);
            profitsText.text = "0";
            return;
        }
        
        errorText.gameObject.SetActive(false);

        float profits = 0;
        
        while (response.Read())
        {
            profits = (float)(decimal)response[0];
        }

        profitsText.text = profits.ToString();
        
        response.Close();
    }

    public void SeeAllManagers()
    {
        string query = "select * from Managers";

        Debug.Log("Clients who purchased the most tours");
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        infoText.text = "";
        
        while (response.Read())
        {
            string client = "Manager_ID ";

            client += response["ID_Manager"];
            client += " UserID ";
            client += response["ID_User"];
            client += " FIO ";
            client += response["FIO"];
            client += " Passport ";
            client += response["Passport"];
            client += " PhoneNumber ";
            client += response["PhoneNumber"];
            client += " Date of birth ";
            client += response["DateOfBirth"];
            client += " Date of employment ";
            client += response["DateOfEmployment"];
            
            infoText.text += client + "\n\n";
        }

    
    }

    public void SeeManagersWhoSoldTheMostTours()
    {
        string query = "select * from ManagersWhoSoldTheMostTours order by NumOfSoldTours desc";

       // Debug.Log("Clients who purchased the most tours");
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        infoText.text = "";
        
        while (response.Read())
        {
            string client = "";

            client += response["FIO"];
            client += " bought ";
            client += response["NumOfSoldTours"];
            client += " tours";

            infoText.text += client + "\n";
        }

    }
    
}
