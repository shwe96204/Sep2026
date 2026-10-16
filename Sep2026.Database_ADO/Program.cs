// ADO.Net SqlConnection Example
// using System.Data;
// using Microsoft.Data.SqlClient;

// SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
// sqlConnectionStringBuilder.DataSource = "."; //sever name
// sqlConnectionStringBuilder.InitialCatalog = "sep_2026"; // database name
// sqlConnectionStringBuilder.UserID = "sa";
// sqlConnectionStringBuilder.Password = "sasa@123";
// sqlConnectionStringBuilder.TrustServerCertificate = true;

// SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
// connection.Open();

// // SqlCommand cmd = new SqlCommand("select * from Customers", connection);
// // SqlDataAdapter adapter = new SqlDataAdapter(cmd);
// // DataTable dt = new DataTable();
// // adapter.Fill(dt);

// string query = @"insert into [dbo].[Customers] 
//     (CustomerId, CustomerName, ContactName, Address, City, PostalCode, Country) 
//     values (95, 'Shwe Wutt Hmone', 'Hmone Hmone', '123 Main St', 'Mandalay', '10001', 'USA')";

// SqlCommand cmd = new SqlCommand(query, connection);
// cmd.ExecuteNonQuery();

// connection.Close();

// Console.ReadLine();
// Console.WriteLine("Data inserted successfully.");

using Sep2026.Database_ADO.Sutdent_CRUD;

SutdentRepo studentRepo = new SutdentRepo();
studentRepo.Start();
