﻿using Client.Models;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Services;
using Core.Value_Objects;
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
        private static readonly int _pageSize = 10;
        private readonly BlogFilterService _filterService;
        private readonly IPostRepository<Post, Employee> _postRepository;

        public BlogController(IPostRepository<Post, Employee> postRepository, BlogFilterService filterService)
        {
            _postRepository = postRepository;
            _filterService = filterService;
        }

        // Display list of blog posts
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(postList)
        public IActionResult Index(BlogFilterParams filterParams, int pageNumber = 1)
        {
            IEnumerable<IPost<Employee>> posts = _postRepository.Queryable.Include(p => p.Author)
               .Where(post => !post.IsSoftDeleted).Select(p => (IPost<Employee>)p);

            var filteredPosts = _filterService.ApplyFilter(posts, filterParams);

            return View(new BlogListModel
            {
                Posts = PaginatedList<IPost<Employee>>.Create(filteredPosts.AsQueryable(), pageNumber, _pageSize),
                FilterParams = filterParams
            });
        }

        // Display detail of a blog post
        // Parameter id represent the id of the post, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no post with id is found, View(post) otherwise
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postRepository.Queryable
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (post == null)
            {
                return NotFound();
            }

            var recommendations = _postRepository.Queryable
                .Where(p => p.ID != id).Include(p => p.Author)
                .Take(5).AsEnumerable();

            return View(new PostDetailModel { 
                Post = post,
                Recommendations = recommendations
            });
        }
    }
}
