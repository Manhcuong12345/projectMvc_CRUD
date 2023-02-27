using DotNetCore5CRUD.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DotNetCore5CRUD.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DotNetCore5CRUD.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        ////http://localhost:5000/Users/app/user
        //[HttpGet("/app/user")]
        //Parameter là các biến được sử dụng trong một hàm mà giá trị của biến đó được cung cấp bởi lời gọi hàm
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Select(user => new User { Id = user.Id, Email = user.Email, PhoneNumber = user.PhoneNumber }).OrderBy(x => x.Id)
                .ToListAsync();
          
            return View(users);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            var users = new User { };
            return View("Create", users);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(User model)
        {
            if (ModelState.IsValid)
            {
                var users = new IdentityUser
                { 
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (id == null)
                return BadRequest();

            var users = await _userManager.FindByIdAsync(id);

            if (users == null)
                return NotFound();

            var user = new User
            {
                Id = users.Id,
                PhoneNumber = users.PhoneNumber,
                Email = users.Email,
            };

            return View("Update", user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(User model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "User");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", "User");
        }

        //Hàm check vai trò role cho user
        [HttpGet]
        public async Task<IActionResult> ManageRole(string userId)
        {
            //check tìm Id
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            //Dữ liệu thông tin của role
            var roles = await _roleManager.Roles.ToListAsync();
            var users = new UserRole
            {
                UserId = user.Id,
                //UserName = user.UserName,
                //select chỉ ra các dữ liệu được lấy ra từ checkbox
                Roles = roles.Select(role => new CheckBoxDto
                {
                    //gán bằng DisplayValue thành role.Name
                    DisplayValue = role.Name,
                    //trả về đối tượng được chỉ định có phải user là thành viên của vai trò đã được chọn hay không.
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };

            return View(users);
        }

        //Hàm add vai trò role user
        [HttpPost]
        public async Task<IActionResult> UpdateRole(UserRole model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            //nhận lưu các tên vai trò được chỉ định user thuộc về.
            var userRoles = await _userManager.GetRolesAsync(user);

            //Xóa vai trò được chỉ định user khỏi vai trò được lưu trc đó
            //(userRoles)Tên của vai trò để xóa người dùng.
            //(user)Người dùng cần xóa khỏi vai trò được đặt tên
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            //Add quyền cho người dùng
            //Người dùng để thêm vào các vai trò được đặt tên.
            //Tên của các vai trò để thêm người dùng vào.
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.DisplayValue));

            return RedirectToAction(nameof(Index));
        }

    }
}
