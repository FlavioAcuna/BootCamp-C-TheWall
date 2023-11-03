using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TheWall.Models;

namespace TheWall.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()
    {

        return View();
    }
    [HttpPost("user/create")]
    public IActionResult RegisterUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("DashboardMessage");
        }
        else
        {
            return View("Index");
        }
    }

    [HttpPost("")]
    public IActionResult ValidaUser(LoginUser userLogin)
    {
        if (ModelState.IsValid)
        {

            User? UserExist = _context.users.FirstOrDefault(u => u.Email == userLogin.EmailLogin);
            if (UserExist == null)
            {
                ModelState.AddModelError("EmailLogin", "Correo o contraseña invalidos");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userLogin, UserExist.Password, userLogin.PasswordLogin);
            if (result == 0)
            {
                ModelState.AddModelError("EmailLogin", "Correo o contraseña invalidos");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", UserExist.UserId);
            return RedirectToAction("DashboardMessage");
        }
        else
        {
            return View("Index");
        }
    }
    [SessionCheck]
    [HttpGet("messages")]
    public IActionResult DashboardMessage()
    {
        int? UserID = HttpContext.Session.GetInt32("UserId");
        ViewBag.Nombre = _context.users.FirstOrDefault(u => u.UserId == UserID);
        int newMessaggeId = _context.messages.Where(u => u.UserId == (int)UserID).OrderByDescending(w => w.MessageId).Select(w => w.MessageId).FirstOrDefault();
        TheWallViewModel viewmodel = new TheWallViewModel
        {
            newListMsg = _context.messages.Include(u => u.MessageComment).Include(u => u.Creador).ToList(),
            newMsg = new Message(),
            newComment = new Comment(),
            newListComm = _context.comments.Include(Co => Co.CommentMessage).Where(c => c.MessageId == newMessaggeId).ToList()
        };
        return View("MessageView", viewmodel);
    }


    [HttpGet("")]
    public IActionResult LogOut()
    {
        HttpContext.Session.SetString("UserId", "");
        HttpContext.Session.Clear();

        List<Comment> omment = _context.comments.Include(Co => Co.CommentMessage).Where(p => p.MessageId == 10).ToList();
        foreach (Comment item in omment)
        {
            Console.WriteLine($"Item comentario{item.CommentText}");
        }
        return View("index");
    }
    [SessionCheck]
    [HttpPost("messages")]
    public IActionResult AddMessage(Message newMessage)
    {
        int? UserID = HttpContext.Session.GetInt32("UserId");
        ViewBag.Nombre = _context.users.FirstOrDefault(u => u.UserId == UserID);
        int newMessaggeId = _context.messages.Where(u => u.UserId == (int)UserID).OrderByDescending(w => w.MessageId).Select(w => w.MessageId).FirstOrDefault();
        TheWallViewModel viewmodel = new TheWallViewModel
        {
            newListMsg = _context.messages.Include(u => u.MessageComment).Include(u => u.Creador).ToList(),
            newMsg = new Message(),
            newComment = new Comment(),
            newListComm = _context.comments.Include(Co => Co.CommentMessage).Where(c => c.MessageId == newMessaggeId).ToList()
        };
        if (ModelState.IsValid)
        {
            newMessage.UserId = (int)UserID;
            _context.messages.Add(newMessage);
            _context.SaveChanges();
            return RedirectToAction("DashboardMessage");
        }
        else
        {
            return View("MessageView", viewmodel);
        }
    }
    [SessionCheck]
    public IActionResult AddComment(Comment newComment, int MessId)
    {
        int? UserID = HttpContext.Session.GetInt32("UserId");
        ViewBag.Nombre = _context.users.FirstOrDefault(u => u.UserId == UserID);
        int newMessaggeId = _context.messages.Where(u => u.UserId == (int)UserID).OrderByDescending(w => w.MessageId).Select(w => w.MessageId).FirstOrDefault();
        TheWallViewModel viewmodel = new TheWallViewModel
        {
            newListMsg = _context.messages.Include(u => u.MessageComment).Include(u => u.Creador).ToList(),
            newMsg = new Message(),
            newComment = new Comment(),
            newListComm = _context.comments.Include(Co => Co.CommentMessage).Where(c => c.MessageId == newMessaggeId).ToList()
        };
        if (ModelState.IsValid)
        {

            int ussid = (int)UserID;
            newComment.UserId = ussid;

            MessId = _context.messages.Where(u => u.UserId == ussid).OrderByDescending(w => w.MessageId).Select(w => w.MessageId).FirstOrDefault();
            Comment? addfirstcom = new Comment();
            addfirstcom.MessageId = MessId;
            addfirstcom.UserId = ussid;
            addfirstcom.CommentText = newComment.CommentText;
            _context.comments.Add(addfirstcom);
            _context.SaveChanges();
            return RedirectToAction("DashboardMessage");
        }
        else
        {
            return View("MessageView", viewmodel);
        }
    }

    [SessionCheck]
    public IActionResult DeleteMessage(int MesId)
    {
        int? UserId = HttpContext.Session.GetInt32("UserId");
        Message? selMsg = _context.messages.SingleOrDefault(msg => msg.MessageId == MesId);
        selMsg.UserId = (int)UserId;
        List<Comment> MessageInCommet = _context.comments.Where(cm => cm.MessageId == MesId).ToList();
        selMsg.MessageId = MesId;
        foreach (var item in MessageInCommet)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine($"-------------{item}-----------");
            Console.WriteLine("------------------------");
            _context.comments.Remove(item);
        }
        _context.messages.Remove(selMsg);
        _context.SaveChanges();
        return RedirectToAction("DashboardMessage");
    }
    [SessionCheck]
    public IActionResult DeletComment(int ComID)
    {
        int? UserId = HttpContext.Session.GetInt32("UserId");
        Comment? selCom = _context.comments.SingleOrDefault(com => com.CommentId == ComID);
        selCom.UserId = (int)UserId;
        selCom.CommentId = ComID;
        _context.comments.Remove(selCom);
        _context.SaveChanges();
        return RedirectToAction("DashboardMessage");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        //Encontrar la sesion 
        int? UserId = context.HttpContext.Session.GetInt32("UserId");
        if (UserId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}