using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quotingDojo.Models;
using quotingDojo.Factory;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace quotingDojo.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        private readonly QuoteFactory quoteFactory;
        private readonly UserFactory userFactory;
        public HomeController(QuoteFactory quote, UserFactory user)
        {
            quoteFactory = quote;
            userFactory = user;
        }
        // GET: /Home/
        [HttpGet]
        [Route("/")]
        public IActionResult Index(string Message)
        {   ViewBag.Message = Message;
            return View("index");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string Email, string Password)
        {
            User logginginas = userFactory.FindByEmail(Email);
            if(logginginas == null)
            {
                return RedirectToAction("index", new {Message = "E-mail not found!"});
            }
            else if(logginginas.Password == Password){
                
                SessionExtensions.SetObjectAsJson(HttpContext.Session, "User", logginginas);
                ViewBag.firstname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").FirstName;
                ViewBag.lastname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").LastName;
                ViewBag.allQuotes = quoteFactory.FindAll();
                return View();
            }
            else{
                return RedirectToAction("index", new {Message = "Incorrect Password!"});
            }
        }
        [HttpPost]
        [Route("register")]
        public IActionResult register(User user1, string PasswordAgain)
        {
            Console.WriteLine(user1.Password + PasswordAgain);
            TryValidateModel(user1);
            if(ModelState.IsValid && user1.Password == PasswordAgain)
            {
                userFactory.Add(user1);
                return RedirectToAction("index", new {Message = "Login successful!"});
            }
            else
            {
                return RedirectToAction("index", new {Message = "Invalid arguments, you noob! \n Make sure both names contain 2 letters \n e-mail is valid, \n and passwords match!"});
            }
        }
        [HttpPost]
        [Route("postquote")]
        public IActionResult Post(Quote NewQuote)
        {
            if(ModelState.IsValid){
                Console.WriteLine(NewQuote.Text);
                int id = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").ID;
                Console.WriteLine(id);
                quoteFactory.Add(NewQuote, id);
            }
            ViewBag.firstname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").FirstName;
            ViewBag.lastname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").LastName;
            ViewBag.allQuotes = quoteFactory.FindAll();
            // foreach(var entry in ViewBag.allQuotes)
            // {
            //     Console.WriteLine(entry.Text);
            // }
            return View("Login");
        }
        [HttpPost]
        [Route("deletequote")]
        public IActionResult DeleteQuote(int toDelete)
        {
            Console.WriteLine(toDelete);
            quoteFactory.Delete(toDelete);
            ViewBag.firstname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").FirstName;
            ViewBag.lastname = SessionExtensions.GetObjectFromJson<User>(HttpContext.Session, "User").LastName;
            ViewBag.allQuotes = quoteFactory.FindAll();
            return View("Login");
        }
    }
}
