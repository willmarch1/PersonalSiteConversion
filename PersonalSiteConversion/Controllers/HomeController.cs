using Microsoft.AspNetCore.Mvc;
using PersonalSiteConversion.Models;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;


namespace PersonalSiteConversion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            //!STEP 08 - Convert the Success message to a ViewBag
            if (TempData["Success"] != null)
            {

                ViewBag.Success = TempData["Success"].ToString();
            }

            //!STEP 09 - Convert the Failure message to a ViewBag
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //public IActionResult Contact()
        //{
        //    return View();
        //}

        //!STEP 03 - Copy/Paste the POST from class code. Make the changes noted inside. Comment out or delete the default Contact() above.
        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {

            if (!ModelState.IsValid)
            {
                //!STEP 04 - Set a TempData (like viewbag, but persists between different views/actions)
                TempData["ErrorMessage"] = "The Model was invalid";
                //!STEP 05 - Return a redirect with the #contact (or other appropriate) anchor.
                return RedirectToAction("Index", "Home", cvm, "contact");
            }
            string message = $"You have receieved a new email from your site's contact form.<br />Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}<br />";
            var mm = new MimeMessage();
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));
            mm.Subject = cvm.Subject;
            mm.Body = new TextPart("HTML") { Text = message };
            mm.Priority = MessagePriority.Urgent;
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config.GetValue<string>("Credentials:Email:Client"), 8889);
                    client.Authenticate(
                        _config.GetValue<string>("Credentials:Email:User"),
                        _config.GetValue<string>("Credentials:Email:Password")
                        );
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //!STEP 06 - Change the ViewBag error message to be a TempData and change the return line.
                    TempData["ErrorMessage"] = $"There was an issue processing your request. Please try again later." +
                                           $"<br /> Error message: {ex.Message}";
                    return RedirectToAction("Index", "Home", cvm, "contact");
                }


            }
            //!STEP 07 - Add a success message and change the return line.
            TempData["Success"] = $"<h3>Message Sent!</h3>" +
                                  $"<p>Thanks for contacting us! A member of our team will respond to your message " +
                                  $"using the provided email address, {cvm.Email}, in the next 1-3 business days.</p>";
            return RedirectToAction("Index", "Home", cvm, "contact");
        }
    }
}