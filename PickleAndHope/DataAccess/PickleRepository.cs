using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PickleAndHope.Models;

namespace PickleAndHope.DataAccess
{


    // repositories have a collection like interface for messing with data;
    // what I mean is, some of the methods from List like add, remove, LINQ stuff is part of the collection-y sort of interface
    // this looks exactly like what we were doing in front end

    // a lot of code is figuring how to make things concise
    // we can do get stuff inside the browser window. in order to do post stuff, we have to use another tool (postman, insomnia, etc.)
    public class PickleRepository
    {

        // static keyword says this particular field should be shared by every instance of pickles controller; normal fields have new instances of field for each class
        // static simulates longterm storage
        // ***why underscore with pickles? doesn't that mean something else?***
        static List<Pickle> _pickles = new List<Pickle>
        {
            new Pickle
            {
                Type = "Bread and Butter",
                NumberInStock = 5,
                Id = 1
            }
        };

        // Connection String - instructions on how to connect
        const string ConnectionString = "Server = localhost; Database = PickleAndHope; Trusted_Connection = True; ";


        public Pickle Add(Pickle pickle)
        {
            // using Max takes into account the possibility of repeats
            //pickle.Id = _pickles.Max(x => x.Id) + 1;
            //_pickles.Add(pickle);

            // Steps to Success
            // write a query in SQL Server Management Studio to see how it is going to work
            // copy it into C# as a string
            // create a connection, open a connection
            // create a command, set the command text
            // add any parameters you need to add
            // execute it
            // do something with the results
            // those are always going to be the steps

            var sql = @"INSERT INTO Pickle(NumberInStock, Price, Size, Type)
                        OUTPUT inserted.*
                        VALUES(@NumberInStock, @Price, @Size, @Type)";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("NumberInStock", pickle.NumberInStock);
                cmd.Parameters.AddWithValue("Price", pickle.Price);
                cmd.Parameters.AddWithValue("Size", pickle.Size);
                cmd.Parameters.AddWithValue("Type", pickle.Type);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var newPickle = MapReaderToPickle(reader);
                    return newPickle;
                }
                return null;
            }
        }

        public void Remove(string type)
        {
            throw new NotImplementedException();
        }

        public Pickle Update(Pickle pickle)
        {
            //var pickleToUpdate = GetByType(pickle.Type);

            //pickleToUpdate.NumberInStock += pickle.NumberInStock;

            //return pickleToUpdate;
            // updates are technically deletes and inserts
            var sql = @"update pickle
                        set NumberInStock = NumberInStock + @NewStock
                        output inserted.*
                        where Id = @Id";

            using (var connection = new SqlConnection(ConnectionString) )
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("NewStock", pickle.NumberInStock);
                cmd.Parameters.AddWithValue("Id", pickle.Id);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var updatedPickle = MapReaderToPickle(reader);

                    return updatedPickle;
                }
                return null;
            }
        }

        public Pickle GetByType(string typeOfPickle)
        {
            //return _pickles.FirstOrDefault(p => p.Type == type);

            //sql connection
            //using statement, using block
            //sql connection comes with a dispose method on it that we activate with a using statement
            // ANY TIME YOU CREATE A CONNECTION, PUT IT INSIDE A USING STATEMENT
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"SELECT *
                          FROM Pickle
                          WHERE Type = @Type";

                //sql command
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;
                // use the below line for added security
                // string interpolation is dangerous
                // (variable, value)
                cmd.Parameters.AddWithValue("Type", typeOfPickle);

                //execute the command
                var reader = cmd.ExecuteReader();

                // rough equivalent to first or default is using the if statement here
                if (reader.Read())
                {
                    var pickle = MapReaderToPickle(reader);
                    return pickle;
                }
                return null;
            }

        }

        public List<Pickle> GetAll()
        {
            // return _pickles;
        
            // SELECT * FROM pickle

            // SQL Connection - the actual connection to the database- creates a pipe from my application and SQL server
            var connection = new SqlConnection(ConnectionString);
            // open should ALWAYS be paired with a close statement as listed below
            // connections, by default, are closed so we must tell them to open
            // SQL server can only handles a certain number of open paths at a time
            connection.Open();

            // SQL command - us telling SQL server what to do
            //var cmd = new SqlCommand();
            //cmd.Connection = connection; this is the same as the command below

            // use this one because it is easiest; us telling sql server what to do
            var cmd = connection.CreateCommand();
            // this tells what to execute against our database
            cmd.CommandText = "SELECT * FROM pickle";

            // SQL Data Reader - get results; run this query and give me the results back
            // returns a SQL data reader
            // run the query against sql server and it returns rows one at a time
            // sql server giving us something back
            // most common way to interact with this reader is a while loop
            var reader = cmd.ExecuteReader();

            var pickles = new List<Pickle>();

            //Map results to C# Things
            // while the expression we are testing is true, keep going
            // .read() says get me the next row to sql server
            // .read() also returns a boolean; while there is a next row, do this thing I am telling you to do
            // reader looks and feels like a dictionary
            // the row tht it just read becomes the data that is available to me

            // map to C# things
            while (reader.Read())
            {
                // reader["Id"] retrieves the Id column from the data we are pulling back
                // the square brackets cause the variable to not the type of the thing; it returns an object; we have to tell it the type
                var id = (int)reader["Id"]; // this way is best to do it
                // you could do ths
                // var id = reader.getInt32() - but you have to pass in the column number here. STAY AWAY from this because, if someone changes the number of your column, it'll mess you up
                // casting is taking one value and saying it is a different kind of value
                // direct casting or explicit cast
                var type = (string)reader["Type"];

                var pickle = MapReaderToPickle(reader);
                pickles.Add(pickle);

            }
            // IMPORTANT - always close anything you've opened - if you don't close it, it will cause you lots and lots and lots of problems
            connection.Close();

            return pickles;

        }

        public Pickle GetById(int id)
        {
            //return _pickles.FirstOrDefault(pickle => pickle.Id == id);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var cmd = connection.CreateCommand();

                var query = @"
                             SELECT *
                             FROM Pickle
                             WHERE id = @id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("Id", id);

                var reader = cmd.ExecuteReader();

                if(reader.Read())
                {
                    return MapReaderToPickle(reader);
                }

                return null;
            }
        }

        Pickle MapReaderToPickle(SqlDataReader reader)
        {
            var pickle = new Pickle
            {
                Id = (int)reader["Id"],
                Type = (string)reader["Type"],
                Price = (decimal)reader["Price"],
                NumberInStock = (int)reader["NumberInStock"],
                Size = (string)reader["Size"]
            };

            return pickle;

        }
    }
}
