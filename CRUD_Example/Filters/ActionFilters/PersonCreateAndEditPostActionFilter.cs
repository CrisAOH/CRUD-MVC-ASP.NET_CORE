using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD_Example.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter (ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //BEFORE LOGIC
            if (context.Controller is PersonsController personsController) 
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp =>
                        new SelectListItem()
                        {
                            Text = temp.CountryName,
                            Value = temp.CountryID.ToString()
                        }
                    );

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest); //Short circuit o salta el siguiente filtro o action method
                }
                else
                {
                    await next(); //Llama al siguiente filtro o metodo
                }
            }
            else
            {
                await next();
            }
            //AFTER LOGIC
        }
    }
}
