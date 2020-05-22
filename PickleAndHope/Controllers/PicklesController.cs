using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleAndHope.DataAccess;
using PickleAndHope.Models;

namespace PickleAndHope.Controllers
{
    // controller should be the thing telling others what to do
    // exposed at this particular endpoint
    // Nathan changes [controller] to something that pertains to this class more directlmodely
    // things in square brackets are end points
    // this is being exposed at this end point; [controller] will be subbed for pickles, the word attached to Controller below; this is the particular url endpoint
    [Route("api/[controller]")]
    // apiController is an attribute; tells the asp.netcore that this controller is for being exposed to the internet and it's going be an http api
    [ApiController]
    // this controller inherits from the controllerBase class; gives us http methods like ok and notfound for free
    public class PicklesController : ControllerBase
    {
        PickleRepository _repository;

        public PicklesController(PickleRepository repository)
        {
            _repository = repository;
        }

        // square brackets start with brackets; HttpPost adds something
        // if we want to identify a specific endpoint, we can write [HttpPost("add")] which means api/pickles/add;
        // no parenthesis is the same as parenthesis with an empty string; this currently says don't add anything to the base route
        // a pickle is an entity idea; if it's in your ERD, the models folder is probably where it's going to end up living
        [HttpPost]
        // descriptive name for method follows IActionResult
        // return type is IActionResult for flexibility ***What does that allow us, again?***
        public IActionResult AddPickle(Pickle pickleToAdd)
        {
            // the below if statement says if there are none in stock, make the current thing the stock;
            // otherwise, find the thing that exists and add the stock to this one
            // every class should have one thing that it does; single responsibility principle
            // controllers single responsibility is to take things in from the internet and return things to the internet; communication with http stuff
            // currently, this class both stores info AND communicates http stuff; something with two jobs means you need to break the thing up;
            // common interview question: one of the more common ways that people deal with storing data is with a pattern called the repository pattern;
            // some people love it, some hate it; it does perform the job; specifically for managing data and data storage;
            // INTERVIEW QUESTIONS
            // what's the difference between an abstract class and an interface?
            // what's the difference between a value type and a reference type?
            // can you describe the repository pattern OR can you describe a pattern that you have used in code?
            // Repository pattern is one of the more common in software development
            // patterns are more for the person reading the code than for being a software developer
            // the DataAccessLayer(DAL, DataAccess, Data) are the same as our data folder in React; this is a division of duty significance
            // if we reuse the below if/else, we can move that code to aits own method called AddOrUpdate
            var existingPickle = _repository.GetByType(pickleToAdd.Type);
            if (existingPickle == null)
            {
                var newPickle = _repository.Add(pickleToAdd);
                return Created("", newPickle);
            }
            else
            {
                var updatedPickle = _repository.Update(pickleToAdd);
                return Ok(pickleToAdd);
            }
            // created says "what is the rest url to get it; nothing yet, so here's an empty string;
            // when we add things, we want a 201 http response
        }

        // api/pickles/all
        [HttpGet]
        public IActionResult GetAllPickles()
        {
            var allPickles = _repository.GetAll();

            return Ok(allPickles);
        }

        // curly braces means whatever comes in here, put this into the parameter
        // api/pickles/{id}
        // api/pickles/5
        [HttpGet("{id}")]
        public IActionResult GetPickleById(int id)
        {
            var pickle = _repository.GetById(id);

            if (pickle == null) return NotFound("No pickle with that id could be found.");

            return Ok(pickle);
        }

        // api/pickles/type/dill
        [HttpGet("type/{type}/")]
        public IActionResult GetPickleByType(string type)
        {
            var pickle = _repository.GetByType(type);

            if (pickle == null) return NotFound("No pickle with that type exists");

            return Ok(pickle);
        }
    }
}