
using L04Cryptography.Data;
using L04Cryptography.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BankAccountDataController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDataProtector _protector;
    private readonly IDataProtector _protector2;

    public BankAccountDataController(ApplicationDbContext context, IDataProtectionProvider provider)
    {
        _context = context;
        _protector = provider.CreateProtector("BankAccounts");
        _protector2 = provider.CreateProtector("BankAccounts");
    }

    // GET: BANKACCOUNTDATAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.BankAccountData.ToListAsync());
    }

    // GET: BANKACCOUNTDATAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccountdata = await _context.BankAccountData
            .FirstOrDefaultAsync(m => m.Id == id);
        if (bankaccountdata == null)
        {
            return NotFound();
        }

        return View(bankaccountdata);
    }

    // GET: BANKACCOUNTDATAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BANKACCOUNTDATAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Number")] BankAccountData bankaccountdata)
    {
        if (ModelState.IsValid)
        {
            bankaccountdata.EncryptedNumber = _protector.Protect(bankaccountdata.Number);
            bankaccountdata.DecryptedNumber = _protector2.Unprotect(bankaccountdata.EncryptedNumber);
            _context.Add(bankaccountdata);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(bankaccountdata);
    }

    // GET: BANKACCOUNTDATAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccountdata = await _context.BankAccountData.FindAsync(id);
        if (bankaccountdata == null)
        {
            return NotFound();
        }
        return View(bankaccountdata);
    }

    // POST: BANKACCOUNTDATAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Number,EncryptedNumber,DecryptedNumber")] BankAccountData bankaccountdata)
    {
        if (id != bankaccountdata.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(bankaccountdata);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountDataExists(bankaccountdata.Id))
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
        return View(bankaccountdata);
    }

    // GET: BANKACCOUNTDATAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccountdata = await _context.BankAccountData
            .FirstOrDefaultAsync(m => m.Id == id);
        if (bankaccountdata == null)
        {
            return NotFound();
        }

        return View(bankaccountdata);
    }

    // POST: BANKACCOUNTDATAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var bankaccountdata = await _context.BankAccountData.FindAsync(id);
        if (bankaccountdata != null)
        {
            _context.BankAccountData.Remove(bankaccountdata);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BankAccountDataExists(int? id)
    {
        return _context.BankAccountData.Any(e => e.Id == id);
    }
}
