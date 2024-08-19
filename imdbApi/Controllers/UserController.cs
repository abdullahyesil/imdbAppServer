using imdbApi.DTO;
using imdbApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<appUser> _userManager;
        private readonly SignInManager<appUser> _signInManager; //Login işlemi için nesneyi tanımladık
        private readonly IConfiguration _config; // appsetting.cs üzerinden bilgi çekmek için IConfiguration tanımladık.


        public UserController(UserManager<appUser> userManager, SignInManager<appUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;

        }



        [HttpGet]
        public async Task<IActionResult> getUsers()
        {
            var users = await _userManager.Users.Select(u => new UserDTO
            {
                UserName = u.UserName,
                Email = u.Email
            }).ToListAsync();

            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }


        [HttpPost("register")]
        public async Task<IActionResult> CrateUser(UserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new appUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return StatusCode(201);
            }

            return BadRequest(result.Errors);

        }


        [HttpPut("user/{id}")]
        public async Task<IActionResult> Edit(string id)
        {

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id); // id bilgisini göster id ile eşleşen veriyi al

            if (user != null)
            {
                return Ok(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                });

            }
            return BadRequest();
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Email veya şifre hatalı" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false); //Kullanıcıadı ile şifre  ile sorguladık

            if (result.Succeeded)
            {
                var response = new
                {
                    token = GenerateJWT(user),
                    message = "Başarıyla giriş yapıldı",
                    Id = user.Id,
                    tokenExpires= 86400 
                };
                return Ok(response); //GenerateJWT() //token oluşturma metodu
            }
            return Unauthorized(new {message= "Email veya şifre yanlış"});
        }

        private object GenerateJWT(appUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Secret").Value ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Name, user.Email ?? ""),
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "abdullahyesil.com" //Bu api kim tarafından sağlanıyor?.
            };


            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token) ;
        }



    }
}
