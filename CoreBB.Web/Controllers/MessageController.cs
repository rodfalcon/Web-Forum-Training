using System;
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
    public class MessageController : Controller
    {
        private CoreBBContext _dbContext;
        public MessageController(CoreBBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var user = _dbContext.User.SingleOrDefault(u => u.Name == User.Identity.Name);
            var messages = _dbContext.Message.Include("ToUser").Include("FromUser")
            .Where(m => m.ToUserId == user.Id || m.FromUserId == user.Id);
            return View(messages);
        }

        public IActionResult Create(string toUserName)
        {
            var toUser = _dbContext.User.SingleOrDefault(u => u.Name == toUserName);
            if(toUser == null)
            {
                throw new Exception("Usuário inexistente");
            }
            var message = new Message { ToUserId = toUser.Id, ToUser = toUser };
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Message model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Informação de mensagem inválido");
            }
            var fromUser = _dbContext.User.SingleOrDefault(u => u.Name == User.Identity.Name);
            model.FromUserId = fromUser.Id;
            model.SendDateTime = DateTime.Now;
            await _dbContext.Message.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var message = _dbContext.Message.Include("ToUser").Include("FromUser").SingleOrDefault(m => m.Id == id);
            if(message == null)
            {
                throw new Exception("Mensagem não existe");
            }
            var user = _dbContext.User.SingleOrDefault(u => u.Name == User.Identity.Name);
            if(message.ToUserId != user.Id && message.FromUserId != user.Id)
            {
                throw new Exception("Acesso a mensagem proibido");
            }
            if(message.ToUserId == user.Id)
            {
                message.IsRead = true;
            }
            _dbContext.Message.Update(message);
            await _dbContext.SaveChangesAsync();
            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var message = _dbContext.Message.SingleOrDefault(m => m.Id == id);
            if(message == null)
            {
                throw new Exception("Mensagem não existe");
            }
            var user = _dbContext.User.SingleOrDefault(u => u.Name == User.Identity.Name);
            if(message.ToUserId != user.Id && message.FromUserId != user.Id)
            {
                throw new Exception("Acesso a mensagem negado"); ;
            }
            _dbContext.Message.Remove(message);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }


    }
}