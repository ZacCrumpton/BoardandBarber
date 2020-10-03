using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardandBarber.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;


namespace BoardandBarber.Data
{
    public class CustomerRepository
    {
        static List<Customer> _customers = new List<Customer>();

        const string _connectionString = "Server=localhost;Database=BoardAndBarber;Trusted_Connection=True;";


        public void Add(Customer customerToAdd)
        {

            var sql = @"INSERT INTO [dbo].[Customer]
                               ([Name]
                               ,[Birthday]
                               ,[FavoriteBarber]
                               ,[Notes])
                        Output inserted.id
                        VALUES
                               (@name,@birthday,@favoritebarber,@notes)";

            using var connection = new SqlConnection(_connectionString);

            var cmd = connection.CreateCommand();

            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("name", customerToAdd.Name);
            cmd.Parameters.AddWithValue("birthday", customerToAdd.Birthday);
            cmd.Parameters.AddWithValue("favoritebarber", customerToAdd.FavoriteBarber);
            cmd.Parameters.AddWithValue("notes", customerToAdd.Notes);

            connection.Open();


            var newId = (int) cmd.ExecuteScalar();

            customerToAdd.Id = newId;


            //var newId = 1;

            //if (_customers.Count > 0)
            //{ 
            //    newId = _customers.Select(c => c.Id).Max() + 1;
            //}

            //customerToAdd.Id = newId;

            //_customers.Add(customerToAdd);
        }


        public Customer GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            //what do we want to tell sql server to do?
            //sql command can lead to issues
            
            var command = connection.CreateCommand();
            
            var query = $@"select *
                          from Customer
                          where id = {id}";

            command.CommandText = query;

            //command.ExecuteNonQuery();
            //run this query and i don't care about the results
            //insert update or delete
            //non read crud methods

            //command.ExecuteScalar();
            //run this query and only return the top leftmost cell

            //command.ExecuteReader();
            //run this query and give me the results, one row at a time

            var reader = command.ExecuteReader();
            //sql server has executed the command and is waiting to give us results



            if (reader.Read()){

                return MapToCustomer(reader);

            }
            else
            {
                return null;
            }

            
            
        }

        public List<Customer> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            var sql = "select * from customer";

            command.CommandText = sql;

            var reader = command.ExecuteReader();
            var customers = new List<Customer>();

            while (reader.Read())
            {
                var customer = MapToCustomer(reader);
                customers.Add(customer);

            }

            return customers;

            //return customers;
        }

            public Customer Update(int id, Customer customer)
        {

            var sql = @"UPDATE [dbo].[Customer]
                            SET [Name] = @name
                                ,[Birthday] = @birthday
                                ,[FavoriteBarber] = @favoriteBarber
                                ,[Notes] = @notes
                            output inserted.*
                            WHERE id = @id";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("name", customer.Name);
            cmd.Parameters.AddWithValue("birthday", customer.Birthday);
            cmd.Parameters.AddWithValue("favoriteBarber", customer.FavoriteBarber);
            cmd.Parameters.AddWithValue("notes", customer.Notes);
            cmd.Parameters.AddWithValue("id", id);

            var reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                return MapToCustomer(reader);
            }

            return null;


            //var customerToUpdate = GetById(id);

            //customerToUpdate.Birthday = customer.Birthday;
            //customerToUpdate.FavoriteBarber = customer.FavoriteBarber;
            //customerToUpdate.Name = customer.Name;
            //customerToUpdate.Notes = customer.Notes;

            //return customerToUpdate;
        }   
        
        public void Remove(int id)
        {

            var sql = @"DELETE 
                        FROM [dbo].[Customer]
                        WHERE @id";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();

            if (rows != 1)
            {
                //do something that is bad
            }

            //var customerToDelete = GetById(id);

            //_customers.Remove(customerToDelete);
        }
        
        Customer MapToCustomer(SqlDataReader reader)
        {
            var customerFromDb = new Customer();

            customerFromDb.Id = (int)reader["id"]; //explicit conversion/cast throws exception
            customerFromDb.Name = reader["Name"] as string; //implicit cast/conversion returns null on failure
            customerFromDb.Birthday = DateTime.Parse(reader["Birthday"].ToString()); //parsing
            customerFromDb.FavoriteBarber = reader["FavoriteBarber"].ToString(); //make it a string
            customerFromDb.Notes = reader["Notes"].ToString(); //make it a string

            return customerFromDb;
        }
    }
}
