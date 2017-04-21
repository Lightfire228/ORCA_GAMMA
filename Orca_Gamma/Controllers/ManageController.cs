using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Orca_Gamma.Models;
using System.Web.Security;
using Orca_Gamma.Models.DatabaseModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;

namespace Orca_Gamma.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _dbContext = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                IsExpert = UserManager.IsInRole(userId, "Expert")
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        //this is going to be to display expert info all on one page
        //should only need a get method for this information -Geoff
        //GET: /Manage/expertInfo
        public ActionResult expertInfo()
        {
            String id = User.Identity.GetUserId();

            if (_dbContext.Experts.Find(id) != null)
            {

                Expert expert = _dbContext.Experts.Find(id);

                IEnumerable<Keyword> keywords = from keyRelation in _dbContext.KeywordRelations.ToList()
                                                from key in _dbContext.Keywords.ToList()
                                                where key.Id == keyRelation.KeywordId && keyRelation.ExpertId == id
                                                select key;

                var cat = expert.Catagory;

                var model = new ExpertInfoViewModel
                {
                    Keywords = keywords,
                    Catagory = cat
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //Creating a new keyword on the expert account -Geoff
        //GET: /Manage/createKeyword
        public ActionResult createKeyword()
        {
            return View();
        }

        //POST: /Manage/createKeyword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult createKeyword(Keyword model)
        {
            Keyword keyword = new Keyword
            {
                Name = model.Name
            };
            _dbContext.Keywords.Add(keyword);

            String id = getCurrentUser().Id;
            Expert expert = _dbContext.Experts.Find(id);
            KeywordRelation relation = new KeywordRelation
            {
                Keyword = keyword,
                Expert = expert
            };
            _dbContext.KeywordRelations.Add(relation);

            _dbContext.SaveChanges();

            if (ModelState.IsValid)
            {
                return RedirectToAction("expertInfo");
            }
            return View(model);

        }

        //this will be where they check to make sure they want to delte - Geoff
        //GET: //Manage/keywordDelete
        public ActionResult keywordDelete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Keyword keyword = _dbContext.Keywords.Find(id);
            if(keyword == null)
            {
                return HttpNotFound();
            }
            return View(keyword);
        }

        //soft delete of keyword. removes relation between expert and keyword -Geoff
        //POST: //Manage/keywordDelete
        [HttpPost, ActionName("keywordDelete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult keywordDeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();

            //need to find the one to remove
            List<KeywordRelation> list = (from words in _dbContext.KeywordRelations.ToList()
                                          where words.ExpertId == userId && words.KeywordId == id
                                          select words).ToList();

            _dbContext.KeywordRelations.Remove(list[0]);
            _dbContext.SaveChanges();
            return RedirectToAction("expertInfo");
        }

        //this is just for regular user acount informaiton change -Geoff
        //GET: //Manage/editUserAccount
        public ActionResult editUserAccount()
        {
            ApplicationUser user = getCurrentUser();
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;
            ViewBag.Bio = user.Bio;
            ViewBag.Phone = user.PhoneNumber;

            return View();
        }

		//this is the post method for regular user account info change -Geoff
        //POST: /Manage/editUserAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editUserAccount(EditAccountViewModel model)
        {
			String id = model.User.Id;
            ApplicationUser user = getCurrentUser();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            //Don't want them to edit login UserName -DBS
            //user.UserName    = user.UserName;
            user.Bio = model.Bio;
            //Moving regex and error message over to EditUserViewModel -RD
            //Match match = Regex.Match(model.PhoneNumber ?? "", @"^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$");
            //if (match.Success)
            //{
            user.PhoneNumber = model.PhoneNumber;

                _dbContext.SaveChanges();

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

            //}
            //else
            //{
               // ViewBag.error = "Invalid phone number! Must be (555) 555-5555 or 555-555-5555 or 555-5555";
            //}
            return View(model);
}

        //needs to check if they already have expert information or not
        //GET: /Manage/editExpertAccount
        public ActionResult editExpertAccount()
        {
            //check goes here
            //if there is not one create one now and leave it null for the next step
            // get current user
            String id = getCurrentUser().Id;
            Expert expert = _dbContext.Experts.Find(id);
            
            if (expert == null)
            {
                //MAY WANT TO CLEAN UP CREATING BOGUS CATEGORY DATA -Geoff
                expert = new Expert();
                expert.User = getCurrentUser();  // sets the relation between expert and user
                expert.Catagory = _dbContext.Catagories.First();
                _dbContext.Experts.Add(expert);
                _dbContext.SaveChanges();
            }

            return View(new EditExpertViewModel {
                Expert = expert,
                Categories = _dbContext.Catagories.ToList()

            });
        }

        //POST: /Manager/editExpertAcount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editExpertAccount(EditExpertViewModel model)
        {
            String id = getCurrentUser().Id;
            Expert expert = _dbContext.Experts.Find(id);
            expert.Catagory = _dbContext.Catagories.Find(model.SelectedCatagory);
            _dbContext.SaveChanges();

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ApplicationUser getCurrentUser() {
			return _dbContext.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
		}

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }

}