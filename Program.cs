using BlogApp.Data.Abstract;
using BlogApp.Data.Concreate.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BlogApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IPostRepository, EfPostRepository>();
            builder.Services.AddScoped<ITagRepository, EfTagRepository>();
            builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
            builder.Services.AddScoped<IUserRepository, EfUserRepository>();
            builder.Services.AddScoped<IReplyRepository, EfReplyRepository>();
            builder.Services.AddDbContext<BlogContext>(opt =>
                opt.UseMySql(builder.Configuration.GetConnectionString("mysql_connection"),ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql_connection"))));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options=>
            {
                options.LoginPath = "/Auth/Login";
                
            });
            var app = builder.Build();
            SeedDatas.VerileriDoldurAsync(app);

            //BlogApp
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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
            

            app.MapControllerRoute("my_blogs_edit", "blogs/details/{url}", new { controller = "Posts", action = "Details" });
            app.MapControllerRoute("post_details", "myblogs/edit/{url}", new { controller = "Posts", action = "Edit" });
            app.MapControllerRoute("post_details", "myblogs/delete/{url}", new { controller = "Posts", action = "Delete" });
            app.MapControllerRoute("auth_user_login", "Auth/Login/{model?}", new { controller = "Users", action = "Login" });
            app.MapControllerRoute("auth_user_register", "Auth/Register", new { controller = "Users", action = "Register" });
            app.MapControllerRoute("user_profile", "profile/{username}", new { controller = "Users", action = "Profile" });
            app.MapControllerRoute("post_by_tag", "posts/tags/{url}", new { controller = "Posts", action = "Index" });
            app.MapControllerRoute("post_manager", "myposts/all/{username}", new { controller = "Posts", action = "ListOfPosts" });
            //app.MapControllerRoute("post_manager", "profile/edit", new { controller = "Users", action = "EditProfile" });
            app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
