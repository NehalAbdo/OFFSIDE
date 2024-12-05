using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OFF.DAL.Context;
using OFF.DAL.Model;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using Stripe;
using System.Numerics;
using Subscription = OFF.DAL.Model.Subscription;

namespace OFF.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly CompanyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMailService _MailService;



        public AccountController(UserManager<AppUser> userManager, CompanyDbContext context, IMapper mapper, SignInManager<AppUser> signInManager, IMailService mailService)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _signInManager = signInManager;
            _MailService = mailService;
        }
        public IActionResult AgentRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgentRegister(AgentRegisterVM model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }

            var existingUserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (existingUserByPhoneNumber != null)
            {
                ModelState.AddModelError(string.Empty, "Phone number is already exist.");
                return View(model);
            }

          
            model.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");
            model.NationalIDName = DocumentSetting.UploadFile(model.National, "Pics");

            var agent = new Agent
            {
                FName = model.FName,
                LName = model.LName,
                Email = model.Email,
                Nationality= model.Nationality,
                NationalIdName=model.NationalIDName,
                ImageName= model.ImageName,
                PhoneNumber = model.Phone,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                UserName = model.FName + model.LName,
                VIP=true
            };

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(agent, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(agent, "Custodian");

                        var freeTrialSubscription = new Subscription
                        {
                            AgentId = agent.Id,
                            SubscriptionType = SubscriptionType.FreeTrial,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMonths(1),
                            SubscriptionStatus = subscriptionStatus.Active,
                            Amount = 0,
                            //SubStatus = SubStatus.NotPaid
                        };

                        _context.Subscriptions.Add(freeTrialSubscription);
                        await _context.SaveChangesAsync();

                        transaction.Commit();

                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        transaction.Rollback();
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public IActionResult PlayerRegister()
        
        
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PlayerRegister(PlayerRegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var existingUserByPhoneNumber = await _userManager.FindByNameAsync(model.Phone);
                if (existingUserByPhoneNumber != null)
                {
                    ModelState.AddModelError(string.Empty, "Phone number is already exist.");
                    return View(model);
                }
            }
            var userName = model.FName + model.LName;
            var userWithSameUserName = await _userManager.FindByNameAsync(userName);
            if (userWithSameUserName != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken. Please choose a different username.");
                return View(model);
            }

            model.ImageName = DocumentSetting.UploadFile(model.Image, "Pics");
                model.NationalIDName = DocumentSetting.UploadFile(model.National, "Pics");

            var player = new PLayer
            {
                UserName = model.FName + model.LName,
                FName = model.FName,
                LName =model.LName,
                Email =model.Email,
                PhoneNumber=model.Phone,
                BirthDate =model.BirthDate,
                Gender =model.Gender,
                Position=model.Position,
                Nationality =model.Nationality,
                ImageName = model.ImageName,
                NationalIdName=model.NationalIDName,
            };

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(player, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(player, "User");
                       
                        transaction.Commit();

                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        transaction.Rollback();
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public IActionResult Login()
        {
            return View(new LoginVM());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null)
            {
                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    return RedirectToAction("Index","Home", new { id = user.Id });
                }
            }
            ModelState.AddModelError("", "Incorrect Email Or Password");
            return View();
        }

        public IActionResult SignOut()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ForgetPassword()
        {
            return View(new ForgetPasswordVM());
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordMailKit(ForgetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var resetPasswordLink = Url.Action("ResetPassword", "Account",
                    new { Email = model.Email, Token = token }, Request.Scheme);
                    var email = new Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body = resetPasswordLink
                    };
                    _MailService.SendEmail(email);



                    return RedirectToAction(nameof(CompleteForgetPassword));

                }

                ModelState.AddModelError("", "Invalid Email");
            }

            return View("ForgetPassword", model);
        }
        public IActionResult CompleteForgetPassword()
        {
            return View();
        }
        public IActionResult ResetPassword(string Email, string Token)
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Login));
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                }

            }
            return View(model);

        }
    }
}


