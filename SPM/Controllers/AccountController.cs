using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SPM.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using SPM.Helpers;

namespace SPM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            using (var db = new Entities.Entities())
            {
                try { 
                    Entities.SPM_Users user = db.SPM_Users.Where(m => m.email == model.Email.ToLower()).Where(m => m.pass == model.Password).First();
                    Session["userId"] = user.id;

                    if(user.email.ToLower() == "gil.silberstein@myblueprint.ca" || user.email.ToLower() == "mytest@spm.com")
                    {
                        Session["isAdmin"] = "1";
                        return RedirectToAction("AdminDashBoard", "Account");
                    }

                    return RedirectToAction("Home", "Account");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }               
            }           
        }


        [AllowAnonymous]
        public ActionResult AdminDashboard()
        {
            using(var db = new Entities.Entities())
            {
                List<Entities.School> schoolList = new List<Entities.School>();

                try
                {
                    var schools = db.Schools.ToList();
                    foreach(var school in schools)
                    {
                        if(school.school_name != "admin")
                        {
                            schoolList.Add(school);
                        }                       
                    }

                    ViewData["schoolList"] = schoolList;
                    return View();
                }
                catch (Exception)
                {
                    ViewData["schoolList"] = null;
                    return View();
                }                       
            }          
        }

        //
        // GET: /Account/Home
        [AllowAnonymous]
        public ActionResult Home()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Entities.SPM_Users user;
            Entities.School school;
            List<Entities.Project> projectList = new List<Entities.Project>();            
            List<Entities.Test> testsList = new List<Entities.Test>();
                        
            using (var db = new Entities.Entities())
            {
                int userId = (int)Session["userId"];
                user = db.SPM_Users.Where(m => m.id == userId).First();
                school = db.Schools.Where(m => m.id == user.fk_schools).First();

                var projectAssignments = db.Project_Assignments.Where(m => m.fk_student == userId).ToList();
             
                foreach (var projectAssignemt in projectAssignments)
                {                  
                    var projectEntity = db.Projects.Where(m => m.due_date <= DbFunctions.AddDays(DateTime.Now, 20) && m.id == projectAssignemt.fk_project).FirstOrDefault();

                    if(projectEntity != null)
                    {
                        projectList.Add(projectEntity);
                    }                 
                }
                
                var testAssignments = db.Test_Assignments.Where(m => m.fk_student == userId).ToList();
                               
                foreach(var testAssigment in testAssignments)
                {
                    var testEntity = db.Tests.Where(m => m.due_date <= DbFunctions.AddDays(DateTime.Now, 20) && m.id == testAssigment.fk_test).FirstOrDefault();
                    if(testEntity != null)
                    {
                        testsList.Add(testEntity);
                    }                   
                }    
            }

            ViewData["upcomingTests"] = testsList;
            ViewData["upcomingProjects"] = projectList;
            ViewData["schoolName"] = school.school_name;

            return View();
        }

        //
        // GET: /Account/ViewClasses
        [AllowAnonymous]        
        public ActionResult ViewClasses()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["userId"];
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            String userName = null;

            // We need to go to the look up table to fetch required fk's that associate users to classes. 
            using (var db = new Entities.Entities())
            {

                var classAssignments = db.Class_Assignements.Where(m => m.fk_student == userId).ToList();
                var tempClass = db.Classes.ToList();

                foreach (var classAssignment in classAssignments)
                {
                    foreach (var temp in tempClass)
                    {
                        if (classAssignment.fk_class == temp.id)
                        {
                            selectListItem.Add(new SelectListItem()
                            {
                                Text = temp.class_name,
                                Value = temp.id.ToString()
                            });
                        }
                    }
                }

                var user = db.SPM_Users.Find(userId);
                userName = user.users_name;
            }

            ViewData["userName"] = userName;
            ViewData["selectClasses"] = selectListItem;           
            return View();
        }

        //
        // Post: /Account/ViewClasses
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ViewClasses(ViewClassesModel model)
        {
            int userId = (int)Session["userId"];
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            List<Entities.Project> projectList = new List<Entities.Project>();
            List<Entities.Test> testList = new List<Entities.Test>();
            String userName = null;
            // We need to go to the look up table to fetch required fk's that associate users to classes. 
            using (var db = new Entities.Entities())
            {

                var classAssignments = db.Class_Assignements.Where(m => m.fk_student == userId).ToList();
                var tempClass = db.Classes.ToList();

                foreach (var classAssignment in classAssignments)
                {
                    foreach (var temp in tempClass)
                    {
                        if (classAssignment.fk_class == temp.id)
                        {
                            selectListItem.Add(new SelectListItem()
                            {
                                Text = temp.class_name,
                                Value = temp.id.ToString()
                            });
                        }
                    }
                }

                int classId = 0;
                Int32.TryParse(model.SelectedValue, out classId);

                var projectEntityList = db.Project_Assignments.Where(m => m.fk_class == classId && m.fk_student == userId).ToList();

                foreach(var project in projectEntityList)
                {
                    projectList.Add(db.Projects.Find(project.fk_project));
                }

                var testEntityList = db.Test_Assignments.Where(m => m.fk_class == classId && m.fk_student == userId).ToList();

                foreach (var test in testEntityList)
                {
                    testList.Add(db.Tests.Find(test.fk_test));
                }

                var user = db.SPM_Users.Find(userId);
                userName = user.users_name;
            }

            double classGrade = Calculations.calculateClassGradePercent(projectList, testList);
            if(!Double.IsNaN(classGrade) && classGrade > 0)
            {
                ViewData["classGrade"] = classGrade + "%";
            }
            else
            {
                ViewData["classGrade"] = "Grade not available";
            }
            ViewData["userName"] = userName;
            
            ViewData["projectList"] = projectList;
            ViewData["testList"] = testList;
            ViewData["selectClasses"] = selectListItem;

            return View();
        }


        //
        // GET: /Account/AddProjects
        [AllowAnonymous]
        [Route("Add-Project")]
        public ActionResult AddProject()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["userId"];
            List<SelectListItem> selectListItem = new List<SelectListItem>();               

            // We need to go to the look up table to fetch required fk's that associate users to classes. 
            using (var db = new Entities.Entities())
            {
                              
                var classAssignments = db.Class_Assignements.Where(m => m.fk_student == userId).ToList();
                var tempClass = db.Classes.ToList(); 
                        
                foreach(var classAssignment in classAssignments)
                {
                    foreach(var temp in tempClass)
                    {
                        if(classAssignment.fk_class == temp.id)
                        {
                            selectListItem.Add(new SelectListItem()
                            {
                                Text = temp.class_name,
                                Value = temp.id.ToString()
                            });
                        }
                    }                  
                }                               
            }

            ViewData["selectClasses"] = selectListItem;
            return View();
        }

        //
        // POST: /Account/AddProjects
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddProject(AddProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int gradeWeight = 0;
                    int gradeOutOf = 0;
                    int fk_class_id = 0;
                    Int32.TryParse(model.gradeWeight, out gradeWeight);
                    Int32.TryParse(model.gradeOutOf, out gradeOutOf);
                    Int32.TryParse(model.SelectedValue, out fk_class_id);

                    using (var db = new Entities.Entities())
                    {
                        db.InsertProjects(model.Name, null, gradeOutOf, gradeWeight, model.dueDate, fk_class_id, DateTime.Now, null);
                        var projects = db.Projects.Where(m => m.projects_name == model.Name).Where(m => m.fk_class_id == fk_class_id).First();
                        
                        int userId = (int)Session["userId"];
                        db.InsertProjectAssignments(projects.id, fk_class_id, userId, null);
                    }

                    return RedirectToAction("Home", "Account");
                }
                catch (Exception)
                {
                    return RedirectToAction("AddProject", "Account");
                }            
            }

            return View(model);             
        }

        //
        // GET: /Account/AddTests
        [AllowAnonymous]
        [Route("Add-Test")]
        public ActionResult AddTest()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["userId"];
            List<SelectListItem> selectListItem = new List<SelectListItem>();

            using (var db = new Entities.Entities())
            {

                var classAssignments = db.Class_Assignements.Where(m => m.fk_student == userId).ToList();
                var tempClass = db.Classes.ToList();

                foreach (var classAssignment in classAssignments)
                {
                    foreach (var temp in tempClass)
                    {
                        if (classAssignment.fk_class == temp.id)
                        {
                            selectListItem.Add(new SelectListItem()
                            {
                                Text = temp.class_name,
                                Value = temp.id.ToString()
                            });
                        }
                    }
                }
            }

            ViewData["selectClasses"] = selectListItem;
            return View();            
        }

        //
        // POST: /Account/AddTest
        [HttpPost]
        [AllowAnonymous] 
        [ValidateAntiForgeryToken]      
        public ActionResult AddTest(AddTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int gradeWeight = 0;
                    int gradeOutOf = 0;
                    int fk_class_id = 0;
                    Int32.TryParse(model.gradeWeight, out gradeWeight);
                    Int32.TryParse(model.gradeOutOf, out gradeOutOf);
                    Int32.TryParse(model.SelectedValue, out fk_class_id);

                    using (var db = new Entities.Entities())
                    {
                        db.InsertTests(model.Name, null, gradeOutOf, gradeWeight, model.dueDate, fk_class_id, DateTime.Now);
                        var tests = db.Tests.Where(m => m.tests_name == model.Name).Where(m => m.fk_class_id == fk_class_id).First();

                        int userId = (int)Session["userId"];
                        db.InsertTestAssignments(tests.id, fk_class_id, userId, null);
                    }

                    return RedirectToAction("Home", "Account");
                }
                catch (Exception)
                {
                    return RedirectToAction("AddProject", "Account");
                }
            }

            return View(model);
        }

        //GET: /Account/EditTest
        [AllowAnonymous]
        public ActionResult EditTest()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Entities.Test test;                        
            var url = Url.RequestContext.RouteData.Values["id"];
            int testId = 0;
            Int32.TryParse(url.ToString(), out testId);
            if(testId != 0)
            {
                using (var db = new Entities.Entities())
                {
                    var testEntity = db.Tests.Find(testId);
                    test = testEntity;
                }
            }else
            {
                test = null;
            }
           
            EditTestViewModel editTestViewModel = new EditTestViewModel();
            editTestViewModel.Id = test.id;
            editTestViewModel.dueDate = test.due_date;
            editTestViewModel.gradeOutOf = test.grade_out_of.ToString();
            editTestViewModel.gradeWeight = test.grade_weight.ToString();
            editTestViewModel.Name = test.tests_name;
            if (test.grade.HasValue)
            {
                editTestViewModel.grade = test.grade.ToString();
            }else
            {
                editTestViewModel.grade = null;
            }
            editTestViewModel.classId = test.fk_class_id.ToString();            
                   
            return View(editTestViewModel);
        }

       //POST: /Account/EditTest
       [HttpPost]
       [AllowAnonymous]
       [ValidateAntiForgeryToken]
       public ActionResult EditTest(EditTestViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                int grade = 0;
                int gradeOutOf = 0;
                int gradeWeight = 0;
                Int32.TryParse(model.grade.ToString(), out grade);
                Int32.TryParse(model.gradeOutOf.ToString(), out gradeOutOf);
                Int32.TryParse(model.gradeWeight.ToString(), out gradeWeight);

                using (var db = new Entities.Entities())
                {
                    try
                    {
                        db.UpdateTests(model.Id, model.Name, grade, gradeOutOf, gradeWeight, model.dueDate);
                    }
                    catch (Exception)
                    {
                        return View(model);
                    }
                   
                }

                return RedirectToAction("ViewClasses", "Account");
            }
            return View(model);
        }

        //GET: /Account/EditTest
        [AllowAnonymous]
        public ActionResult EditProject()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Entities.Project project;
            var url = Url.RequestContext.RouteData.Values["id"];
            int projectId = 0;
            Int32.TryParse(url.ToString(), out projectId);
            if (projectId != 0)
            {
                using (var db = new Entities.Entities())
                {
                    var projectEntity = db.Projects.Find(projectId);
                    project = projectEntity;
                }
            }
            else
            {
                project = null;
            }

            EditProjectViewModel editProjectViewModel = new EditProjectViewModel();
            editProjectViewModel.Id = project.id;
            editProjectViewModel.dueDate = project.due_date;
            editProjectViewModel.gradeOutOf = project.grade_out_of.ToString();
            editProjectViewModel.gradeWeight = project.grade_weight.ToString();
            editProjectViewModel.Name = project.projects_name;
            if (project.grade.HasValue)
            {
                editProjectViewModel.grade = project.grade.ToString();
            }
            else
            {
                editProjectViewModel.grade = null;
            }
            editProjectViewModel.classId = project.fk_class_id.ToString();

            return View(editProjectViewModel);
        }

        //POST: /Account/EditTest
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject(EditProjectViewModel model)
        {

            if (ModelState.IsValid)
            {
                int grade = 0;
                int gradeOutOf = 0;
                int gradeWeight = 0;
                Int32.TryParse(model.grade.ToString(), out grade);
                Int32.TryParse(model.gradeOutOf.ToString(), out gradeOutOf);
                Int32.TryParse(model.gradeWeight.ToString(), out gradeWeight);
                
                using (var db = new Entities.Entities())
                {
                    try
                    {
                        db.UpdateProjects(model.Id, model.Name, grade, gradeOutOf, gradeWeight, model.dueDate);
                    }
                    catch (Exception)
                    {
                        return View(model);
                    }

                }

                return RedirectToAction("ViewClasses", "Account");
            }
            return View(model);
        }
        //
        //GET: /Account/AddClass
        [AllowAnonymous]      
        public ActionResult AddClass()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        //
        //POST: /Account/AddClass
        // Required: Make sure to add id's to class look up table after adding a new class. table: Class_Assignments
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(AddClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int creditHours = 0;
                    Int32.TryParse(model.creditHours, out creditHours);

                    using (var db = new Entities.Entities())
                    {
                        int userId = (int)Session["userId"];
                        var user = db.SPM_Users.Where(m => m.id == userId).First();
                        
                        db.InsertClasses(model.Name, model.courseCode, model.teachersName, creditHours, user.fk_schools);
                        var classes = db.Classes.Where(m => m.class_name == model.Name).Where(m => m.fk_School == user.fk_schools).First();
                        db.InsertClassAssignments(classes.id, user.fk_schools, user.fk_programs, user.id, null);
                        return RedirectToAction("Home", "Account");
                    }
                }
                catch (Exception)
                {
                    return View(model);
                }
               
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult DeleteProject()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var url = Url.RequestContext.RouteData.Values["id"];
            int projectId = 0;
            Int32.TryParse(url.ToString(), out projectId);

            if(projectId != 0)
            {
                try
                {
                    using (var db = new Entities.Entities())
                    {
                        db.DeleteProjects(projectId);
                    }

                    return RedirectToAction("ViewClasses", "Account");
                }
                catch (Exception)
                {
                    return RedirectToAction("ViewClasses", "Account");
                }                           
            }

            return RedirectToAction("ViewClasses", "Account");
        }


        [AllowAnonymous]
        public ActionResult DeleteTest()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var url = Url.RequestContext.RouteData.Values["id"];
            int testId = 0;
            Int32.TryParse(url.ToString(), out testId);

            if (testId != 0)
            {
                try
                {
                    using (var db = new Entities.Entities())
                    {
                        db.DeleteTests(testId);
                    }

                    return RedirectToAction("ViewClasses", "Account");
                }
                catch (Exception)
                {
                    return RedirectToAction("ViewClasses", "Account");
                }
            }

            return RedirectToAction("ViewClasses", "Account");
        }

        [AllowAnonymous]        
        public JsonResult ProgramList(string id)
        {
            int programId = 0;
            Int32.TryParse(id, out programId);
                
            using (var db = new Entities.Entities())
            {
                var programs = db.Programs.Where(m => m.fk_school == programId).ToList();                                         
                return Json(programs, JsonRequestBehavior.AllowGet);
            }                   
        }

        [AllowAnonymous]
        public ActionResult ProgrogramListRegister(RegisterViewModel model)
        {
            int schoolFk = 0;
            Int32.TryParse(model.SelectedValue, out schoolFk);
            List<SelectListItem> programList = new List<SelectListItem>();
            using (var db = new Entities.Entities())
            {
                var programs = db.Programs.Where(m => m.fk_school == schoolFk).ToList(); 

                foreach(var program in programs)
                {
                    programList.Add(new SelectListItem { Text = program.programs_name, Value = program.id.ToString() });
                }

                return View(model);               
            }
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            List<SelectListItem> schoolList = new List<SelectListItem>();
           
            using (var db = new Entities.Entities())
            {               
                var schools = db.Schools.ToList();
                               
                foreach (Entities.School school in schools)
                {
                    schoolList.Add(new SelectListItem { Text = school.school_name, Value = school.id.ToString() });
                }
            }      
                      
               
            ViewData["schoolList"] = schoolList;
            return View();
        }     

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            List<SelectListItem> schoolList = new List<SelectListItem>();

            using (var db = new Entities.Entities())
            {
                var userName = db.SPM_Users.Where(m => m.users_name == model.UserName).ToList();
                var userEmail = db.SPM_Users.Where(m => m.email == model.Email).ToList();

                var schools = db.Schools.ToList();
                int schoolFk = 0;
                Int32.TryParse(model.SelectedValue, out schoolFk);

                foreach (Entities.School school in schools)
                {
                    schoolList.Add(new SelectListItem { Text = school.school_name, Value = school.id.ToString() });
                }

                var programs = db.Programs.Where(m => m.fk_school == schoolFk).ToList();

                
                List<SelectListItem> programList = new List<SelectListItem>();

                foreach (var program in programs)
                {
                    programList.Add(new SelectListItem { Text = program.programs_name, Value = program.id.ToString() });
                }
                ViewData["programList"] = programList;
                ViewData["schoolList"] = schoolList;

                if (userName.Count > 0)
                {
                    ViewData["userNameError"] = "User Name in use";
                    return View(model);
                }

                if (userEmail.Count > 0)
                {                    
                    ViewData["userEmailError"] = "Email in use";
                    return View(model);
                }

                //return View(model);
            }

            if (ModelState.IsValid)
            {                                             
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PasswordHash = model.Password };
               
                    int fk_school = 0;
                    Int32.TryParse(model.SelectedValue, out fk_school);
                    int fk_program = 0;
                    Int32.TryParse(model.ProgramSelectedValue, out fk_program);
                    var t = model.SelectSchool;
                    using (var db = new Entities.Entities())
                    {
                        db.InsertUsers(user.UserName, user.PasswordHash, fk_school, fk_program, user.Email.ToLower(), true, false, false, DateTime.Now, null, true, 0);                       
                        
                    }                
                               
                    return RedirectToAction("Login", "Account");            
            }        

            using (var db = new Entities.Entities())
            {
                var schools = db.Schools.ToList();

                foreach (Entities.School school in schools)
                {
                    schoolList.Add(new SelectListItem { Text = school.school_name, Value = school.id.ToString() });
                }
            }          

            return View(model);
        }

        //GET: /Account/RegisterAdmin
        [AllowAnonymous]
        public ActionResult RegisterAdmin()
        {
            return View();
        }

        //POST: /Account/RegisterAdmin
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterAdmin(RegisterAdminViewModel model)
        {
            if(model.Email.ToLower() != "gil.silberstein@myblueprint.ca" && model.Email.ToLower() != "scottlegrove@gmail.com" && model.Email.ToLower() != "mytest@spm.com")
            {
                ViewData["adminError"] = "You're not Gil!";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PasswordHash = model.Password };
                
                using (var db = new Entities.Entities())
                {
                    db.InsertUsers(user.UserName, user.PasswordHash, 2, 2, user.Email.ToLower(), false, false, true, DateTime.Now, null, true, 0);

                }

                return RedirectToAction("Login", "Account");
            }          
                        
            return View(model);
        }

        [HttpPost]
        public JsonResult checkIfUserNameExists(string userNameInput)
        {
            using (Entities.Entities context = new Entities.Entities())
            {
                var user = context.SPM_Users.Where(userName => userName.users_name == userNameInput).First();
                return Json(user == null);
            }              
        }

        [AllowAnonymous]
        public ActionResult AddSchool()
        {
            if (!String.IsNullOrEmpty(Session["isAdmin"].ToString()))
            {
                if (Session["isAdmin"].ToString() == "1")
                {
                    return View();
                }
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddSchool(AddSchoolViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Entities.Entities())
                {
                    try
                    {
                        db.InsertSchools(model.Name, model.Country, model.City);
                        return RedirectToAction("AddProgram", "Account");
                    }
                    catch (Exception)
                    {
                        return View(model);
                    }                  
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AddProgram()
        {
            if(!String.IsNullOrEmpty(Session["isAdmin"].ToString()))
            {
                if (Session["isAdmin"].ToString() == "1")
                {
                    List<SelectListItem> schoolList = new List<SelectListItem>();

                    using (var db = new Entities.Entities())
                    {
                        var schools = db.Schools.ToList();

                        foreach (Entities.School school in schools)
                        {
                            if (school.school_name != "admin")
                            {
                                schoolList.Add(new SelectListItem { Text = school.school_name, Value = school.id.ToString() });
                            }
                        }
                    }

                    ViewData["schoolList"] = schoolList;
                    return View();
                }
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddProgram(AddProgramViewModel model)
        {

            if (ModelState.IsValid)
            {
                using (var db = new Entities.Entities())
                {
                    int fk_school_id = 0;
                    Int32.TryParse(model.SelectedValue, out fk_school_id);
                    int semesters = 0;
                    Int32.TryParse(model.Semesters, out semesters);
                    try
                    {
                        db.InsertPrograms(fk_school_id, model.Name, semesters, model.CourseCode);
                        return RedirectToAction("AdminDashboard", "Account");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("AddProgram", "Account");
                    }
                }
            }

            List<SelectListItem> schoolList = new List<SelectListItem>();

            using (var db = new Entities.Entities())
            {
                var schools = db.Schools.ToList();              

                foreach (Entities.School school in schools)
                {
                    if (school.school_name != "admin")
                    {
                        schoolList.Add(new SelectListItem { Text = school.school_name, Value = school.id.ToString() });
                    }
                }
            }

            ViewData["schoolList"] = schoolList;
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CustomLogout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}