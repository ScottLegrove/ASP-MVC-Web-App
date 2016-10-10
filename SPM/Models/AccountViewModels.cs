using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPM.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be atleast {2} characters long", MinimumLength = 4)]
        [Display(Name = "User Name")]
        //[System.Web.Mvc.Remote("checkIfUserNameExists", "Account", HttpMethod = "POST", ErrorMessage = "User name already in use.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }       
       
        public IEnumerable<System.Web.Mvc.SelectListItem> SelectSchool { get; set; }

        [Required]
        [Display(Name = "Select a school")]
        public string SelectedValue { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> SelectProgram { get; set; }

        [Required]
        [Display(Name = "Select a program")]
        public string ProgramSelectedValue { get; set; }
    }


    public class AddProjectViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Grade out of")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeOutOf { get; set; }

        [Required]
        [Display(Name = "Grade Weight")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeWeight { get; set; }         
        
        [Required]
        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        public DateTime dueDate { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> SelectClass{ get; set; }

        [Required]
        [Display(Name = "Class")]
        public string SelectedValue { get; set; }
    }

    public class AddClassViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
      
        [Display(Name = "Class Code")]       
        public string courseCode { get; set; }
      
        [Display(Name = "Teacher's Name")]        
        public string teachersName { get; set; }

        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        [Display(Name = "Credit Hours")]       
        public string creditHours { get; set; }
    }
    public class AddTestViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Grade out of")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeOutOf { get; set; }

        [Required]
        [Display(Name = "Grade Weight")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeWeight { get; set; }

        [Required]
        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        public DateTime dueDate { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> SelectClass { get; set; }

        [Required]
        [Display(Name = "Class")]
        public string SelectedValue { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ViewClassesModel{

        [Key]
        public int Id { get; set; }

        [Display(Name = "Select a class")]
        public IEnumerable<System.Web.Mvc.SelectListItem> SelectClass { get; set; }

        [Required]
        [Display(Name = "Class")]
        public string SelectedValue { get; set; }
    }

    public class EditTestViewModel {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required]
        [Display(Name = "Grade")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string grade { get; set; }

        [Required]
        [Display(Name = "Grade out of")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeOutOf { get; set; }

        [Required]
        [Display(Name = "Grade Weight")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeWeight { get; set; }

        [Required]
        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime dueDate { get; set; }       

        [Required]
        [Display(Name = "Class")]
        public string classId { get; set; }
    }

    public class EditProjectViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required]
        [Display(Name = "Grade")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string grade { get; set; }

        [Required]
        [Display(Name = "Grade out of")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeOutOf { get; set; }

        [Required]
        [Display(Name = "Grade Weight")]
        [Range(0, 1000, ErrorMessage = "Please enter valid number")]
        public string gradeWeight { get; set; }

        [Required]
        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime dueDate { get; set; }

        [Required]
        [Display(Name = "Class")]
        public string classId { get; set; }
    }

    public class RegisterAdminViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be atleast {2} characters long", MinimumLength = 4)]
        [Display(Name = "User Name")]       
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }      
    }

    public class AddSchoolViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "City")]      
        public string City { get; set; }

        [Required]
        [Display(Name = "Country")]       
        public string Country { get; set; }
    }

    public class AddProgramViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Semesters")]
        [Range(0, 10, ErrorMessage = "Please enter valid number")]
        public string Semesters { get; set; }

        [Required]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Select a School")]
        public IEnumerable<System.Web.Mvc.SelectListItem> SelectClass { get; set; }

        [Required]
        [Display(Name = "School")]
        public string SelectedValue { get; set; }
    }

}
