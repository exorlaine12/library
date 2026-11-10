using library2.Models;
using library2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace library2.Controllers;
public class LoansController : Controller
{
    private readonly LoanRepository _loans;
    private readonly MemberRepository _members;
    private readonly BookRepository _books;

    public LoansController(
        LoanRepository loans,
        MemberRepository members,
        BookRepository books)
    {
        _loans = loans;
        _members = members;
        _books = books;
    }

    // GET /Loans
    // Displays every loan, newest first.
    public async Task<IActionResult> Index()
    {
        var loans = await _loans.GetAllAsync();

        return View(loans);
    }

    // Fills the Borrow form dropdowns.
    private async Task FillDropdownsAsync()
    {
        ViewBag.Members = new SelectList(
            await _members.GetAllAsync(),
            "MemberId",
            "FullName");

        ViewBag.Books = new SelectList(
            await _books.GetAllAsync(),
            "BookId",
            "Title");
    }

    // GET /Loans/Borrow
    // Displays the empty Borrow form.
    public async Task<IActionResult> Borrow()
    {
        await FillDropdownsAsync();

        return View(new BorrowForm());
    }

    // POST /Loans/Borrow
    // Saves a new loan if validation succeeds.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowForm form)
    {
        if (ModelState.IsValid)
        {
            // Check whether the member can borrow another book.
            bool saved = await _loans.BorrowAsync(
                form.MemberId!.Value,
                form.BookId!.Value,
                form.DaysAllowed);

            if (saved)
            {
                TempData["Success"] = "Book successfully borrowed.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                $"This member already has {LoanRepository.MaxOpenLoans} books out. Nothing was saved.");
        }

        // Reload dropdowns if validation fails.
        await FillDropdownsAsync();

        return View(form);
    }

    // POST /Loans/Return/5
    // Returns a borrowed book.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(long id)
    {
        // Verify that the loan exists.
        var loans = await _loans.GetAllAsync();

        var loanExists = loans.Any(l => l.LoanId == id);

        if (!loanExists)
        {
            TempData["Error"] = "Loan record not found.";

            return RedirectToAction(nameof(Index));
        }

        // Return the book.
        await _loans.ReturnAsync(id);

        TempData["Success"] = "Book successfully returned.";

        return RedirectToAction(nameof(Index));
    }
}
