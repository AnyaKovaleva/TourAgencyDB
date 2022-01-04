using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

public class UpdateClientInfo : MonoBehaviour
{
     [SerializeField] private TMP_InputField login;
    [SerializeField] private TMP_Text loginError;
    
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField fio;
    
    [SerializeField] private TMP_InputField passport;
    [SerializeField] private TMP_Text passportError;
    
    [SerializeField] private TMP_InputField dateOfBirth;
    [SerializeField] private TMP_Text dateOfBirthError;
    
    [SerializeField] private TMP_InputField phoneNumber;
    [SerializeField] private TMP_Text phoneNumberError;

    public UpdateClientInfo instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void SetInputFields()
    {
        Client client = CurrentLogin.instance.GetClient();
        string login = "";
        string password= "";

        string query = "select * from Users where ID_User = '" + client.ID_User + "'";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        while (response.Read())
        {
            login = (string)response["Login"];
            password = (string)response["Password"];
        }

        response.Close();
        this.login.text = login;

        this.password.text = DeShifrovka(password, "p");

        this.fio.text = client.FIO;
        this.passport.text = client.Passport;
        
        this.dateOfBirth.text = client.DateOfBirth.ToShortDateString();
        this.phoneNumber.text = client.PhoneNumber;
        

    }

    public void UpdateInfo()
    {
        string login;
        string password;

        string fio;
        string passport;

        string dateOfBirth;
        string phoneNumber;

        if (LoginCorrect(this.login.text))
        {
            login = this.login.text;
            loginError.gameObject.SetActive(false);
        }
        else
        {
            loginError.gameObject.SetActive(true);
            return;
        }

        password = this.password.text;
        fio = this.fio.text;

        if (PassportCorrect(this.passport.text))
        {
            passport = this.passport.text;
            passportError.gameObject.SetActive(false);
        }
        else
        {
            passportError.gameObject.SetActive(true);
            return;
        }


        if (DateCorrect(this.dateOfBirth.text))
        {
            dateOfBirth = this.dateOfBirth.text;
            dateOfBirthError.gameObject.SetActive(false);
        }
        else
        {
            dateOfBirthError.gameObject.SetActive(true);
            return;
        }

        if (PhoneNumberCorrect(this.phoneNumber.text))
        {
            phoneNumber = this.phoneNumber.text;
            phoneNumberError.gameObject.SetActive(false);
        }
        else
        {
            phoneNumberError.gameObject.SetActive(true);
            return;
        }

        string encriptedPassword = Shifrovka(password, "p");
        Debug.Log("encrypted password " + encriptedPassword);

        string decriptedPassword = DeShifrovka(encriptedPassword, "p");
        Debug.Log("decripted password " + decriptedPassword);
        
        Debug.Log("all good");

        Client client = CurrentLogin.instance.GetClient();

        string query = "exec UpdateClient '" + client.ID_Client + "' , '" + client.ID_User + "' , '" + login + "' , '" + encriptedPassword + "' , '" + fio + "' , '" + passport +
                       "' , '" + dateOfBirth + "' , '" + phoneNumber + "'";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);
        response.Close();


    }

    private bool LoginCorrect(string login)
    {
        string query = "select Login from Users where Login = '" + login + "'";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        if (response.HasRows)
        {
            response.Close();
            return false;
        }
        
        response.Close();

        return true;
    }

    private bool PassportCorrect(string passport)
    {
        string query = "select Passport from Clients where Passport = '" + passport + "'";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        if (response.HasRows)
        {
            response.Close();
            return false;
        }
        
        response.Close();

        return true;
    }

    private bool DateCorrect(string date)
    {
        if (date.Length != 10)
        {
            return false;
        }

        string year = "";
        year += date[0];
        year += date[1];
        year += date[2];
        year += date[3];


        if (!int.TryParse(year, out int n))
        {
            return false;
        }

        if (date[4] != '-')
        {
            return false;
        }

        string month = "";
        month += date[5];
        month += date[6];

        int monthInt;
        
        if (!int.TryParse(month, out monthInt))
        {
            return false;
        }

        if (monthInt > 12)
        {
            return false;
        }

        if (date[7] != '-')
        {
            return false;
        }
        
        string day = "";
        day += date[8];
        day += date[9];

        int dayInt;
        
        if (!int.TryParse(day, out dayInt))
        {
            return false;
        }

        if (dayInt > 31)
        {
            return false;
        }
        
        return true;
    }

    private bool PhoneNumberCorrect(string phoneNumber)
    {
        string query = "select PhoneNumber from Clients where PhoneNumber = '" + phoneNumber + "'";

        SqlDataReader response = SQLConnection.instance.SendQuery(query);

        if (response.HasRows)
        {
            response.Close();
            return false;
        }
        
        response.Close();

        return true;
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
