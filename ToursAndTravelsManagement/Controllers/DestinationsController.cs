﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories.IRepositories;

namespace ToursAndTravelsManagement.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class DestinationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DestinationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Destinations
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";
            Log.Information("User {UserName} accessed the Destinations Index page", userName);

            int pageIndex = pageNumber ?? 1;
            int size = pageSize ?? 10;

            var destinations = await _unitOfWork.DestinationRepository.GetPaginatedAsync(pageIndex, size);
            return View(destinations);
        }

        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Destination Details with null ID", userName);
                return NotFound();
            }

            var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
            if (destination == null)
            {
                Log.Warning("User {UserName} tried to access Destination Details with invalid ID {DestinationId}", userName, id);
                return NotFound();
            }

            Log.Information("User {UserName} accessed details of Destination {DestinationId}", userName, id);
            return View(destination);
        }

        // GET: Destinations/Create
        public IActionResult Create()
        {
            var userName = User?.Identity?.Name ?? "Unknown User";
            Log.Information("User {UserName} is accessing the Destination Creation page", userName);
            return View();
        }

        // POST: Destinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Country,City,ImageUrl,CreatedBy,CreatedDate,IsActive")] Destination destination)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (ModelState.IsValid)
            {
                await _unitOfWork.DestinationRepository.AddAsync(destination);
                await _unitOfWork.CompleteAsync();

                Log.Information("User {UserName} created a new Destination: {@Destination}", userName, destination);
                return RedirectToAction(nameof(Index));
            }

            Log.Warning("User {UserName} attempted to create a destination with invalid data: {@Destination}", userName, destination);
            return View(destination);
        }

        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Destination Edit with null ID", userName);
                return NotFound();
            }

            var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
            if (destination == null)
            {
                Log.Warning("User {UserName} tried to access Destination Edit with invalid ID {DestinationId}", userName, id);
                return NotFound();
            }

            Log.Information("User {UserName} is editing Destination {DestinationId}", userName, id);
            return View(destination);
        }

        // POST: Destinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DestinationId,Name,Description,Country,City,ImageUrl,CreatedBy,CreatedDate,IsActive")] Destination destination)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id != destination.DestinationId)
            {
                Log.Warning("User {UserName} tried to edit a destination with mismatched ID {DestinationId}", userName, id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DestinationRepository.Update(destination);
                    await _unitOfWork.CompleteAsync();

                    Log.Information("User {UserName} successfully edited Destination {DestinationId}", userName, id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destination.DestinationId))
                    {
                        Log.Error("User {UserName} attempted to edit a non-existent Destination {DestinationId}", userName, id);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            Log.Warning("User {UserName} submitted invalid data for editing Destination {DestinationId}", userName, id);
            return View(destination);
        }

        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            if (id == null)
            {
                Log.Warning("User {UserName} tried to access Destination Delete with null ID", userName);
                return NotFound();
            }

            var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id.Value);
            if (destination == null)
            {
                Log.Warning("User {UserName} tried to access Destination Delete with invalid ID {DestinationId}", userName, id);
                return NotFound();
            }

            Log.Information("User {UserName} is deleting Destination {DestinationId}", userName, id);
            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userName = User?.Identity?.Name ?? "Unknown User";

            var destination = await _unitOfWork.DestinationRepository.GetByIdAsync(id);
            if (destination == null)
            {
                Log.Warning("User {UserName} tried to delete a non-existent Destination {DestinationId}", userName, id);
                return NotFound();
            }

            _unitOfWork.DestinationRepository.Remove(destination);
            await _unitOfWork.CompleteAsync();

            Log.Information("User {UserName} successfully deleted Destination {DestinationId}", userName, id);
            return RedirectToAction(nameof(Index));
        }

        private bool DestinationExists(int id)
        {
            return _unitOfWork.DestinationRepository.GetByIdAsync(id) != null;
        }
    }
}
