using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickleAndHope.Models;

namespace PickleAndHope.DataAccess
{


    // repositories have a collection like interface for messing with data;
    // what I mean is, some of the methods from List like add, remove, LINQ stuff is part of the collectiony sort of interface
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

        public void Add(Pickle pickle)
        {
            // using Max takes into account the possibility of repeats
            pickle.Id = _pickles.Max(x => x.Id) + 1;
            _pickles.Add(pickle);
        }

        public void Remove(string type)
        {
            throw new NotImplementedException();
        }

        public Pickle Update(Pickle pickle)
        {
            var pickleToUpdate = GetByType(pickle.Type);

            pickleToUpdate.NumberInStock += pickle.NumberInStock;

            return pickleToUpdate;
        }

        public Pickle GetByType(string type)
        {
            return _pickles.FirstOrDefault(p => p.Type == type);
        }

        public List<Pickle> GetAll()
        {
            return _pickles;
        }

        public Pickle GetById(int id)
        {
            return _pickles.FirstOrDefault(pickle => pickle.Id == id);
        }
    }
}
