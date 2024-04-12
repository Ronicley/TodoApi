using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly TodoContext _context;

        public UserController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.User.Select(x => UserToDTO(x)).ToListAsync();
        }
        
        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDTO(user);
        }
        
        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserDTO userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        
        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDto)
        {
            string userDocument = string.Empty;
            string creditCardToken = string.Empty;

            using (var myHash = SHA256.Create())
            {
                var byteArrayResultOfUserDocument = Encoding.UTF8.GetBytes(userDto.UserDocument);
                var byteArrayResultOfCreditCardToken = Encoding.UTF8.GetBytes(userDto.CreditCardToken);

                var byteArrayResult1 = myHash.ComputeHash(byteArrayResultOfUserDocument);
                var byteArrayResult2 = myHash.ComputeHash(byteArrayResultOfCreditCardToken);

                userDocument = string.Concat(Array.ConvertAll(byteArrayResult1, h => h.ToString("x8")));
                creditCardToken = string.Concat(Array.ConvertAll(byteArrayResult2, h => h.ToString("x8")));
            }
            
            var user = new User
            {
                UserDocument = userDocument,
                CreditCardToken = creditCardToken,
                Value = userDto.Value
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                UserToDTO(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }
        
        private static UserDTO UserToDTO(User userDto) => new UserDTO
        {
            Id = userDto.Id,
            UserDocument = userDto.UserDocument,
            CreditCardToken = userDto.CreditCardToken,
            Value = userDto.Value
        };
    }    
};

