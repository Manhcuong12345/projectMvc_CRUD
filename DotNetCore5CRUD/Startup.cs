using DotNetCore5CRUD.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System;

namespace DotNetCore5CRUD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // This method gets called by the runtime. Use this method to add services to the container

        //Routing là một quá trình khi ASP.NET Core xem xét các URL request gửi đến và "chỉ đường"
        //cho nó đến Controller Actions. Nó cũng được sử dụng để tạo ra URL đầu ra

        //sử dụng Route để đến controller action.

        //IServiceCollection đối tượng này để đăng ký các dịch vụ vào ứng dụng. Các đăng ký sẽ thực hiện ở phương thức
        //ConfigureServices của lớp Startup
        public void ConfigureServices(IServiceCollection services)
        {
            //Đăng ký AppDbContext và các dịch vụ Identity vào hệ thống
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
            {
                ProgressBar = true,
                PositionClass = ToastPositions.TopRight,
                PreventDuplicates = true,
                CloseButton = true
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

            });

            // Cấu hình Cookie
            services.ConfigureApplicationCookie(options => {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = $"/Account/Login/";                                 // Url đến trang đăng nhập
                options.LogoutPath = $"/Account/Logout/";
            
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //cấu hình middleware trong configure bằng cách sử dụng thể hiện của interface IApplicationBuilder

        //IApplicationBuilder dùng để chia nhánh Pipeline. Nó phân nhánh Asp.net Core Pipeline dựa vào việc khớp đường dẫn Request. Nếu 
        //đường dẫn Request bắt đầu vớiđường dẫn đã cho,Middleware trên nhánh đó sẽ được thực thi.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            // đăng ký route
            // phần thứ nhất tương ứng với tên Controller (tên lớp trong thư mục Controllers, ví dụ này là HomeController),
            // phần thứ 2 biến action - là Action trong Controller (tên phương thức), phần thứ 3 là một tham số tên biến id
            // code đây đã tạo ra một Route ánh xạ URL vào Controller
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
