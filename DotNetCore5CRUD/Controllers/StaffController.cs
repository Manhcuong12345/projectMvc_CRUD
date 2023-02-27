
using DotNetCore5CRUD.Dto;
using DotNetCore5CRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5CRUD.Controllers
{
   
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public StaffController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        [Authorize(Roles = "Admin,SuperAdmin,Printer")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Staff.OrderByDescending(m => m.Id).ToListAsync();
            return View(users);
        }

        [Authorize(Roles = "SuperAdmin")]
        //Parameter là các biến được sử dụng trong một hàm mà giá trị của biến đó được cung cấp bởi lời gọi hàm
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var staff = new Staff{};
            return View("StaffForm", staff);
        }

        //Đối với các phương thức post thì parameter đc truyền từ model User 
        //chứa các thông tin gửi đến trong một đối tượng HTTP Request.Đối tượng này có được bằng truy cập thuộc tính của Request của Controller
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(Staff model)
        {
            //kiểm tra nếu ModelState.IsValid nếu có bất cứ lỗi nào thì chúng ta trả về cho người dùng
            if (!ModelState.IsValid)
            {
                return View("StaffForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("StaffForm", model);
            }

            var poster = files.FirstOrDefault();

            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
               // model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("StaffForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("StaffForm", model);
            }

            using var dataStream = new MemoryStream();

            await poster.CopyToAsync(dataStream);

            var staffs = new Staff
            {
                Title = model.Title,
                Job = model.Job,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Poster = dataStream.ToArray()
            };

            _context.Staff.Add(staffs);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie created successfully");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var users = await _context.Staff.FindAsync(id);

            if (users == null)
                return NotFound();

            var staffs = new StaffDto
            {
                Id = users.Id,
                PhoneNumber = users.PhoneNumber,
                FullName = users.FullName,
                Email = users.Email,
                Poster = users.Poster
                
            };

            return View("Edit", staffs);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(StaffDto model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.UserName = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            //    return View("MovieForm", model);
            //}

            var user = await _context.Staff.FindAsync(model.Id);

            if (user == null)
                return NotFound();

            var files = Request.Form.Files;

            //Kiểm tra đk add ảnh và lưu vào db
            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                //kiểm tra đuôi loại file ảnh
                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("Edit", model);
                }

                //kiểm tra dung lượng file 
                if (poster.Length > _maxAllowedPosterSize)
                {
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("Edit", model);
                }


                user.Poster = model.Poster;
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie updated successfully");
            return RedirectToAction(nameof(Index));
        }

        //Hàm lấy thông tin chi tiết
        [Authorize(Roles = "Admin,SuperAdmin,Printer")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

           var user = await _context.Staff.SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [Authorize(Roles ="SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var user = await _context.Staff.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Staff.Remove(user);
            _context.SaveChanges();

            return Ok();
        }
    }
}