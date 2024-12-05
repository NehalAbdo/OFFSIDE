using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using System.Numerics;

namespace OFF.PL.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly CompanyDbContext _context;
        private readonly IPaymentService _paymentService;



        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper, SignInManager<AppUser> signInManager = null, IMailService mailService = null, CompanyDbContext context = null, IPaymentService paymentService = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _signInManager = signInManager;
            _mailService = mailService;
            _context = context;
            _paymentService = paymentService;
        }
        public async Task <IActionResult> Index()
        {
            var users= await _userManager.Users.ToListAsync();
            var fetchedusers = users.Where(u=>_userManager.IsInRoleAsync(u,"User").Result);
            var mappedUsers = new List<UserRoleVM>();
            foreach (var user in fetchedusers)
            {
                var roles = _userManager.GetRolesAsync(user).Result;

                var mappedUser = new UserRoleVM
                {
                    ID = user.Id,
                    Email = user.Email,
                    Roles = roles,
                    FName=user.FName,
                    LName=user.LName,
                };
                mappedUsers.Add(mappedUser);
            }
            return View(mappedUsers);
        }
        public async Task<IActionResult> Viewrequest(string? id) => await ReturnViewWithPlayer(id, nameof(Viewrequest));
        [HttpPost]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, "Player");
                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, "User"))
                    {
                        var removeResult = await _userManager.RemoveFromRoleAsync(user, "User");
                        if (!removeResult.Succeeded)
                        {
                            foreach (var error in removeResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return RedirectToAction(nameof(AllUser)); 
                        }
                    }
                    await _signInManager.RefreshSignInAsync(user);
                    var email = new Email
                    {
                        To = user.Email,
                        Subject = "Request Approved",
                        Body = "Your request to become a player has been approved."
                    };
                    _mailService.SendEmail(email);
                    return RedirectToAction(nameof(AllUser));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction(nameof(AllUser));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(); 
            }

            var email = new Email
            {
                To = user.Email,
                Subject = "Request Rejected",
                Body = "Your request to become a player has been rejected."
            };

            try
            {
                 _mailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to send email: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(); 
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Delete posts related to the user
                    var posts = await _unitOfWork.Posts.GetAllPostsAsync(p => p.AppUserId == user.Id);
                    if (posts != null && posts.Any())
                    {
                        _unitOfWork.Posts.RemoveRange(posts);
                    }

                    if (await _userManager.IsInRoleAsync(user, "Agent"))
                    {
                        var subscriptions = await _unitOfWork.Subscriptions.GetSubscriptionsByAgentIdAsync(user.Id);
                        if (subscriptions != null && subscriptions.Any())
                        {
                            _unitOfWork.Subscriptions.RemoveRange(subscriptions);
                        }
                    }
                    // Delete the user
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return RedirectToAction(nameof(Index));
                    }

                    await _unitOfWork.completeAsync();
                    await transaction.CommitAsync();

                    await _signInManager.SignOutAsync();
                    return RedirectToAction(nameof(AllUser));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred while deleting the user: " + ex.Message);
                    return RedirectToAction(nameof(Index));
                }
            }

        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Player"))
            {
                var playerDetails = _mapper.Map<PlayerRegisterVM>(user); 
                return View("Viewrequest", playerDetails);
            }
            else if (await _userManager.IsInRoleAsync(user, "Custodian"))
            {
                var custodianDetails = _mapper.Map<AgentRegisterVM>(user); 
                return View("CustodianDetails", custodianDetails);
            }
            else
            {
                return BadRequest(); 
            }
        }
        [HttpGet]
        public async Task<IActionResult> MailBox()
        {
            var contacts = await _context.Contacts
                 .Include(c => c.User)
                 .Select(c => new ContactVM
                 {
                     ID = c.Id,
                     Message = c.Message,
                     UserId = c.UserId,
                     //Name = c.User.UserName,
                     //Phone = c.User.PhoneNumber,
                     Email = c.User.Email,
                 })
                 .ToListAsync();

            return View(contacts);
        }
        public async Task<IActionResult> ContactDetails(int id)
        {
            var contact = await _context.Contacts
                 .Include(c => c.User)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactVM = new ContactVM
            {
                ID = contact.Id,
                Message = contact.Message,
                UserId = contact.UserId,
                //Name = contact.User.UserName,
                //Phone = contact.User.PhoneNumber,
                Email = contact.User.Email,
            };

            return View(contactVM);
        }
     
        public async Task<IActionResult> AgentView(string?id)=>await ReturnViewWithAgent(id,nameof(AgentView));

        public async Task<IActionResult> AddPost()
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
                Id = post.Id,
                Content = post.Content,
                ImageName = post.ImageName,
                VideoName = post.VideoName,
                CreatedAt = post.CreatedAt,
            }).ToList();

            return View(postsview);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await _unitOfWork.Posts.GetAsync(id);
            if (post != null)
            {
                _unitOfWork.Posts.Delete(post);
                await _unitOfWork.completeAsync();
                return RedirectToAction(nameof(AddPost));
            }
            return RedirectToAction(nameof(AddPost));
        }
        public async Task<IActionResult> UpdatePostnews(string postId)
        {
            var post = await _unitOfWork.Posts.GetAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<Post, PostVM>(post);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePostnews(PostVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = await _unitOfWork.Posts.GetAsync(model.Id);
            if (post == null)
            {
                return NotFound();
            }

            if (model.Content != post.Content || IsFileChanged(model.Image, post.ImageName) || IsFileChanged(model.Video, post.VideoName))
            {
                post.Content = model.Content;

                if (IsFileChanged(model.Image, post.ImageName))
                {
                    post.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");
                }

                if (IsFileChanged(model.Video, post.VideoName))
                {
                    post.VideoName = DocumentSetting.UploadFile(model.Video, "Video");
                }

                _unitOfWork.Posts.Update(post);
                await _unitOfWork.completeAsync();
            }

            return RedirectToAction("Profile", "Agent", new { id = post.AppUserId });
        }
        private bool IsFileChanged(IFormFile file, string existingFileName)
        {
            return file != null && file.Length > 0; // Check if a new file is uploaded
        }

        public IActionResult Post()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Post(PostVM model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var post = _mapper.Map<PostVM, Post>(model);
            post.CreatedAt = DateTime.UtcNow;
            post.AppUserId = user.Id;

            if (model.Image != null && model.Image.Length > 0)
                post.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");

            if (model.Video != null && model.Video.Length > 0)
                post.VideoName = DocumentSetting.UploadFile(model.Video, "Video");

            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.completeAsync();

            return RedirectToAction("News", "Home");

        }

        public async Task <IActionResult> Payment()
        {
            var payments = await _paymentService.GetAllPayments(); // Adjust method according to your service

            var viewModel = new AdminPaymentsVM
            {
                Payments = payments
            };

            return View(viewModel);
        }

        
        public async Task<IActionResult> AllUser(string? role, bool? isVip)
        {
           
                var users = await _userManager.Users.ToListAsync();

                var filteredUsers = users.AsQueryable();

                if (!string.IsNullOrEmpty(role))
                {
                    filteredUsers = filteredUsers.Where(u => _userManager.IsInRoleAsync(u, role).Result);
                }

                if (isVip.HasValue && role == "Custodian")
                {
                    filteredUsers = filteredUsers.Where(u => ((Agent)u).VIP == isVip.Value);
                }

                var mappedUsers = new List<UserRoleVM>();

                foreach (var user in filteredUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var mappedUser = new UserRoleVM
                    {
                        ID = user.Id,
                        FName = user.UserName,  // Assuming FName is the username
                        Email = user.Email,
                        Roles = roles.ToList(),
                        IsVIP = role == "Custodian" ? ((Agent)user).VIP : false  // Assuming UserRoleVM has IsVip property
                    };

                    mappedUsers.Add(mappedUser);
                }

                return View(mappedUsers);
            }

        
        private async Task<IActionResult> ReturnViewWithPlayer(string? id, string viewName)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();


            var player = await _unitOfWork.Players.GetAsync(id);
            if (player is null)
                return NotFound();
            var prof = new PlayerRegisterVM
            {
                FName = player.FName,
                Email = player.Email,
                Nationality = player.Nationality,
                ImageName = player.ImageName,
                Phone = player.PhoneNumber,
                BirthDate = player.BirthDate,
                userName = player.FName + " " + player.LName,
                Position = player.Position,
                NationalIDName=player.NationalIdName,
                Height = player.Height,
                Weight = player.Weight,
                Gender = player.Gender,
            };

            return View(viewName, prof);
        }
        private async Task<IActionResult> ReturnViewWithAgent(string? id, string viewName)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();


            var agent = await _unitOfWork.Agents.GetAsync(id);
            if (agent is null)
                return NotFound();
            var prof = new AgentRegisterVM
            {
                FName = agent.FName,
                Email = agent.Email,
                Nationality = agent.Nationality,
                ImageName = agent.ImageName,
                Phone = agent.PhoneNumber,
                BirthDate = agent.BirthDate,
                userName = agent.FName + "  " + agent.LName,
            };

            return View(viewName, prof);
        }

    }
}
