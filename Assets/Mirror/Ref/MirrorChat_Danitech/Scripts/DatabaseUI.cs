using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;

public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

    [Header("ConnectionInfo")]
    [SerializeField] string m_Ip = "127.0.0.1";
    [SerializeField] string m_DbName = "test";
    [SerializeField] string m_UId = "root";
    [SerializeField] string m_Password = "1234";

    private bool m_isConnectTestComplete; //중요하지 않음음

    private static MySqlConnection m_DbConnection;

    private bool ConnectTest()
    {
        string connectStr = $"Server={m_Ip};Database={m_DbName};Uid={m_UId};Pwd={m_Password};";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectStr))
            {
                m_DbConnection = conn;
                conn.Open();
            }

            Text_Log.text = "DB 연결을 성공했습니다!";
            return true;
        }
        catch(Exception e)
        {
            Debug.LogWarning($"e : {e.ToString()}");
            Text_Log.text = "DB 연결 실패";
            return false;
        }
    }

    public void OnClick_TestDBConnect()
    {
        m_isConnectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendQuery()
    {
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }

}
