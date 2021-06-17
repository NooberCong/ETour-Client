using Client.Models;
using Core.Interfaces;
using Infrastructure.InterfaceImpls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostRepository<Post,Employee> _blogRepository;

        public BlogController(IPostRepository<Post, Employee> blogRepository)
        {
            _blogRepository = blogRepository;
           
        }

        // Display list of blog posts
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(postList)
        public IActionResult Index()
        {

            IEnumerable<Post> BlogList = _blogRepository.Queryable.Include(p => p.Author)
               .Where(post =>  !post.IsSoftDeleted).AsEnumerable();
            return View(new BlogListModel
            {
                Posts = BlogList
            }) ;
        }

        // Display detail of a blog post
        // Parameter id represent the id of the post, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no post with id is found, View(post) otherwise
        public IActionResult Details(int? id)
        {
            return View();
        }
    }
}
