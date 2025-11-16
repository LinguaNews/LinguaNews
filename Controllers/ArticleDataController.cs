using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LinguaNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleDataController : ControllerBase
    {
        // GET: api/<NewsDataController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<NewsDataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<NewsDataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<NewsDataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<NewsDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
