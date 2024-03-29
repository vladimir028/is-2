using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                var allPurchases = new List<Order>();
                ViewBag.Purchases = allPurchases;
                ViewBag.TotalSum = 0;
            }
            else
            {
                Guid userIdGuid = new Guid(userId);
                ViewBag.User = userIdGuid;
                var order = _context.Order
              .Include(i => i.TicketsOrder)
              .Include("TicketsOrder.Ticket")
              .Include("TicketsOrder.Ticket.Movie")
              .FirstOrDefault(x => x.UserId == userIdGuid);

                if(order != null)
                {
                    var allPurchases = order.TicketsOrder.ToList();
                    //allPurchases.Select(i => )
                    ViewBag.Purchases = allPurchases;
                    ViewBag.TotalSum = allPurchases.Select(i => i.Ticket.Price).Sum();
                }
                else
                {
                    var allPurchases = new List<Order>();
                    //allPurchases.Select(i => i.UserId)
                    ViewBag.Purchases = allPurchases;
                    ViewBag.TotalSum = 0;
                }
            }
          
            return View();
            
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userIdGuid = new Guid(userId);

            var order = _context.Order.FirstOrDefault(i => i.UserId == userIdGuid);
            if (order != null)
            {
                _context.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketsOrder = _context.TicketsOrder.FirstOrDefault(t => t.Id == id);
            if (ticketsOrder == null)
            {
                return NotFound();
            }
            _context.TicketsOrder.Remove(ticketsOrder);
            _context.SaveChanges();

           
            return RedirectToAction("Index");
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(Guid id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        //id -> ticketId
        [Authorize]
        public IActionResult CreateOrder(Guid id) 
        {
            if(id == null)
            {
                return NotFound();
            }

            var ticket = _context.Tickets.FirstOrDefault(i => i.Id == id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid userIdGuid = new Guid(userId);
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            var order = _context.Order.Include(i => i.TicketsOrder).FirstOrDefault(i => i.UserId == userIdGuid);
            if(order == null)
            {
                order = new Order();
                order.Id = new Guid();
                order.UserId = userIdGuid;
            }
          
            TicketsOrder ticketsOrder = new TicketsOrder();
            if (ticket != null && userId != null)
            {
                
                
                    if (order.TicketsOrder == null)
                    {
                        order.TicketsOrder = new List<TicketsOrder>();
                    }

                    ticketsOrder.Order = order;
                    ticketsOrder.OrderId = order.Id;
                    ticketsOrder.Ticket = ticket;
                    ticketsOrder.TicketId = ticket.Id;
                    ticketsOrder.Id = new Guid();
                    order.TicketsOrder.Add(ticketsOrder);
                }

            if (order == null)
            {
                _context.Add(order);
            }
            _context.Add(ticketsOrder);
            _context.SaveChanges();

            var a = ticket;
            return RedirectToAction("Index");
        }
    }
}
