using System;
using ServiceContracts.Enums;
using Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name no puede estar vacío.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email no puede estar vacío.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Seleccione su género")]
        public GenderOptions? Gender { get; set; }
        
        [Required(ErrorMessage ="Seleccione un país")]
        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Convierte el objeto actual PersonAddRequest en un nuevo objeto del tipo Person
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person()
            {
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
