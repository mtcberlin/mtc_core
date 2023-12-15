namespace MtcMvcCore.Areas.Admin.Pages.UserManager.Models
{
	public class UserViewModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }
		public string NewPw { get; set; }
		public bool IsActive { get; set; }

	}
}
