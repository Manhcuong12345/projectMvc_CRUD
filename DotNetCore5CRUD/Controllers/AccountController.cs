using DotNetCore5CRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Threading.Tasks;
using DotNetCore5CRUD.Dto;

namespace DotNetCore5CRUD.Controllers
{
    //base dotnet tu sinh
    //contructer nghĩa là nó chỉ thực hiện xử lý trong phạm vi khai báo controller đó.
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //đây là một hàm khởi tạo tham số đc khai báo ở trên
        public AccountController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

      
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        ////http://localhost:5000/Users/app/user
        //[HttpGet("/app/user")]
        //Parameter là các biến được sử dụng trong một hàm mà giá trị của biến đó được cung cấp bởi lời gọi hàm
        //Model Binding là cơ chế map dữ liệu được gửi qua HTTP Request vào các tham số của action method trong Controller
        //các giá trị trong form sẽ tự động được map vào đối tượng User trong Action method của controller
        //ta đã tạo một form cơ bản cho phép nhận một đối tượng Register. Khi người dùng click nút Submit thì dữ liệu sẽ được
        //post lên phương thức Create trên Controller
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            //kiểm tra nếu ModelState.IsValid nếu có bất cứ lỗi nào thì chúng ta trả về cho người dùng
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                { 
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber= model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError("", error.Description);
                //}

                ModelState.AddModelError(string.Empty, "Đăng ký không hợp lệ");

            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        //Single là lấy ra all kiểm tra nếu trong bảng data có 2 trường id giôn
        //firstor có thể lấy ra null
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe,true);

                if (result.Succeeded)
                { 
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ");

            }
            return View(user);
        }

       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
