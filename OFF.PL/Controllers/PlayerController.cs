using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;
using OFF.PL.MappingProfile;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using System.Security.Claims;

namespace OFF.PL.Controllers
{
    public class PlayerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CompanyDbContext _context;


        public PlayerController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper, CompanyDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> Profile(string id)
            {
                var player = await _unitOfWork.Players.GetAsync(id);
                if (player == null)
                {
                    return NotFound();
                }

            var posts = await _unitOfWork.Posts.GetPostsByUserIdAsync(id);
            var orderedPosts = posts.OrderByDescending(post => post.CreatedAt);


            var playerProfileVM = new PlayerRegisterVM
                {
                    Id = player.Id,
                    FName = player.FName,
                    LName = player.LName,
                    Email = player.Email,
                    Experience= player.Experience,
                    Phone = player.PhoneNumber,
                    Nationality = player.Nationality,
                    BirthDate = player.BirthDate,
                    Gender = player.Gender,
                    Weight = player.Weight,
                    Height = player.Height,
                    Position = player.Position,
                    ImageName = player.ImageName,
                    userName=player.FName+" "+player.LName,
                Posts = orderedPosts.Select(p => new PostVM
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageName = p.ImageName,
                    VideoName = p.VideoName,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };
            
                return View("Profile",playerProfileVM);
            }
  
        public IActionResult AddPost()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(PostVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var postId = Guid.NewGuid().ToString();
            model.Id = postId;
            model.CreatedAt = DateTime.UtcNow;
            model.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (model.Image != null && model.Image.Length > 0)
                model.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");

            if (model.Video != null && model.Video.Length > 0)
                model.VideoName = DocumentSetting.UploadFile(model.Video, "Video");

            var post = _mapper.Map<PostVM, Post>(model);
            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.completeAsync();

            return RedirectToAction("Profile", "Player", new { id = model.AppUserId });
        }

        public async Task<IActionResult> EditPost(string postId)
        {
            var post = await _unitOfWork.Posts.GetAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<Post, PostVM>(post);

            return View(model);
        }
        private bool IsFileChanged(IFormFile file, string existingFileName)
        {
            return file != null && file.Length > 0; 
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePost(PostVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = await _unitOfWork.Posts.GetAsync(model.Id);
            if (post == null)
            {
                return NotFound(); // Optionally handle not found case
            }

            if (model.Content != post.Content || IsFileChanged(model.Image, post.ImageName) || IsFileChanged(model.Video, post.VideoName))
            {
                post.Content = model.Content;

                // Handle image update if necessary
                if (IsFileChanged(model.Image, post.ImageName))
                {
                    post.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");
                }

                // Handle video update if necessary
                if (IsFileChanged(model.Video, post.VideoName))
                {
                    post.VideoName = DocumentSetting.UploadFile(model.Video, "Video");
                }

                // Update the post entity in the database
                _unitOfWork.Posts.Update(post);
                await _unitOfWork.completeAsync();
            }

            return RedirectToAction("Profile", "Player", new { id = post.AppUserId });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var post = await _unitOfWork.Posts.GetAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            _unitOfWork.Posts.Delete(post);
            await _unitOfWork.completeAsync();

            return RedirectToAction("Profile", "Player", new { id = post.AppUserId });
        }

        [HttpGet]
        public IActionResult AddExperience () 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddExperience(PlayerExperienceVM model)
        {
            if (string.IsNullOrEmpty(model.Experience) &&
                model.Height == null &&
                model.Weight == null) 
            { 
                ModelState.AddModelError("", "Please fill out at least one of the fields (Experience, Height, or Weight).");
            }

            if (!ModelState.IsValid)
                return View(model);
            model.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var player = await _unitOfWork.Players.GetAsync(model.Id);

            if (model.Card != null && model.Card.Length > 0)
                model.FootballCard = DocumentSetting.UploadFile(model.Card, "Pics");

            player.Experience = model.Experience;
            player.Weight = model.Weight.HasValue ? model.Weight.Value : 0; 
            player.Height = model.Height.HasValue ? model.Height.Value : 0;

            //await _unitOfWork.Players.AddAsync(player);
            await _unitOfWork.completeAsync();
            return RedirectToAction("Profile", "Player", new { id = model.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var player = await _userManager.FindByIdAsync(id) as PLayer;
            if (player == null)
            {
                return NotFound();
            }

            var playerVm = new EditingVM
            {
                Id = player.Id,
                userName = player.FName + " " + player.LName,
                Email = player.Email,
                Phone = player.PhoneNumber,
                Position = player.Position, // Accessing 'Position' from 'PLayer'
                Nationality = player.Nationality,
                ImageName = player.ImageName,
            };

            return View(playerVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditingVM playerVm)
        {
            if (!ModelState.IsValid)
            {
                return View(playerVm);
            }

            try
            {
                var player = await _unitOfWork.Players.GetAsync(playerVm.Id); // Assuming Players represents PLayer objects
                if (player == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(playerVm.userName) && playerVm.userName.Contains(" "))
                {
                    var parts = playerVm.userName.Split(' ');
                    player.FName = parts[0];
                    player.LName = parts[1];
                }

                // Update other properties
                player.Email = playerVm.Email;
                player.PhoneNumber = playerVm.Phone;
                player.Position = playerVm.Position; 
                        player.UserName = playerVm.FName + playerVm.LName; 


                if (playerVm.Image != null)
                {
                    player.ImageName = DocumentSetting.UploadFile(playerVm.Image, "Pics");
                }

                _unitOfWork.Players.Update(player);
                await _unitOfWork.completeAsync();

                return RedirectToAction("Profile", "Player", new { id = playerVm.Id });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error updating player: " + ex.Message);
                return View(playerVm);
            }
        }

        public async Task<IActionResult> ViewProf(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var player = await _unitOfWork.Players
                .Include(p => p.Posts)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (player is null)
                return NotFound();

            var prof = new PlayerRegisterVM
            {
                Id = player.Id,
                FName = player.FName,
                LName = player.LName,
                Email = player.Email,
                Experience = player.Experience,
                Phone = player.PhoneNumber,
                Nationality = player.Nationality,
                BirthDate = player.BirthDate,
                Gender = player.Gender,
                Weight = player.Weight,
                Height = player.Height,
                Position = player.Position,
                ImageName = player.ImageName,
                userName = player.FName + " " + player.LName,
                Posts = player.Posts.Select(post => new PostVM
                {
                    Content = post.Content,
                    ImageName = post.ImageName,
                    VideoName = post.VideoName,
                    CreatedAt = post.CreatedAt
                }).ToList()
            };

            return View(prof);
        }

    



    }
}
