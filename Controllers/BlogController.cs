using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class BlogController : Controller
    {
        private static readonly int _pageSize = 6;

        // Display list of blog posts
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(postList)
        public IActionResult Index(int pageNumber = 1)
        {
            return View();
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
