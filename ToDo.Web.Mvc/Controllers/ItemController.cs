using Microsoft.AspNetCore.Mvc;
using ToDo.Domain.Entities;
using ToDo.Domain.Interface;
using ToDo.Web.Mvc.Models;

namespace ToDo.Web.Mvc.Controllers
{
    public class ItemController : Controller
    {
        protected IItemRepository repository;

        public ItemController(IItemRepository repository)
        {
            this.repository = repository;
        }
        public async Task<IActionResult> Index()
        {
            var items = await repository.GetAllAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Description")] CreateItemModel createItemModel)
        {
            if (ModelState.IsValid)
            {
                var item = new Item(createItemModel.Description);
                await repository.AddAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(createItemModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id is null)
                return NotFound();

            var item = await repository.GetAsync((Guid)id);

            if (item is null)
                return NotFound();

            ViewBag.Item = item;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id", "Description", "Done")] EditItemModel editItemModel)
        {
            if (id != editItemModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var item = new Item(editItemModel.Id, editItemModel.Done, editItemModel.Description);
                await repository.EditAsync(item);

                return RedirectToAction(nameof(Index));
            }
            return View(editItemModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id is null)
                return NotFound();

            var item = await repository.GetAsync((Guid)id);

            if (item is null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (ModelState.IsValid)
            {
                await repository.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
