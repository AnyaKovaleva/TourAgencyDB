using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentLogin : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private TMP_Text currentlyLoggedInName;
    public enum LoginType
    {
        NOT_LOGGED_IN,
        CLIENT,
        MANAGER,
        DIRECTOR
    }

    private LoginType loginType;
    private Manager loggedInManager;
    private Client loggedInClient;

    public static CurrentLogin instance;

    private void Awake()
    {
        loginType = LoginType.NOT_LOGGED_IN;
        instance = this;
    }

    public void SetManager(Manager manager)
    {
        loggedInManager = manager;
        loginType = LoginType.MANAGER;
        currentlyLoggedInName.text = loggedInManager.FIO;
        loginPanel.SetActive(false);
        AccountPanel.instance.Set(loginType);
    }

    public void SetClient(Client client)
    {
        loggedInClient = client;
        loginType = LoginType.CLIENT;
        currentlyLoggedInName.text = loggedInClient.FIO;
        loginPanel.SetActive(false);
        AccountPanel.instance.Set(loginType);
    }

    public void SetDirector()
    {
        loginType = LoginType.DIRECTOR;
        currentlyLoggedInName.text = "Director";
        loginPanel.SetActive(false);
        AccountPanel.instance.Set(loginType);
    }

    public void LogOut()
    {
        loginType = LoginType.NOT_LOGGED_IN;
        loginPanel.SetActive(true);
    }

    public LoginType GetLoginType()
    {
        return loginType;
    }

    public Manager GetManager()
    {
        return loggedInManager;
    }

    public Client GetClient()
    {
        return loggedInClient;
    }
    
    
}