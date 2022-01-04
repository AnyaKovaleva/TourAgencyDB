using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasedTourObject : MonoBehaviour
{
    [SerializeField] private TMP_Text ID_Purchase;
    [SerializeField] private TMP_Text DateOfPurchase;
    [SerializeField] private TMP_Text TourName;
    [SerializeField] private TMP_Text Manager;
    [SerializeField] private TMP_Text Client;
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private TMP_Dropdown Status;
    [SerializeField] private TMP_Text NumberOfPeople;
    [SerializeField] private TMP_Text TotalPrice;


    [SerializeField] private Button changeStatusButton;
    
    private PurchasedTour purchasedTour;
    
    public void SetUp(PurchasedTour tour)
    {
        purchasedTour = tour;

        ID_Purchase.text = "Purchase ID:\n" + purchasedTour.ID_Purchase;
        DateOfPurchase.text = "Date of purchase:\n" + purchasedTour.DateOfPurchase;
        TourName.text = purchasedTour.TourName;
        Manager.text = "Manager: " + purchasedTour.Manager;
        Client.text = "Client: " + purchasedTour.Client;
        NumberOfPeople.text = "Number of people: " + purchasedTour.NumberOfPeople;
        TotalPrice.text = "Total price: " + purchasedTour.TotalPrice + " rub";

        List<Status> statuses = PurchasedTours.instance.GetAllStatuses();
        
        Status.ClearOptions();

        foreach (var status in statuses)
        {
            TMP_Dropdown.OptionData newStatus = new TMP_Dropdown.OptionData();
            newStatus.text = status.StatusName;
            Status.options.Add(newStatus);
        }

        Dropdown.OptionData statusToFind = new Dropdown.OptionData();
        statusToFind.text = tour.Status;

        int index = 0;
        foreach (var option in Status.options)
        {
            if (option.text == tour.Status)
            {
                break;
            }

            index++;
        }

        Status.value = index;

        if (CurrentLogin.instance.GetLoginType() == CurrentLogin.LoginType.CLIENT)
        {
            changeStatusButton.interactable = false;
            changeStatusButton.gameObject.SetActive(false);
            Status.gameObject.SetActive(false);
            StatusText.text = "Status: " + purchasedTour.Status;

        }
        else
        {
            changeStatusButton.interactable = true;

            changeStatusButton.gameObject.SetActive(true);
            Status.gameObject.SetActive(true);
            StatusText.text = "Status: ";
        }

    }

    public void UpdateStatus()
    {
        if (CurrentLogin.instance.GetLoginType() != CurrentLogin.LoginType.MANAGER)
        {
            changeStatusButton.interactable = false;
            changeStatusButton.gameObject.SetActive(false);
            Status.gameObject.SetActive(false);
            StatusText.text = "Status: " + purchasedTour.Status;

        }
        else
        {
            changeStatusButton.interactable = true;

            changeStatusButton.gameObject.SetActive(true);
            Status.gameObject.SetActive(true);
            StatusText.text = "Status: ";
        }
        
        
        
    }

    public PurchasedTour GetPurchasedTour()
    {
        return purchasedTour;
    }

    public void ChangeStatus()
    {
        List<Status> statusList = PurchasedTours.instance.GetAllStatuses();

        string statusName = Status.options[Status.value].text;

        Status status = new Status();

        foreach (var s in statusList)
        {
            if (s.StatusName == statusName)
            {
                status = s;
            }
        }
        
        Debug.Log(status.StatusName + " , " + status.ID_Status);

        string query = "exec ChangePurchasedTourStatus " + purchasedTour.ID_Purchase + ", " + status.ID_Status;

        SQLConnection.instance.SendQuery(query);

    }
    
}
