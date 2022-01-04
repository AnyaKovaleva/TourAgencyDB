using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TourObject : MonoBehaviour
{
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text destination;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text price;

    private Tour thisTour = new Tour();
    
    public void SetUp(Tour tour)
    {
        thisTour = tour;

        name.text = thisTour.TourName;
        destination.text = "Destination:\n" + thisTour.Destination;
        duration.text = "Duration:\n" + thisTour.DurationDays + " days";
        price.text = "Price:\n" + thisTour.Price + " rub";
    }

    public void OpenTourcard()
    {
        TourCard.instance.OpenCard(thisTour);
    }

    public string GetTourName()
    {
        return thisTour.TourName;
    }
}
