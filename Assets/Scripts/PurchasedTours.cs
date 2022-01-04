using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PurchasedTour
{
    public int ID_Purchase;
    public DateTime DateOfPurchase;
    public string TourName;
    public string Manager;
    public string Client;
    public string Status;
    public int NumberOfPeople;
    public float TotalPrice;
}

[Serializable]
public class Status
{
    public int ID_Status;
    public string StatusName;
}

public class PurchasedTours : MonoBehaviour
{
    [SerializeField] private GameObject purchasedTourObject;
    [SerializeField] private GameObject purchasedToursList;

    private List<Status> statuses = new List<Status>();
    private List<PurchasedTour> purchasedTours = new List<PurchasedTour>();

    private List<PurchasedTourObject> displayedTours = new List<PurchasedTourObject>();

    public static PurchasedTours instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdateStatuses();
        UpdateListOfPurchasedTours();
    }

    public void UpdateListOfPurchasedTours()
    {
        string query = "select * from PurchasedToursWithFullInfo";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        List<PurchasedTour> newTours = new List<PurchasedTour>();
        
        
        while (response.Read())
        {
            PurchasedTour newPurchasedTour = new PurchasedTour();

            newPurchasedTour.ID_Purchase = (int)response["ID_Purchase"];
            newPurchasedTour.DateOfPurchase = (DateTime)response["DateOfPurchase"];
            newPurchasedTour.TourName = (string)response["TourName"];
            newPurchasedTour.Manager = (string)response["Manager"];
            newPurchasedTour.Client = (string)response["Client"];
            newPurchasedTour.Status = (string)response["StatusName"];
            newPurchasedTour.NumberOfPeople = (int)response["NumberOfPeople"];
            newPurchasedTour.TotalPrice = (float)(decimal)response["TotalPrice"];

            newTours.Add(newPurchasedTour);
        }

        response.Close();

        if (newTours.Count != purchasedTours.Count)
        {
            purchasedTours = newTours;
        }

            foreach (var tour in purchasedTours)
        {
            bool needToAdd = true;

            foreach (var displayedTour in displayedTours)
            {
                PurchasedTour purchasedTour = displayedTour.GetPurchasedTour();
                if (purchasedTour.TourName == tour.TourName)
                {
                    needToAdd = false;
                }
            }

            if (needToAdd)
            {
                GameObject newTour = Instantiate(purchasedTourObject, purchasedToursList.transform);
                newTour.GetComponentInChildren<PurchasedTourObject>().SetUp(tour);
                displayedTours.Add(newTour.GetComponentInChildren<PurchasedTourObject>());
            }
        }

        CurrentLogin.LoginType loginType = CurrentLogin.instance.GetLoginType();

        switch (loginType)
        {
            case CurrentLogin.LoginType.MANAGER:
                Manager manager = CurrentLogin.instance.GetManager();
                foreach (var tour in displayedTours)
                {
                    tour.UpdateStatus();
                    PurchasedTour purchasedTour = tour.GetPurchasedTour();
                    if (purchasedTour.Manager == manager.FIO || purchasedTour.Manager =="NONE")
                    {
                        tour.gameObject.SetActive(true);
                    }
                    else
                    {
                        tour.gameObject.SetActive(false);
                    }
                }
                break;
            
            case CurrentLogin.LoginType.CLIENT:
                Client client = CurrentLogin.instance.GetClient();
                foreach (var tour in displayedTours)
                {
                    tour.UpdateStatus();

                    PurchasedTour purchasedTour = tour.GetPurchasedTour();
                    if (purchasedTour.Client == client.FIO)
                    {
                        tour.gameObject.SetActive(true);
                    }
                    else
                    {
                        tour.gameObject.SetActive(false);
                    }
                }
                break;
            
            case CurrentLogin.LoginType.DIRECTOR:
                foreach (var tour in displayedTours)
                {
                    tour.UpdateStatus();
                    tour.gameObject.SetActive(true);
                }
                break;
        }

    }
    
    public void UpdateStatuses()
    {
        string query = "select * from Status";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        while (response.Read())
        {
            Status newStatus = new Status();

            newStatus.ID_Status = (int)response["ID_Status"];
            newStatus.StatusName = (string)response["StatusName"];

            statuses.Add(newStatus);
        }

        response.Close();
    }

    public List<Status> GetAllStatuses()
    {
        return statuses;
    }
}