using System.Text;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public static class SeedDatas
    {
        public static async Task VerileriDoldurAsync(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if (context!=null)
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag() { Text = "web programlama" ,Url ="web-programlama",Color = Tag.TagColors.primary},
                        new Tag() { Text = "backend",Url = "back-end",Color = Tag.TagColors.danger},
                        new Tag() { Text = "frontend" ,Url ="front-end",Color = Tag.TagColors.success},
                        new Tag() { Text = "fullstack" ,Url = "full-stack",Color = Tag.TagColors.warning},
                        new Tag() { Text = "javascript" ,Url ="js",Color = Tag.TagColors.secondary},
                        new Tag() { Text = "Data" ,Url ="data",Color = Tag.TagColors.info}
                    );
                    await context.SaveChangesAsync();

                }

                
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User() { UserName = "mertcalik", Email = "Mertclkdev@gmail.com", UserId = 1,Image = "syl.jpg",Name="Mert",SurName="Çalık"}
                    );
                    await context.SaveChangesAsync();

                }



                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(

                        new Post() { Title = "Asp.Net Core Mvc", Content = "Asp.Net Core Mvc Dersleri", IsActive = true, Tags = context.Tags.Where(x=>x.TagId==1||x.TagId==3||x.TagId==4).ToList(),User = context.Users.FirstOrDefault(x => x.UserId == 1),Image = "1.png",Url = "aspnet-core",PublishedOn = DateTime.Now.AddDays(1),Comments = new List<Comment>{new Comment{CommentText = "Yav harikaa harikaaaaaa",User = context.Users.FirstOrDefault(x=>x.UserId==1),},new Comment{ CommentText = "Yav harikaa harikaaaaaa", User = context.Users.FirstOrDefault(x => x.UserId == 1)}},Description = ".Net dersleri serisi"},
                        new Post() { Title = "Asp.Net Core Web Api", Content = "Asp.Net Core Web Api Dersleri", IsActive = true, Tags = context.Tags.Take(3).ToList(), User = context.Users.FirstOrDefault(x => x.UserId == 2), Image = "2.png", Url = "aspnet-core-web-api",PublishedOn = DateTime.Now,Comments = new List<Comment>(){new Comment{CommentText = "Çok iyi çok faydalı",User = context.Users.FirstOrDefault(x=>x.UserId==1)},new Comment { CommentText = "Çok iyi çok faydalı", User = context.Users.FirstOrDefault(x => x.UserId == 2) } }, Description = ".Net dersleri serisi" },
                        new Post() { Title = "Angular Dersleri", Content = "Angular Dersleri", IsActive = false, Tags = context.Tags.Take(3).ToList(), User = context.Users.FirstOrDefault(x => x.UserId == 1), Image = "3.png",Url = "angular-js",PublishedOn = DateTime.Now.AddDays(-1), Description = "FrontEnd dersleri serisi" },
                        new Post() { Title = "Sql Dersleri", Content = "Sql Dersleri", IsActive = false, Tags = context.Tags.Where(x=>x.TagId==4).ToList(), User = context.Users.FirstOrDefault(x => x.UserId == 1), Image = "4.png",Url = "sql-db",PublishedOn =DateTime.Now.AddDays(-15), Description = "Data management dersleri serisi" },
                        new Post() { Title = "Bootstrap Dersleri", Content = "Bootstrap Dersleri", IsActive = false, Tags = context.Tags.Where(x=>x.TagId==2).ToList(), User = context.Users.FirstOrDefault(x => x.UserId == 1), Image = "5.png",Url = "bootstrap",PublishedOn = DateTime.Now.AddDays(-30), Description = "FrontEnd dersleri serisi" }
                    );
                    await context.SaveChangesAsync();

                }

                if (!context.Replies.Any())
                {
                    context.Replies.AddRange(
                        
                        new Reply() { CommentId = 1, UserId = 1, Text = "Harika yorum" },
                        new Reply() { CommentId = 1, UserId = 1, Text = "Harika yorum" });
                }
                await context.SaveChangesAsync();

            }



        }
    }
}
