using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class LatestPostsMenu:ViewComponent
    {
        private readonly IPostRepository _postRepository;
        public LatestPostsMenu(IPostRepository postRepository)
        {
            _postRepository=postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latesPosts=await _postRepository.Posts.OrderByDescending(x=>x.PublishedOn).Take(8).Where(x=>x.IsActive).ToListAsync();
            return View(latesPosts);
        }
    }
}
