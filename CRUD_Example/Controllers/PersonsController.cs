﻿using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUD_Example.Controllers
{
    [Route("[controller]")] //Se añade como prefijo a todas las rutas
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { 
                    nameof(PersonResponse.PersonName), 
                    "Person Name" 
                },

                {
                    nameof(PersonResponse.Email),
                    "Email"
                },

                {
                    nameof(PersonResponse.DateOfBirth),
                    "Date Of Birth"
                },

                {
                    nameof(PersonResponse.Gender),
                    "Gender"
                },

                {
                    nameof(PersonResponse.Country),
                    "Country"
                },

                {
                    nameof(PersonResponse.Address),
                    "Address"
                },
            };
            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;
                
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries;

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            PersonResponse personPerson = _personsService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }
    }
}
