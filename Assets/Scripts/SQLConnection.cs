using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UnityEngine;

public class SQLConnection : MonoBehaviour
{
    public SqlConnection connection;

    public static SQLConnection instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        // Build connection string
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = "MCDMITRY"; // update me
        builder.UserID = "unitylogin"; // update me
        builder.Password = "unitylogin"; // update me
        builder.InitialCatalog = "master";

        // Connect to SQL
        Debug.Log("Connecting to SQL Server ... ");
        connection = new SqlConnection(builder.ConnectionString);

        connection.Open();
        Debug.Log("Done.");
        Debug.Log("Current database is " + connection.Database);

        SqlCommand command = new SqlCommand("use TourAgency", connection);

        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log(String.Format("{0}, {1}",
                reader[0], reader[1]));
        }

        Debug.Log("Depth " + reader.Depth);
        
        Debug.Log("now database is " + connection.Database);
        
        reader.Close();
        
        command = new SqlCommand("select *from Users", connection);
        reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            string row = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row += reader[i] + ", ";
            }
            Debug.Log(row);
        }
        
        
        Debug.Log("reader + " + reader);

        instance = this;
    }

    public SqlDataReader SendQuery(string query)
    {
        SqlCommand command = new SqlCommand(query, connection);

        return command.ExecuteReader();
    }
}