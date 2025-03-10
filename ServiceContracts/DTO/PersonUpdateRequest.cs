using System;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    //SE CONSERVAN LAS PROPIEDADES QUE SE DESEEN ACTUALIZAR
    /// <summary>
    /// Represents the DTO class that cointains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "No puede dejar vacío este campo")]
        public Guid PersonID { get; set; }

        [Required(ErrorMessage = "Person Name no puede estar vacío.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email no puede estar vacío.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public GenderOptions? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Convierte el objeto actual PersonUpdateRequest en un nuevo objeto del tipo Person
        /// </summary>
        /// <returns>Returns Person object</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
