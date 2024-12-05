using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Migrations;
using OFF.DAL.Model;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using System.Numerics;

namespace OFF.PL.Controllers
{

    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CompanyDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;


        public HomeController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, CompanyDbContext context, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                try
                {
                    var contact = new Contact
                    {
                        Id = 0,
                        Message = contactVM.Message,
                        UserId = user.Id,
                        DateOfMessage = DateTime.UtcNow
                    };

                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "An error occurred while sending your message. Please try again later.");
                    Console.WriteLine($"An error occurred while saving the contact message: {ex.Message}");
                }
            }

            // If we are here, something went wrong, redisplay the form
            return View(contactVM);


        }
        [HttpGet]
        public async Task<IActionResult> AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            var contactVM = _mapper.Map<Contact, ContactVM>(contact);
            return View(contactVM);
        }

        public IActionResult Join()
        {
            return View();
        }
        public async Task<IActionResult> News()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                return NotFound("Admin role not found.");
            }

            var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name);
            var adminUserIds = adminUsers.Select(admin => admin.Id).ToList();

            var posts = await _unitOfWork.Posts.GetAllPostsAsync(p => adminUserIds.Contains(p.AppUserId));
            var orderedPosts = posts.OrderByDescending(post => post.CreatedAt);

            var postsview = orderedPosts.Select(post => new PostVM
            {
                Content = post.Content,
                ImageName = post.ImageName,
                VideoName = post.VideoName,
                CreatedAt = post.CreatedAt,
            }).ToList();

            return View(postsview);
        }
        public async Task<IActionResult> AllAgent()
        {

            var users = await _userManager.Users.ToListAsync();
            var fetchedusers = users.Where(u => _userManager.IsInRoleAsync(u, "Custodian").Result);
            var mappedUsers = new List<AgentRegisterVM>();

            foreach (var agentvm in fetchedusers)
            {
                var roles = await _userManager.GetRolesAsync(agentvm);

                var mappedUser = new AgentRegisterVM
                {
                    Id = agentvm.Id,
                    userName = agentvm.UserName,
                    ImageName = agentvm.ImageName,
                };

                mappedUsers.Add(mappedUser);
            }

            return View(mappedUsers);
        }
        //public async Task<IActionResult> AllPlayer()
        //{
        //    var players = await _userManager.GetUsersInRoleAsync("Player");

        //    var playerVMs = players.Select(p => new PlayerRegisterVM
        //    {
        //        Id = p.Id,
        //        userName = p.FName + " " + p.LName,
        //        ImageName = p.ImageName,
        //    }).ToList();

        //    return View(playerVMs); 
        //}



        public async Task<IActionResult> AllPlayer(string age, string position)
        {
            var players = await _userManager.GetUsersInRoleAsync("Player");

            // Ensure we're dealing with PLayer instances
            var playerList = players.OfType<PLayer>().ToList();

            // Filter players based on the age range
            if (!string.IsNullOrEmpty(age))
            {
                var currentDate = DateTime.Today;

                if (age == "Above 40")
                {
                    playerList = playerList.Where(p =>
                    {
                        var ageInYears = currentDate.Year - p.BirthDate.Year;
                        if (p.BirthDate > currentDate.AddYears(-ageInYears)) ageInYears--;
                        return ageInYears > 40;
                    }).ToList();
                }
                else
                {
                    var ageRange = age.Split('-');
                    if (ageRange.Length == 2 && int.TryParse(ageRange[0], out int minAge) && int.TryParse(ageRange[1], out int maxAge))
                    {
                        playerList = playerList.Where(p =>
                        {
                            var ageInYears = currentDate.Year - p.BirthDate.Year;
                            if (p.BirthDate > currentDate.AddYears(-ageInYears)) ageInYears--;
                            return ageInYears >= minAge && ageInYears <= maxAge;
                        }).ToList();
                    }
                }
            }

            // Filter players based on the position
            if (!string.IsNullOrEmpty(position))
            {
                playerList = playerList.Where(p => p.Position == position).ToList();
            }

            var playerVMs = playerList.Select(p => new PlayerRegisterVM
            {
                Id = p.Id,
                userName = p.FName + " " + p.LName,
                ImageName = p.ImageName,
            }).ToList();

            return View(playerVMs);
        }
    }

}

