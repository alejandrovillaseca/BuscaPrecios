using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Controller;

namespace BuscaPrecios.API.Controllers.Falabella
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuscaController : ControllerBase
    {
        // GET: api/Busca
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Controller.FalabellaController.ProcesaPorSiteMap();
            return new string[] { "value1", "value2" };
        }

        // GET: api/Busca/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Busca
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Busca/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
