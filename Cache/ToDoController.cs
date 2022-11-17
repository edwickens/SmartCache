using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cache.Data;
using StackExchange.Redis;

namespace Cache
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly CacheContext _context;
        private readonly IConnectionMultiplexer _redis;

        public ToDoController(CacheContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }

        // GET: api/ToDo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetToDo()
        {
            if (_context.ToDo == null)
            {
                return NotFound();
            }

            return await _context.ToDo.ToListAsync();
        }

        // GET: api/ToDo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetToDo(int id)
        {
            if (_context.ToDo == null)
            {
                return NotFound();
            }

            var x = new RedEfCachedQuery<ToDo?>(async () =>
                {
                    var t = await _context.ToDo.FindAsync(id);
                    t.IsDone = false;
                    return t;
                },
            _redis.GetDatabase(), id.ToString());

            var toDo = await x.ExecuteAsync();
            if (toDo == null)
            {
                return NotFound();
            }

            return toDo;
        }

        // PUT: api/ToDo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo(int id, ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return BadRequest();
            }

            _context.Entry(toDo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(id))
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

        // POST: api/ToDo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToDo>> PostToDo(ToDo toDo)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(toDo.Id.ToString(), JsonSerializer.Serialize(toDo));
            
            if (_context.ToDo == null)
            {
                return Problem("Entity set 'CacheContext.ToDo'  is null.");
            }

            var x = new RedEfCachedCommand<ToDo>(async () =>
            {
                _context.ToDo.Add(toDo);
                await _context.SaveChangesAsync();
            }, toDo, _redis.GetDatabase(), () => Task.FromResult(toDo.Id.ToString()));

            await x.ExecuteAsync();

            return CreatedAtAction("GetToDo", new { id = toDo.Id }, toDo);
        }

        // DELETE: api/ToDo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            if (_context.ToDo == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDo.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            _context.ToDo.Remove(toDo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoExists(int id)
        {
            return (_context.ToDo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}