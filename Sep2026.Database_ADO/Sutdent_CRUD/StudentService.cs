using System;
using Microsoft.Data.SqlClient;
using System.Data;
using Sep2026.Database_ADO.CommonFunction;

namespace Sep2026.Database_ADO.Sutdent_CRUD;

public class StudentService
{
    private SqlConnectionStringBuilder sqlConnectionStringBuilder1 = new SqlConnectionStringBuilder()
    {
        DataSource = ".",
        InitialCatalog = "sep_2026",
        UserID = "sa",
        Password = "sasa@123",
        TrustServerCertificate = true
    };

    //Create: Register new student
    public void CreateStudent(string no, string name, string classNo)
    {
        SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder1.ConnectionString);
        // sql connection open
        sqlConnection.Open();

        string query = $@"INSERT INTO [dbo].[Students] (StudentNo, Name, ClassNo) VALUES ('{no}', '{name}', '{classNo}')";
        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
        sqlCommand.ExecuteNonQuery();
        ConsoleHelper.ShowSuccessMessage("Student registered successfully.");

        // sql connection close
        sqlConnection.Close();
    }

    //Read: Show all Student
    public void ShowAllStudent()
    {
        SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder1.ConnectionString);
        sqlConnection.Open();
        
        string query = "SELECT [Id], [StudentNo], [Name], [ClassNo] FROM [dbo].[Students] ORDER BY [Id]";
        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
        SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
        DataTable table = new DataTable(); // System data table to receive data from database
        adapter.Fill(table);

        if (table.Rows.Count == 0)
        {
            ConsoleHelper.ShowErrorMessage("No students found.");
        }

        foreach (DataRow item in table.Rows)
        {
            Console.WriteLine($"Id: {item["Id"]} | Student No.: {item["StudentNo"]} | Name: {item["Name"]} | ClassNo: {item["ClassNo"]}");
        }
        sqlConnection.Close();
    }

    // Check whether the student exist or not
    public bool IsStudentExist(int id)
    {
        using SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder1.ConnectionString);
        sqlConnection.Open();

        string query = $@"SELECT COUNT(1) FROM [dbo].[Students] WHERE Id = {id}";
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
        int count = Convert.ToInt32(sqlCommand.ExecuteScalar());

        return count > 0;
    }

    // Edit: update existing student
    public void EditStudent(int id, string no, string name, string classNo)
    {
        SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder1.ConnectionString);
        sqlConnection.Open();

        string query = $@"UPDATE [dbo].[Students] SET [StudentNo] = '{no}', [Name] = '{name}', [ClassNo] = '{classNo}' WHERE [Id] = {id}";
        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
        var row = sqlCommand.ExecuteNonQuery();

        if (row > 0)
        {
            ConsoleHelper.ShowSuccessMessage("Student updated successfully.");
        }
        else
        {
            ConsoleHelper.ShowErrorMessage("Student not found.");
        }

        sqlConnection.Close();
    }

    // Delete: delete student
    public void DeleteStudent(int id)
    {
        SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder1.ConnectionString);
        sqlConnection.Open();

        string query = $@"DELETE FROM [dbo].[Students] WHERE [Id] = {id}";
        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
        var row = sqlCommand.ExecuteNonQuery();
        if (row > 0)
        {
            ConsoleHelper.ShowSuccessMessage("Studnet deleted successfully");
        }
        else
        {
            ConsoleHelper.ShowErrorMessage("Student not found.");
        }
        sqlConnection.Close();
    }

}