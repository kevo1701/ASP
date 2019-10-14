using CommonFiles.DTO;
using InfrastructureInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TestApp.Models;
namespace TestApp.Controllers
{
    public class BoxController : Controller
    {
        private readonly IBoxService _service;

        public BoxController(IBoxService service)
        {
            _service = service;
        }
        

        public ActionResult Index()
        {
            return View("Box");
        }
        [HttpGet]
        //Create a new Box Model if the previous session had created one and show page for Box Creation
        public ActionResult Create()
        {
            BoxModel viewModel = (BoxModel)TempData["viewModel"] ?? new BoxModel();

            return View(viewModel);
        }

        [HttpGet]
        //List all Boxes
        public ActionResult List()
        {
            List<BoxModel> boxes = new List<BoxModel>();
            foreach (BoxDTO dto in _service.GetAll())
            {
                boxes.Add(Convert(dto));
            }
            return View(boxes);
        }

        [HttpPost]
        //Output information of created box
        public ActionResult RequestBox(BoxModel viewModel)
        {
            //Show a view consisting of the fields of the box
            if (ModelState.IsValid)
            {
                string message = string.Format("The request for box with with size {0}:{1}:{2} and weight {3}, color {4} and material {5} was successfully accepted."
                , viewModel.Width, viewModel.Height, viewModel.Length, viewModel.Weight, viewModel.Colour, viewModel.Material);

                //Create DTO
                BoxDTO box = new BoxDTO();
                //Set Properties of DTO
                foreach (PropertyInfo property in box.GetType().GetProperties())
                {
                    property.SetValue(box, viewModel.GetType().GetProperty(property.Name).GetValue(viewModel));
                }
                _service.Insert(box);

                ViewBag.SuccessMessage = message;
                return View();

            }
            //Go back to creating a box, because something was not valid
            else
            {
                TempData["viewModel"] = viewModel;
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        //Delete a box with the specified id
        public ActionResult Delete(int id)
        {
            _service.Delete(id);//Find box if there exists one
            return RedirectToAction("List");
        }

        [HttpGet]
        //Edit information of a specified by id box
        public ActionResult Edit(int id)
        {
            BoxModel model = Convert(_service.Get(id));//Find box if there exists one
            return View(model);
        }

        [HttpPost]
        //Edit box with a specified BoxModel
        public ActionResult Edit(BoxModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BoxDTO @new = new BoxDTO();
                foreach (PropertyInfo property in @new.GetType().GetProperties())
                {
                    property.SetValue(@new, viewModel.GetType().GetProperty(property.Name).GetValue(viewModel));
                }

                _service.Edit(@new);

                return RedirectToAction("List");
            }
            else
                return RedirectToAction("Edit", new { id = viewModel.ID });
        }

        //DTO to Model Converter
        private BoxModel Convert(BoxDTO dto)
        => new BoxModel
        {
            Colour = dto.Colour,
            Height = dto.Height,
            ID = dto.ID,
            Length = dto.Length,
            Material = dto.Material,
            Weight = dto.Weight,
            Width = dto.Width
        };
    }
}