﻿using CommonFiles.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.Models;
using reCaptcha;
using System.Configuration;
using System.Diagnostics;
using System.Web.Helpers;
using InfrastructureInterface;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace TestApp.Controllers
{
    public class CVController : Controller
    {
        private readonly ICVService _service;

        public CVController(ICVService service)
        {
            _service = service;
        }

        // GET: CV
        public ActionResult Index()
        {
            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult List()
        {
            IEnumerable<CVModel> dtos = _service.GetAll().Select(element => Convert(element));
            return View(dtos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckModel(CVModel model)
        {
            model.PictureBytes = new byte[model.Picture.ContentLength];
            model.Picture.InputStream.Read(model.PictureBytes, 0, model.Picture.ContentLength);

            if (ModelState.IsValid)
            {
                _service.Insert(Convert(model));
                return View(model);
            }
            else
            {
                TempData["CVModel"] = model;
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Recaptcha = ReCaptcha.GetHtml(ConfigurationManager.AppSettings["ReCaptcha:SiteKey"]);
            ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            CVModel model = (CVModel)TempData["CVModel"] ?? new CVModel();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(Convert(_service.Get(id)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CVModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Picture != null)
                {
                    _service.Edit(Convert(model));
                }
                else
                {
                    CVDTO dto = _service.Get(model.ID);
                    dto.Address = model.Address;
                    dto.Education = model.Education;
                    dto.Email = model.Email;
                    dto.Experience = model.Experience;
                    dto.Qualities = model.Qualities;
                    _service.Edit(dto);
                }
                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            _service.Delete(id);
            return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            CVModel model = Convert(_service.Get(id));

            Image img;
            using (MemoryStream stream = new MemoryStream(model.PictureBytes))
            { img = new Bitmap(stream); }

            Image pic = Resize(img, 400, 400);
            using (MemoryStream ms = new MemoryStream())
            {
                pic.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                model.PictureBytes = ms.GetBuffer();
            }

            return View(model);
        }

        private Image Resize(Image img, int resizedW, int resizedH)
        {
            Bitmap bmp = new Bitmap(resizedW, resizedH);
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            graphic.Dispose();
            return bmp;
        }

        private CVModel Convert(CVDTO dto) => new CVModel
        {
            Address = dto.Address,
            Education = dto.Education,
            ID = dto.ID,
            Email = dto.Email,
            Experience = dto.Experience,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Qualities = dto.Qualities,
            PictureBytes = dto.PictureBytes,
            PictureName = dto.PictureName
        };
        private CVDTO Convert(CVModel model) => new CVDTO
        {
            Address = model.Address,
            Education = model.Education,
            ID = model.ID,
            Email = model.Email,
            Experience = model.Experience,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Qualities = model.Qualities,
            PictureBytes = model.PictureBytes,
            PictureName = model.Picture.FileName
        };
    }
}