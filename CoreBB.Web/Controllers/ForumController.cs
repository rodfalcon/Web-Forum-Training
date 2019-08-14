﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private CoreBBContext _dbContext;

        [HttpGet]
        public IActionResult Index()
        {
            var forums = _dbContext.Forum.Include("Owner").ToList();
            return View(forums);
        }

        public ForumController(CoreBBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(Forum model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Informações inválidas");
            }

            model.OwnerId = _dbContext.User.SingleOrDefault(u => u.Name == User.Identity.Name).Id;
            model.CreateDateTime = DateTime.Now;
            await _dbContext.Forum.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Detail (int id)
        {
            var forum = _dbContext.Forum.Include("Owner").SingleOrDefault(f => f.Id == id);
            if (forum == null) { throw new Exception("Fórum Inexistente");  }
            return View(forum);
        }

        [HttpGet, Authorize (Roles = Roles.Administrator)]
        public IActionResult Delete (int id)
        {
            var forum = _dbContext.Forum.Include("Owner").SingleOrDefault(f => f.Id == id);
            if (forum == null) { throw new Exception("Fórum Inexistente"); }
            return View(forum);
        }
        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Delete(Forum model)
        {
            var forum = _dbContext.Forum.SingleOrDefault(f => f.Id == model.Id);
            if (forum == null)
            {
                throw new Exception("Fórum Inexistente");
            }
            _dbContext.Forum.Remove(forum);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public IActionResult Edit(int id)
        {
            var forum = _dbContext.Forum.Include("Owner").SingleOrDefault(f => f.Id == id);
            if (forum == null)
            {
                throw new Exception("Forum does not exist.");
            }

            return View(forum);
        }
        [HttpPost, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Edit(Forum model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid forum information.");
            }

            _dbContext.Forum.Update(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}