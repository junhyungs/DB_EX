using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;

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

    private bool m_isConnectTestComplete; //�߿����� ����

    private static MySqlConnection m_DbConnection;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void SendQuery(string queryStr,string tableName)
    {
        //������ Select ���� �Լ� ȣ��
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);
            Text_DBResult.text = DeformatResult(dataSet);
        }
        else //���ٸ� Insert �Ǵ� Update ���� ����
        {
            Text_DBResult.text = OnInsertOnUpdateRequest(queryStr) ? "����" : "����";
        }
    }
    //sql ���� �Լ��� ����Ϸ��� static �Լ��� �����ؾ���.
    public static bool OnInsertOnUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = m_DbConnection;
            sqlCommand.CommandText = query;

            m_DbConnection.Open();
            sqlCommand.ExecuteNonQuery();
            m_DbConnection.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;

        foreach(DataTable table in dataSet.Tables)
        {
            foreach(DataRow row in table.Rows)
            {
                foreach(DataColumn column in table.Columns)
                {
                    resultStr += $"{column.ColumnName}: {row[column]}\n";
                }
            }
        }

        return resultStr;
    }

    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            m_DbConnection.Open();
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = m_DbConnection;
            sqlCmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);

            m_DbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;    
        }
    }

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

            Text_Log.text = "DB ������ �����߽��ϴ�!";
            return true;
        }
        catch(Exception e)
        {
            Debug.LogWarning($"e : {e.ToString()}");
            Text_Log.text = "DB ���� ����";
            return false;
        }
    }

    public void OnClick_TestDBConnect()
    {
        m_isConnectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendQuery()
    {
        if(m_isConnectTestComplete == false)
        {
            Text_Log.text = "DB ������ ���� �õ��ϼ���";
            return;
        }
        Text_Log.text = string.Empty;

        string query = string.IsNullOrWhiteSpace(Input_Query.text) ? "SELECT U_Name,U_Password FROM user_info"
            : Input_Query.text;

        SendQuery(query, "user_info");
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
