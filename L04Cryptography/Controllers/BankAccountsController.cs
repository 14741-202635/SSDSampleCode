
using L04Cryptography.Data;
using L04Cryptography.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class BankAccountsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public BankAccountsController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // GET: BANKACCOUNTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.BankAccount.ToListAsync());
    }

    // GET: BANKACCOUNTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccount = await _context.BankAccount
            .FirstOrDefaultAsync(m => m.Id == id);
        if (bankaccount == null)
        {
            return NotFound();
        }

        return View(bankaccount);
    }

    // GET: BANKACCOUNTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BANKACCOUNTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Number")] BankAccount bankaccount)
    {
        if (ModelState.IsValid)
        {
            bankaccount = EncryptAndDecrypt(bankaccount);
            _context.Add(bankaccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(bankaccount);
    }

    private BankAccount EncryptAndDecrypt(BankAccount bankAccount)
    {
        Aes aes = AesInitialize(); //Aes.Create();

        //encrypt bank account number
        ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] input = Encoding.UTF8.GetBytes(bankAccount.Number);
        byte[] output = encryptor.TransformFinalBlock(input, 0, input.Length);
        bankAccount.EncryptedNumber = Convert.ToBase64String(output);

        //decrypt bank account number
        ICryptoTransform decryptor = aes.CreateDecryptor();
        input = Convert.FromBase64String(bankAccount.EncryptedNumber);
        output = decryptor.TransformFinalBlock(input, 0, input.Length);
        bankAccount.DecryptedNumber = Encoding.UTF8.GetString(output);

        return bankAccount;
    }
    private Aes AesInitialize()
    {
        Aes aes = Aes.Create();

        // retrieve key & IV from secrets in our configuration
        var aesSettings = _config.GetSection("AES").Get<AESSettings>();

        if (aesSettings == null)
        {
            string key = null, IV = null; // prepare Configuration strings for Key and IV
            foreach (byte b in aes.Key)
                key += $"{b},";
            key = key.Trim(',');

            foreach (byte b in aes.IV)
                IV += $"{b},";
            IV = IV.Trim(',');
        }
        else
        {
            aes.Key = aesSettings.Key;
            aes.IV = aesSettings.IV;
        }

        return aes;
    }

    // GET: BANKACCOUNTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccount = await _context.BankAccount.FindAsync(id);
        if (bankaccount == null)
        {
            return NotFound();
        }
        return View(bankaccount);
    }

    // POST: BANKACCOUNTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Number,EncryptedNumber,DecryptedNumber")] BankAccount bankaccount)
    {
        if (id != bankaccount.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(bankaccount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(bankaccount.Id))
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
        return View(bankaccount);
    }

    // GET: BANKACCOUNTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bankaccount = await _context.BankAccount
            .FirstOrDefaultAsync(m => m.Id == id);
        if (bankaccount == null)
        {
            return NotFound();
        }

        return View(bankaccount);
    }

    // POST: BANKACCOUNTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var bankaccount = await _context.BankAccount.FindAsync(id);
        if (bankaccount != null)
        {
            _context.BankAccount.Remove(bankaccount);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BankAccountExists(int? id)
    {
        return _context.BankAccount.Any(e => e.Id == id);
    }
}
