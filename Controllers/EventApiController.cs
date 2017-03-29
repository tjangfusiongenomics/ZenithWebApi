using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZenithWebSite.Models;

namespace ZenithWebSite.Controllers
{

    [Produces("application/json")]
    [Route("api/Eventapi")]
    public class EventApiController : Controller

    {
        private ZenithContext _context { get; set; }
        public EventApiController(ZenithContext context)
        {
            _context = context;
        }

        // GET: api/Eventapi
        [HttpGet]
        public IEnumerable<Event> Get()
        {
            return _context.Events.ToList();
        }
        // GET api/Eventapi/5
        [HttpGet("{id}")]
        public Event Get(int id)
        {
            return _context.Events.FirstOrDefault(s => s.EventId == id);
        }
        // POST api/Eventapi
        [HttpPost]
        public void Post([FromBody]Event e)
        {
            _context.Events.Add(e);
            _context.SaveChanges();
        }
        // PUT api/Eventapi/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Event e)
        {
            _context.Events.Update(e);
            _context.SaveChanges();
        }
        // DELETE api/Eventapi/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var e = Events.FirstOrDefault(t => t.EventId == id);
            if (e != null)
            {
                _context.Events.Remove(e);
                _context.SaveChanges();
            }
        }
    }
}