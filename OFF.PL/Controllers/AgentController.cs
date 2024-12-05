using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.BLL.Repository;
using OFF.DAL.Model;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using Stripe;
using System;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace OFF.PL.Controllers
{
    public class AgentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly UserManager<AppUser> _userManager;


        public AgentController(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService, ISubscriptionService subscriptionService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
            _userManager = userManager;
        }

        public IActionResult Subscribe()
        {
            return View(new SubscriptionVM());
        }

        public async Task UpdateAgentVipStatus(string agentId)
        {
            var agent = await _unitOfWork.Agents.GetAgentWithSubscriptionsAsync(agentId);

            if (agent != null)
            {
                var activeSubscription = agent.Subscriptions.FirstOrDefault(s => s.SubscriptionStatus == subscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);
                agent.VIP = activeSubscription != null;
                _unitOfWork.Agents.Update(agent);
                await _unitOfWork.completeAsync();
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var agent = await _unitOfWork.Agents.GetAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            var agentVm = new EditingVM
            {
                Id = agent.Id,
                userName = agent.FName + " " + agent.LName,
                Email = agent.Email,
                Phone = agent.PhoneNumber,
                Nationality = agent.Nationality,
                ImageName = agent.ImageName,
            };

            return View(agentVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditingVM AgentVm)
        {
            if (!ModelState.IsValid)
            {
                return View(AgentVm);
            }

            try
            {
                AgentVm.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var agent = await _unitOfWork.Agents.GetAsync(AgentVm.Id);
                if (agent == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(AgentVm.userName) && AgentVm.userName.Contains(" "))
                {
                    var parts = AgentVm.userName.Split(' ');
                    agent.FName = parts[0];
                    agent.LName = parts[1];
                }

                agent.Email = AgentVm.Email;
                agent.PhoneNumber = AgentVm.Phone;
                agent.Nationality = AgentVm.Nationality;

                if (AgentVm.Image != null)
                {
                    agent.ImageName = DocumentSetting.UploadFile(AgentVm.Image, "Pics");
                }

                _unitOfWork.Agents.Update(agent);
                await _unitOfWork.completeAsync();

                return RedirectToAction("Profile", "Agent", new { id = AgentVm.Id });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error updating : " + ex.Message);
                return View(AgentVm);
            }
        }
        public async Task<IActionResult> Profile(string id)
        {
            var agent = await _unitOfWork.Agents.GetAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            var posts = await _unitOfWork.Posts.GetPostsByUserIdAsync(id);
            var orderedPosts = posts.OrderByDescending(post => post.CreatedAt);


            var agentProfileVM = new AgentRegisterVM
            {
                Id = agent.Id,
                FName = agent.FName,
                LName = agent.LName,
                Email = agent.Email,
                Phone = agent.PhoneNumber,
                Nationality = agent.Nationality,
                BirthDate = agent.BirthDate,
                Gender = agent.Gender,
                ImageName = agent.ImageName,
                userName = agent.FName + " " + agent.LName,
                Posts = posts.Select(p => new PostVM
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageName = p.ImageName,
                    VideoName = p.VideoName,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };

            return View("Profile", agentProfileVM);
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

            return RedirectToAction("Profile", "Agent", new { id = model.AppUserId });
        }

      //[HttpGet]
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

            return RedirectToAction("Profile", "Agent", new { id = post.AppUserId });
        }
        private bool IsFileChanged(IFormFile file, string existingFileName)
        {
            return file != null && file.Length > 0; 
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
                userName = agent.FName +"  "+ agent.LName,
            };

            return View(viewName,prof);
        }

        public async Task<IActionResult> ViewProf(string?id)=>await ReturnViewWithAgent(id,nameof(ViewProf));


    }
}

