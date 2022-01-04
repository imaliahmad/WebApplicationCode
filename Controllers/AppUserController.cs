using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AppUserController : Controller
    {
        private MyDbContext context = new MyDbContext();
        public IActionResult Index()
        {
            return View(context.AppUser);
        }

        [HttpGet]
        public IActionResult CreateOrEdit(int id)
        {
            AppUser obj = new AppUser();

            if (id > 0) //edit
            {
                obj = context.AppUser.Find(id);
            }

            //Dropdown List --> City List
            obj.CityList = new SelectList(context.Cities.ToList(), "CityId", "Name");
            //ViewBag.CityList = list;


            //Radio Buttons --> Gender
            obj.GenderList = new SelectList(context.Gender.ToList(), "GenderId", "Name");

            //CheckBox --> Interest List
            //obj.InterestList = new SelectList(context.Interest.ToList(), "InterestId", "Name");

            var interestList = context.Interest.ToList();
            var userInterestList = context.UserInterest.Where(x => x.UserId == id).ToList();


            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in context.Interest.ToList())
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name.ToString(),
                    Value = item.InterestId.ToString(),
                    Selected = userInterestList.Where(x => x.InterestId == item.InterestId).Count() > 0 ? true : false
                });
            }

           


            

            //foreach (var item in interestList)
            //{
            //    //SelectListItem element = new SelectListItem();
            //    //element.Text = item.Name.ToString();
            //    //element.Value = item.InterestId.ToString();

            //    //SelectListItem element = new SelectListItem() { Text = item.Name.ToString(), Value = item.InterestId.ToString()};

            //}

           

            obj.InterestList = list;

            return View(obj);
        }
        [HttpPost]
        public IActionResult CreateOrEdit(AppUser model)
        {
            if (ModelState.IsValid)
            {
                model.InterestList = model.InterestList.Where(x => x.Selected == true).ToList();
                if (model.UserId > 0) //Edit
                {
                    //update user table
                    context.AppUser.Update(model);
                    context.SaveChanges();

                    //remove userInterest existing records
                    var range = context.UserInterest.Where(x => x.UserId == model.UserId).ToList();
                    context.UserInterest.RemoveRange(range);
                    context.SaveChanges();

                    //insert userInterest new records
                    List<UserInterest> userInterests = new List<UserInterest>();
                    model.InterestList.ForEach(x => { userInterests.Add(new UserInterest() { InterestId = Convert.ToInt32(x.Value), UserId = model.UserId }); });

                    context.UserInterest.AddRange(userInterests);
                    context.SaveChanges();
                }
                else
                {
                    context.AppUser.Add(model);
                    context.SaveChanges();

                    List<UserInterest> userInterests = new List<UserInterest>();
                    model.InterestList.ForEach(x => {userInterests.Add(new UserInterest() { InterestId = Convert.ToInt32(x.Value), UserId = model.UserId });});

                    context.UserInterest.AddRange(userInterests);
                    context.SaveChanges();

                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }
    }
}
