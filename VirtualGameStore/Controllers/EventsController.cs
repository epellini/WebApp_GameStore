using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;
using VirtualGameStore.Services;

namespace VirtualGameStore.Controllers
{
    public class EventsController : Controller
    {
        public EventsController(IGameStoreManager gameStoreManager, SignInManager<User> signInManager, UserManager<User> userManager) 
        {
            _gameStoreManager = gameStoreManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET /events
        [HttpGet("/events")]
        public IActionResult ViewAllEvents()
        {
            List<Event>? events = _gameStoreManager.GetAllEvents();
            return View("AllEvents", events);
        }

        // GET /events/{id}
        [HttpGet("/events/{id}")]
        public async Task<IActionResult> ViewEvent(int id)
        {
            Event? eventEntity = _gameStoreManager.GetEventById(id);
            EventViewModel eventViewModel = new EventViewModel()
            {
                Event = eventEntity
            };
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    List<EventRegistration>? regs = _gameStoreManager.GetAllEventRegistrationsByUserId(user.Id);
                    if (regs != null)
                    {
                        eventViewModel.Registrations = regs;
                    }
                }
            }
            return View("Event", eventViewModel);  
        }

        // POST /events/{id}/register
        [HttpPost("/events/{id}/register")]
        public JsonResult RegisterForEvent(int id)
        {
            bool added = false;
            int count = 0;

            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    Event eventEntitiy = _gameStoreManager.GetEventById(id);
                    if (eventEntitiy != null)
                    {
                        user.EventRegistrations = _gameStoreManager.GetAllEventRegistrationsByUserId(user.Id);
                        bool alreadyReg = false;
                        if (user.EventRegistrations != null && user.EventRegistrations.Count > 0)
                        {
                            foreach (var eventRegistration in user.EventRegistrations)
                            {
                                if (eventRegistration.EventId == id)
                                {
                                    _gameStoreManager.DeleteEventRegistration(eventRegistration);
                                    alreadyReg = true;
                                    break;
                                }
                            }
                        }
                        if (!alreadyReg)
                        {
                            EventRegistration er = new EventRegistration()
                            {
                                EventId = eventEntitiy.EventId,
                                UserId = user.Id,
                            };

                            _gameStoreManager.CreateEventRegistration(er);
                        }
                        added = true;
                    }
                    count = eventEntitiy.EventRegistrations.Count();
                }
            }
            return Json(new { added = added, id = id, count = count });
        }

        // GET /events/add-event
        [HttpGet("/events/add-event")]
        public IActionResult AddEvent()
        {
            Event eventEntity = new Event();

            return View("AddEvent", eventEntity);
        }

        // GET /events/add-event
        [HttpGet("/events/{id}/edit")]
        public IActionResult EditEvent(int id)
        {
            Event eventEntity = _gameStoreManager.GetEventById(id);

            if (eventEntity != null)
            {
                return View("EditEvent", eventEntity);
            }
            else
            {
                ViewBag.errorMessage = "Event not found.";
            }
            return View("Error", "Account");

        }

        // POST /events/add-event
        [HttpPost("/events/add-event")]
        public IActionResult SaveEvent(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                _gameStoreManager.CreateEvent(eventEntity);
                return RedirectToAction("ViewAdminPanel", "Admin", new {tab = "Events"});
            }
            return View("AddEvent", eventEntity);
        }

        // POST /events/add-event
        [HttpPost("/events/{id}/edit")]
        public IActionResult UpdateEvent(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                _gameStoreManager.UpdateEvent(eventEntity);
                return RedirectToAction("ViewAdminPanel", "Admin", new { tab = "Events" });
            }
            return View("EditEvent", eventEntity);
        }

        // GET: /events/{id}/delete
        [HttpGet("events/{id}/delete")]
        public IActionResult DeleteEvent(int id)
        {
            Event eventEntity = _gameStoreManager.GetEventById(id);

            if (eventEntity != null)
            {
                _gameStoreManager.DeleteEvent(eventEntity);
                return RedirectToAction("ViewAdminPanel", "Admin", new {tab = "Events"});
            }
            else
            {
                ViewBag.errorMessage = "Game not found.";
            }
            return View("Error", "Account");
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
