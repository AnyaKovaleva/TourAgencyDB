using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

public class Manager
{
    public int ID_Manager;
    public int ID_User;
    public string FIO;
    public string Passport;
    public string PhoneNumber;
    public DateTime DateOfBirth;
    public DateTime DateOfEmployment;
}

public class Client
{
    public int ID_Client;
    public int ID_User;
    public string FIO;
    public string Passport;
    public DateTime DateOfBirth;
    public string PhoneNumber;
}

public class LoginToAccount : MonoBehaviour
{
    [SerializeField] private TMP_InputField loginInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private bool encryptPasswords;
    
    private string login = "";

    private string password = "";

    public void SetLogin()
    {
        login = loginInputField.text;
    }

    public void SetPassword()
    {
        password = passwordInputField.text;
    }

    public void Login()
    {
        SetLogin();
        SetPassword();
        
        Debug.Log("login " + login);
        Debug.Log("Password " + password);

        string query = "select ID_User from Users where Login ='"+ login +"' and Password ='" + password + "'";

        if (encryptPasswords)
        {
            string encryptedPassword = Shifrovka(password, "p");
            Debug.Log("encrypted password");
            query = "select ID_User from Users where Login ='"+ login +"' and Password ='" + encryptedPassword + "'";
        }
        

        SqlDataReader response = SQLConnection.instance.SendQuery(query);
        DataTable table = response.GetSchemaTable();

        if (!response.HasRows)
        {
            Debug.Log("No user found");
            errorText.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("User found");
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Debug.Log(String.Format("{0} = {1}",
                        column.ColumnName, row[column.ColumnName]));
                }
            }

            int userID = 0;
            while (response.Read())
            {
                userID = int.Parse(response[0].ToString());
            }
            
            response.Close();
            
            Debug.Log("user id = " + userID);

            Manager manager = FindManager(userID);

            if (manager != null)
            {
                Debug.Log("its manager");
                CurrentLogin.instance.SetManager(manager);
            }
            else
            {
                Client client = FindClient(userID);
                if (client != null)
                {
                    Debug.Log("its client");
                    CurrentLogin.instance.SetClient(client);
                }
                else
                {
                    Debug.Log("its DIRECTOR");
                    CurrentLogin.instance.SetDirector();
                }
            }
            
        
            errorText.gameObject.SetActive(false);
        }
        
        response.Close();
        //loginInputField.text = "";
        //passwordInputField.text = "";

        //password = "";
        //login = "";


    }

    private Manager FindManager(int ID_User)
    {
        string query = "select * from Managers where ID_User ='"+ ID_User +"'";
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        Manager manager = new Manager();
        
        if (response.HasRows)
        {
            while (response.Read())
            {
                manager.ID_Manager = (int)response["ID_Manager"];
                manager.ID_User = (int)response["ID_User"];
                manager.FIO = (string)response["FIO"];
                manager.Passport = (string)response["Passport"];
                manager.PhoneNumber = (string)response["PhoneNumber"];
                manager.DateOfBirth = (DateTime)response["DateOfBirth"];
                manager.DateOfEmployment = (DateTime)response["DateOfEmployment"];
            }
            Debug.Log(manager.FIO);
        }
        else
        {
            manager = null;
        }
        
        response.Close();

        return manager;
    } 
    
    private Client FindClient(int ID_User)
    {
        string query = "select * from Clients where ID_User ='"+ ID_User +"'";
        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        Client client = new Client();
        
        if (response.HasRows)
        {
            while (response.Read())
            {
                client.ID_Client = (int)response["ID_Client"];
                client.ID_User = (int)response["ID_User"];
                client.FIO = (string)response["FIO"];
                client.Passport = (string)response["Passport"];
                client.DateOfBirth = (DateTime)response["DateOfBirth"];
                client.PhoneNumber = (string)response["PhoneNumber"];
            }
            Debug.Log(client.FIO);
        }
        else
        {
            client = null;
        }

        
        response.Close();

        return client;
    }
    
     //метод шифрования строки
        public static string Shifrovka(string ishText, string pass,
               string sol = "doberman", string cryptographicAlgorithm = "SHA1",
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
               int keySize = 256)
        {
            if (string.IsNullOrEmpty(ishText))
                return "";
 
            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] ishTextB = Encoding.UTF8.GetBytes(ishText);
 
            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);
            RijndaelManaged symmK = new RijndaelManaged();
            symmK.Mode = CipherMode.CBC;
 
            byte[] cipherTextBytes = null;
 
            using (ICryptoTransform encryptor = symmK.CreateEncryptor(keyBytes, initVecB))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(ishTextB, 0, ishTextB.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
 
            symmK.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }
 
        //метод дешифрования строки
        public static string DeShifrovka(string ciphText, string pass,
               string sol = "doberman", string cryptographicAlgorithm = "SHA1",
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
               int keySize = 256)
        {
            if (string.IsNullOrEmpty(ciphText))
                return "";
 
            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] cipherTextBytes = Convert.FromBase64String(ciphText);
 
            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);
 
            RijndaelManaged symmK = new RijndaelManaged();
            symmK.Mode = CipherMode.CBC;
 
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;
 
            using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
            {
                using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                    {
                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        mSt.Close();
                        cryptoStream.Close();
                    }
                }
            }
 
            symmK.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }
 
    
}
