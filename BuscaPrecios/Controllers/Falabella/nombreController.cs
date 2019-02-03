using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Models;

namespace BuscaPrecios.API.Controllers.Falabella
{
    [Route("api/[controller]")]
    [ApiController]
    public class nombreController : ControllerBase
    {
        private readonly DataContext _context;

        public nombreController(DataContext context)
        {
            _context = context;
        }

        // GET: api/nombre
        [HttpGet]
        public IEnumerable<Sistemas> GetSistemas()
        {
            return _context.Sistemas;
        }

        // GET: api/nombre/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSistemas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sistemas = await _context.Sistemas.FindAsync(id);

            if (sistemas == null)
            {
                return NotFound();
            }

            return Ok(sistemas);
        }

        // PUT: api/nombre/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSistemas([FromRoute] int id, [FromBody] Sistemas sistemas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sistemas.id)
            {
                return BadRequest();
            }

            _context.Entry(sistemas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SistemasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/nombre
        [HttpPost]
        public async Task<IActionResult> PostSistemas([FromBody] Sistemas sistemas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Sistemas.Add(sistemas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSistemas", new { id = sistemas.id }, sistemas);
        }

        // DELETE: api/nombre/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSistemas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sistemas = await _context.Sistemas.FindAsync(id);
            if (sistemas == null)
            {
                return NotFound();
            }

            _context.Sistemas.Remove(sistemas);
            await _context.SaveChangesAsync();

            return Ok(sistemas);
        }

        private bool SistemasExists(int id)
        {
            return _context.Sistemas.Any(e => e.id == id);
        }
    }
}