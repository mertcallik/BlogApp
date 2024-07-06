using System;
using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concreate.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly ITagRepository _tagRepository;

        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, IReplyRepository replyRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _replyRepository = replyRepository;
            _tagRepository = tagRepository;
        }

        private int _pagesize=4;
        [HttpGet]
        public async Task<IActionResult> Index(string? url, string? val,int page=1)
        {
            var posts = _postRepository.Posts;

            if (url != null)
            {
                posts = posts.Where(x => x.Tags.Any(x => x.Url == url));

            }

            if (val != null)
            {
                posts = posts.Where(x => x.Title.Contains(val));
            }



            var postViewModel = new PostViewModel()
            {
                Posts = await posts.Skip((page - 1) * _pagesize).Where(x => x.IsActive).Take(_pagesize).ToListAsync(),
                PostInfo = new PostInfo()
                {
                    PostPerPage = _pagesize,
                    TotalPosts = _postRepository.Posts.Count(p=>p.IsActive),
                    CurrentPage = page
                }
            };
            return View(postViewModel);

        }
        [HttpGet]
        public async Task<IActionResult> Details(string? url)
        {

            var post = await _postRepository.Posts.Include(x => x.Tags)

                .Include(x => x.User)
                .Include(x => x.Comments).ThenInclude(x => x.User).ThenInclude(x => x.Replies)
                .FirstOrDefaultAsync(x => x.Url == url);

            return View(post);
        }
        [HttpPost]
        public async Task<JsonResult> AddComment(string Text, string postId /*,string url*/)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new
                {
                    error = "Lütfen giriş yapınız"
                });
            }
            var comment = new Comment()
            {
                CommentText = Text,
                PublishedOn = DateTime.Now,
                PostId = int.Parse(postId),
                UserId = int.Parse(userId ?? ""),

            };
            await _commentRepository.CreateCommentAsync(comment);


            //return Redirect($"/blogs/details/{url}");
            // return RedirectToRoute("post_details",new {url=url});
            return Json(new
            {
                userName,
                Text,
                comment.PublishedOn,
                avatar
            });
        }
        [Authorize]
        public IActionResult Create()
        {
            var viewModel = new PostCreateViewModel
            {
                Tags = new SelectList(_tagRepository.Tags, "TagId", "Text")
            };


            return View("CreatePost", viewModel);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PostCreateViewModel model, IFormFile? file, int[]? tagIds)
        {

            if (!ModelState.IsValid)
            {
                return View("CreatePost", new PostCreateViewModel(){Tags = new SelectList(_tagRepository.Tags,"TagId","Text")});

            }

            model.Image= "emptypost.jpg";
            if (file != null)
            {
                var randomfileName = await ImageUpluader(file);
                model.Image = randomfileName;
            }



            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);



            await _postRepository.CreatePostAsync(new Post()
            {
                Title = model.Title,
                Content = model.Content,
                UserId = int.Parse(userId ?? ""),
                Image = model.Image,
                Url = model.Url,
                IsActive = false,
                Description = model.Description,
                Tags = new List<Tag>(_tagRepository.Tags.Where(tag=>tagIds.Contains(tag.TagId)).ToList()),
                
        });

            return RedirectToAction("Index");
    }
    [Authorize]
    public async Task<IActionResult> ListOfPosts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        var role = User.FindFirstValue(ClaimTypes.Role) == "admin" ? "admin" : null;
        var posts = _postRepository.Posts;
        if (string.IsNullOrEmpty(role))
        {
            posts = posts.Where(x => x.User.UserId == userId);
        }
        return View(await posts.ToListAsync());
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Reply(int commentId, string? Text, string url)
    {
        if (Text == null)
        {
            return RedirectToAction("Details", new { url });
        }
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var avatar = User.FindFirstValue(ClaimTypes.UserData);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        await _replyRepository.AddReply(new Reply
        {
            UserId = userId,
            Text = Text,
            CommentId = commentId
        });

        return RedirectToAction("Details", new { url });
    }

    [HttpPost]
    public async Task<IActionResult> ListOfPosts(List<Post> posts)
    {
        foreach (var post in posts)
        {
            await _postRepository.BatchUpdatePostAsync(post);
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Edit(string? url)
    {
        if (url == null)
        {
            return RedirectToAction("Index");
        }
        var userId = Convert.ToInt16(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
        var post = _postRepository.Posts.Include(x=>x.Tags).FirstOrDefault(x => x.Url == url);
        if (!(post!.UserId == userId || role == "admin"))
        {
            return RedirectToAction("Index");
        }

        var postUpdateViewModel = new PostUpdateViewModel
        {
            PostId = post.PostId,
            Content = post.Content,
            Description = post.Description,
            Title = post.Title,
            Image = post.Image,
            IsActive = post.IsActive,
            Tags = post.Tags.ToList(),
            AllTags = _tagRepository.Tags.ToList()
        };
        return View(postUpdateViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(PostUpdateViewModel postModel, IFormFile? file, int[]? tagIds)
    {
        if (!ModelState.IsValid)
        {
            return View(postModel);
        }

        if (file != null)
        {
           var randomfileName= await ImageUpluader(file);
            postModel.Image = randomfileName;
        }

        var entityToUpdate = new Post
        {
            PostId = postModel.PostId,
            Content = postModel.Content,
            Description = postModel.Description,
            Image = postModel.Image,
            Title = postModel.Title,
            IsActive = postModel.IsActive,
            Tags = new List<Tag>(_tagRepository.Tags.Where(x=>tagIds.Contains(x.TagId)).ToList())
        };
        await _postRepository.UpdatePostAsync(entityToUpdate);
        return RedirectToAction("ListOfPosts", "Posts");
    }

    public async Task<string> ImageUpluader(IFormFile? file)
    {
        var allowedExtensions = new string[] { ".jpg", ".png", ".webp", ".gif", ".jpeg" };
        var extension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("", "Resim dosyanız dogru formatta değil.");
        }

        var randomfileName = string.Format($"{Guid.NewGuid()}{extension}");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomfileName);
        using (var stream = new FileStream(path, FileMode.Append))
        {
            await file.CopyToAsync(stream);
        }

        return randomfileName.ToString();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(string? url)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        var role = User.FindFirstValue(ClaimTypes.Role) == "admin" ? "admin" : "";
        if (url == null)
        {
            return RedirectToAction("ListOfPosts");
        }

        var post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.Url == url);
        if (!(post.UserId == userId || role == "admin"))
        {
            return BadRequest();
        }
        return View("DeleteConfirm", post);
    }
        //x
    [HttpPost]
    public async Task<IActionResult> Delete(Post post)
    {
        var entity = await _postRepository.Posts.Include(x => x.Comments)!.ThenInclude(x => x.Replies).FirstOrDefaultAsync(x => x.Url == post.Url);
        await _postRepository.Delete(entity);
        return RedirectToAction("ListOfPosts");
    }
}
}
