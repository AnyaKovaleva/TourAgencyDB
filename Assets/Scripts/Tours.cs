using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Tour
{
    public int ID_Tour;

    public string TourName;

    public string Destination;

    public string StartLocation;

    public int DurationDays;

    public float Price;

    public string Description;
}

public class Tours : MonoBehaviour
{
    [SerializeField] private List<Tour> tours;
    [SerializeField] private List<Tour> allTours;
    [SerializeField] private List<Tour> mostPopularTours;

    [SerializeField] private GameObject toursList;

    [SerializeField] private GameObject tourObject;

    [SerializeField] private TMP_Text header;
    [SerializeField] private Button showAllToursButton;
    [SerializeField] private Button showMostPopularToursButton;

    [SerializeField] private GameObject morePanel;
    [SerializeField] private TMP_InputField findTourByNameIF;
    [SerializeField] private TMP_Text noTourFoundText;

    private List<TourObject> displayedTours;

    // Start is called before the first frame update
    void Start()
    {
        tours = new List<Tour>();
        allTours = new List<Tour>();
        mostPopularTours = new List<Tour>();
        displayedTours = new List<TourObject>();
        ShowAllTours();
    }

    public void ShowAllTours()
    {
        header.text = "Tours";
        showAllToursButton.interactable = false;
        showMostPopularToursButton.interactable = true;

        string query = "select * from Tours";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        while (response.Read())
        {
            Tour newTour = new Tour();

            newTour.ID_Tour = (int)response["ID_Tour"];
            newTour.TourName = (string)response["TourName"];
            newTour.Destination = (string)response["Destination"];
            newTour.StartLocation = (string)response["StartLocation"];
            newTour.DurationDays = (int)response["DurationDays"];
            newTour.Price = (float)(decimal)response["Price"];
            newTour.Description = (string)response["Description"];

            allTours.Add(newTour);
        }

        if (allTours.Count > tours.Count)
        {
            tours = allTours;
        }

        foreach (var tour in displayedTours)
        {
            tour.GetComponentInParent<Image>().gameObject.SetActive(true);
        }

        response.Close();

        DisplayToursList(true);

        toursList.SetActive(false);
        toursList.SetActive(true);
        
    }

    public void ShowMostPopularTours()
    {
        header.text = "Most popular tours";

        showAllToursButton.interactable = true;
        showMostPopularToursButton.interactable = false;

        string query = "select * from MostPopularTours order by TimesPurchased";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        List<string> popularTours = new List<string>();

        while (response.Read())
        {
            popularTours.Add((string)response["Name"]);
        }

        foreach (var tour in displayedTours)
        {
            if (!popularTours.Contains(tour.GetTourName()))
            {
                tour.GetComponentInParent<Image>().gameObject.SetActive(false);
            }
        }

        response.Close();
        
        toursList.SetActive(false);
        toursList.SetActive(true);
    }

    private void DisplayToursList(bool allTours)
    {
        foreach (var tour in tours)
        {
            bool needToSpawn = true;

            foreach (var displayedTour in displayedTours)
            {
                if (displayedTour.GetTourName() == tour.TourName)
                {
                    needToSpawn = false;
                }
            }

            if (needToSpawn)
            {
                GameObject newTour = Instantiate(tourObject, toursList.transform);
                newTour.GetComponentInChildren<TourObject>().SetUp(tour);
                displayedTours.Add(newTour.GetComponentInChildren<TourObject>());
            }
        
        }
    }

    public void FindTourByName()
    {
        Tour tourToFind = null;
        foreach (var tour in tours)
        {
            if (tour.TourName == findTourByNameIF.text)
            {
                tourToFind = tour;
            }
        }

        if (tourToFind != null)
        {
            noTourFoundText.gameObject.SetActive(false);
            TourCard.instance.OpenCard(tourToFind);
            morePanel.SetActive(false);
        }
        else
        {
            noTourFoundText.gameObject.SetActive(true);
        }
    }
}