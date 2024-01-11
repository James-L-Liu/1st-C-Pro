using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using A2.Models;
using A2.Dtos;
using System.Text.Json;
using System.Net;
using A2.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Globalization;

namespace A2.Controllers
{
    [Route("webapi")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repo;
        public A2Controller(IA2Repo repo)
        {
            _repo = repo;
        }

        [HttpGet("GetVersion")]
        public ActionResult<string> GetVersion()
        {
            Console.WriteLine("Yes, it is processing to here already and nothing should be a trouble !!!");
            return Ok("1.0.0 (Ngongotahā) by James Liu");
        }


        [HttpGet("Logo")]
        public ActionResult Logo()
        {
            string path = Directory.GetCurrentDirectory();
            string logoDir = Path.Combine(path, "Logos");
            string fileName = Path.Combine(logoDir, "Logo.png");
            return PhysicalFile(fileName, "image/png");
        }

        [HttpGet("AllItems")]
        public ActionResult<IEnumerable<Product>> AllItems()
        {
            IEnumerable<Product> items = _repo.GetProducts();
            return Ok(items);
        }

        [HttpGet("Items/{term}")]
        public ActionResult<IEnumerable<Product>> Items(string term)
        {
            IEnumerable<Product> items = _repo.GetItemsByName(term);
            return Ok(items);
        }

        [HttpGet("ItemImage/{id}")]
        public ActionResult ItemImage(string id)
        {

            string path = Directory.GetCurrentDirectory();  // would this be working on idepedent machines?
            string imgDir = Path.Combine(path, "ItemsImages");
            string filename2 = Path.Combine(imgDir, id + ".jpg");
            string filename5 = Path.Combine(imgDir, id + ".jpeg");
            string filename1 = Path.Combine(imgDir, id + ".png");
            string filename3 = Path.Combine(imgDir, id + ".gif");
            string filename4 = Path.Combine(imgDir, id + ".svg");
            string respHeader = "";
            string fileName = "";

            if (System.IO.File.Exists(filename2))
            {
                respHeader = "image/jpeg";
                fileName = filename2;
            }
            else if (System.IO.File.Exists(filename5))
            {
                respHeader = "image/jpeg";
                fileName = filename5;
            }
            else if (System.IO.File.Exists(filename1))
            {
                respHeader = "image/png";
                fileName = filename1;
            }
            else if (System.IO.File.Exists(filename3))
            {
                respHeader = "image/gif";
                fileName = filename3;
            }
            else if (System.IO.File.Exists(filename4))
            {
                respHeader = "image/svg";
                fileName = filename4;
            }
            else
            {
                respHeader = "image/png";
                fileName = Path.Combine(imgDir, "default.png");
            }

            return PhysicalFile(fileName, respHeader);
        }



        [HttpGet("GetComment/{id}")]
        public ActionResult<Comment> GetComment(int id)
        {
            Comment comment = _repo.getCommentById(id);
            if (comment == null)
            {
                return BadRequest(String.Format("Comment {0} does not exist.", id));
            }
            return Ok(comment);
        }

        [HttpPost("WriteComment")]
        public ActionResult<Comment> WriteComment(CommentInput comment)
        {

            Comment commentNew = new Comment
            {
                Name = comment.Name,
                UserComment = comment.UserComment,
                Time = DateTime.Now.ToString("yyyyMMddTHHmmssZ"),
                IP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
            };
            Comment addedComment = _repo.AddComment(commentNew);
            return CreatedAtAction(nameof(GetComment), new { id = addedComment.Id }, addedComment);
        }

        [HttpGet("Comments/{num}")]
        public ActionResult<IEnumerable<Comment>> Comments(int num = 5)
        {
            IEnumerable<Comment> c = _repo.getAllComments(num);
            return Ok(c);
        }










        [HttpPost("Register")]
        public ActionResult<string> Register(User NewUser)
        {
            User user = new User { UserName = NewUser.UserName, Password = NewUser.Password, Address = NewUser.Address };
            User u = _repo.AddNewUser(user);
            if (u == null)
            {
                return Ok(String.Format("UserName {0} is not available.", NewUser.UserName));
            }
            else
            {
                return Ok("User successfully registered.");
            }
        }

        [Authorize(AuthenticationSchemes="UserAuthentication")]
        [Authorize(Policy= "AuthOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public ActionResult<PurchaseOutput> PurchaseItem(int id)
        {

            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("normalUser");
            if (c == null)
                return Forbid();
            else
            {
                Product p = _repo.getProductById(id);
                if (p == null)
                    return BadRequest(String.Format("Product {0} not found", id));
                else
                {
                    string nameOfUser = c.Value;
                    PurchaseOutput result = new PurchaseOutput { UserName = nameOfUser, ProductID = id };

                    return Ok(result);
                }
            }
        }

        [Authorize(AuthenticationSchemes = "UserAuthentication")]
        [Authorize(Policy = "AuthOnly")]
        [HttpPost("AddEvent")]
        public ActionResult<string> AddEvent(EventInput NewEvent)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("Organizor");
            if (c == null)
                return Forbid();
            else
            {
                DateTime result;
                if (!DateTime.TryParseExact(NewEvent.Start, "yyyyMMddTHHmmssZ",
                    null, DateTimeStyles.None, out result)
                    &&
                    !DateTime.TryParseExact(NewEvent.End, "yyyyMMddTHHmmssZ",
                    null, DateTimeStyles.None, out result))
                { 
                    var json = new
                    {
                        message = "Bad request",
                        errorCode = 400,
                        detail = "The format of Start and End should be yyyyMMddTHHmmssZ."
                    };
                    //var options = new JsonSerializerOptions { WriteIndented = true };
                    //string j = JsonSerializer.Serialize(json, options);
                    return BadRequest(json);
                }

                else if (!DateTime.TryParseExact(NewEvent.Start, "yyyyMMddTHHmmssZ",
                    null, DateTimeStyles.None, out result))
                {
                    var json = new
                    {
                        message = "Bad request",
                        errorCode = 400,
                        detail = "The format of Start should be yyyyMMddTHHmmssZ."
                    };
                    return BadRequest(json);
                }

                else if (!DateTime.TryParseExact(NewEvent.End, "yyyyMMddTHHmmssZ",
                    null, DateTimeStyles.None, out result))
                {
                    var json = new
                    {
                        message = "Bad request",
                        errorCode = 400,
                        detail = "The format of End should be yyyyMMddTHHmmssZ."
                    };

                    return BadRequest(json);
                }

                Event e = new Event { Start = NewEvent.Start, End = NewEvent.End,
                    Description = NewEvent.Description, Summary = NewEvent.Summary,
                    Location = NewEvent.Location};

                _repo.addEvent(e);
                return Ok("Success");
            }

        }

        [Authorize(AuthenticationSchemes = "UserAuthentication")]
        [Authorize(Policy = "AuthOnly")]
        [HttpGet("EventCount")]
        public ActionResult<int> EventCount()
        {
            int result = _repo.countEvent();
            return result;
        }

        [Authorize(AuthenticationSchemes = "UserAuthentication")]
        [Authorize(Policy = "AuthOnly")]
        [HttpGet("Event/{id}")]
        public ActionResult getEventInCalendarFormat(int id)
        {
            Event e = _repo.getEvent(id);
            if (e == null)
                return BadRequest(String.Format("Event {0} does not exist.", id));
            Response.Headers.Add("Content-Type", "text/calendar");
            return Ok(e);
        }

    }
}