using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;

public class TourCard : MonoBehaviour
{
    [SerializeField] private GameObject tourCard;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text destination;
    [SerializeField] private TMP_Text startLocation;
    [SerializeField] private TMP_Text durationDays;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_InputField numOfPeople;
    [SerializeField] private TMP_Dropdown clients;
    [SerializeField] private GameObject bookButton;
    

    public static TourCard instance;

    private List<Client> clientsList = new List<Client>();

    private Tour currentTour = new Tour();

    private void Start()
    {
        instance = this;
    }

    public void OpenCard(Tour tour)
    {
        name.text = tour.TourName;
        startLocation.text = "Start location:\n" + tour.StartLocation;
        destination.text = "Destination:\n" + tour.Destination;
        durationDays.text = "Duration:\n" + tour.DurationDays + " days";
        price.text = "Price:\n" + tour.Price + " rub";
        description.text = tour.Description;

        currentTour = tour;

        if (CurrentLogin.instance.GetLoginType() == CurrentLogin.LoginType.DIRECTOR)
        {
            bookButton.SetActive(false);
        }
        else
        {
            bookButton.SetActive(true);
        }

        tourCard.SetActive(true);

        clientsList.Clear();

        string query = "select * from Clients";
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        while (response.Read())
        {
            Client newClient = new Client();
            newClient.ID_Client = (int)response["ID_Client"];
            newClient.ID_User = (int)response["ID_User"];
            newClient.FIO = (string)response["FIO"];
            newClient.Passport = (string)response["Passport"];
            newClient.DateOfBirth = (DateTime)response["DateOfBirth"];
            newClient.PhoneNumber = (string)response["PhoneNumber"];

            clientsList.Add(newClient);
        }

        response.Close();
    }

    public void GetAllClients()
    {
        int num = 1;
        if (numOfPeople.text.Length != 0)
        {
            num = int.Parse(numOfPeople.text);
        }


        clients.ClearOptions();

        foreach (var c in clientsList)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = c.FIO;
            Debug.Log(c.FIO);
            clients.options.Add(option);
        }

        if (CurrentLogin.instance.GetLoginType() == CurrentLogin.LoginType.CLIENT)
        {
            clients.gameObject.SetActive(false);
        }
        else
        {
            clients.gameObject.SetActive(true);
        }
    }

    public void BookTour()
    {
        int num = 1;
        if (numOfPeople.text.Length != 0)
        {
            num = int.Parse(numOfPeople.text);
        }

        int clientID = 0;
        string managerID = "";

        if (CurrentLogin.instance.GetLoginType() == CurrentLogin.LoginType.CLIENT)
        {
            clientID = CurrentLogin.instance.GetClient().ID_Client;
            managerID = "NULL";
        }
        else
        {
            string clientName = clients.options[clients.value].text;

            foreach (var c in clientsList)
            {
                if (c.FIO == clientName)
                {
                    clientID = c.ID_Client;
                    break;
                }
            }

            Debug.Log(clientID);

            managerID = CurrentLogin.instance.GetManager().ID_Manager.ToString();
        }


        string query = "exec AddPurchasedTour " + currentTour.ID_Tour + " , " + managerID + ", " + clientID + ", " +
                       num;
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        response.Close();
    }
}