using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PickleAndHope.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace PickleAndHope.DataAccess
{
    // repositories have a collection like interface for messing with data;
    // what I mean is, some of the methods from List like add, remove, LINQ stuff is part of the collection-y sort of interface
    // this looks exactly like what we were doing in front end

    // a lot of code is figuring how to make things concise
    // we can do get stuff inside the browser window. in order to do post stuff, we have to use another tool (postman, insomnia, etc.)
    public class PickleRepository
    {

        // Connection String - instructions on how to connect
        string ConnectionString;

        public PickleRepository(IConfiguration config)
        {
            ConnectionString = config.GetConnectionString("PickleAndHope");
        }

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

            using (var db = new SqlConnection(ConnectionString))
            {
                // these two lines are the same as the commented out lines below
                var result = db.QueryFirstOrDefault<Pickle>(sql, pickle);
                return result;
            }
        }

        public void Remove(string type)
        {
            throw new NotImplementedException();
        }

        public Pickle Update(Pickle pickle)
        {
            //return pickleToUpdate;
            // updates are technically deletes and inserts
            var sql = @"update pickle
                        set NumberInStock = NumberInStock + @NewStock
                        output inserted.*
                        where Id = @Id";

            using (var db = new SqlConnection(ConnectionString))
            {
                var parameters = new
                {
                    NewStock = pickle.NumberInStock,
                    Id = pickle.Id
                };

                return db.QueryFirstOrDefault<Pickle>(sql, parameters);
            }
        }

        public Pickle GetByType(string typeOfPickle)
        {
            var query = @"SELECT *
                          FROM Pickle
                          WHERE Type = @Type";
            //sql connection
            //using statement, using block
            //sql connection comes with a dispose method on it that we activate with a using statement
            // ANY TIME YOU CREATE A CONNECTION, PUT IT INSIDE A USING STATEMENT
            using (var db = new SqlConnection(ConnectionString))
            {
                var parameters = new { Type = typeOfPickle };
                var pickle = db.QueryFirstOrDefault<Pickle>(query, parameters);
                return pickle;
            }
        }

        public List<Pickle> GetAll()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                // returning this straight out brings back an IEnumerable. you can convert an IeNumerable to a List if need be
                //you can also just return an IEnumerable instead of List<Pickle>
                return db.Query<Pickle>("SELECT * FROM pickle").ToList();
            }
              
        }

        public Pickle GetById(int id)
        {
            var query = @"SELECT *
                          FROM Pickle
                          WHERE id = @id
                                numberinstock = @numberInStock";
            using (var db = new SqlConnection(ConnectionString))
            {
                //at the end of the next line is an Anonymous Type
                var parameters = new { Id = id };
                var pickle = db.QueryFirstOrDefault<Pickle>(query, parameters);
                return pickle;
            }
        
        }
    }
}
