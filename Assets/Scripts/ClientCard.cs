using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientCard : MonoBehaviour
{
    [SerializeField] private TMP_Text fio;
    [SerializeField] private TMP_Text passport;
    [SerializeField] private TMP_Text phoneNumber;
    [SerializeField] private TMP_Text dateOfBirth;

    private Client client;
    
    public static ClientCard instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Open()
    {
        client = CurrentLogin.instance.GetClient();

        fio.text = "FIO: " + client.FIO;
        passport.text = "Passport: " + client.Passport;
        phoneNumber.text = "Phone number: " + client.PhoneNumber;
        dateOfBirth.text = "Date of birth: " + client.DateOfBirth;
    }

}
