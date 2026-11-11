
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using L05Logging.Models;
using L05Logging.Data;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ApplicationDbContext context, ILogger<CarsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: CARS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Car.ToListAsync());
    }

    // GET: CARS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // GET: CARS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CARS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Make,Model,Year,Color,Price")] Car car)
    {
        if (ModelState.IsValid)
        {
            _context.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(car);
    }

    // GET: CARS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }
        return View(car);
    }

    // POST: CARS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Make,Model,Year,Color,Price")] Car car)
    {
        if (id != car.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(car.Id))
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
        return View(car);
    }

    // GET: CARS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Car
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // POST: CARS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var car = await _context.Car.FindAsync(id);
        if (car != null)
        {
            _context.Car.Remove(car);
        }

        await _context.SaveChangesAsync();

        string logMsg = $"User {User.Identity.Name} car delete confirmed for id : {id}";
        _logger.LogInformation(logMsg);

        return RedirectToAction(nameof(Index));
    }

    private bool CarExists(int? id)
    {
        return _context.Car.Any(e => e.Id == id);
    }
}
