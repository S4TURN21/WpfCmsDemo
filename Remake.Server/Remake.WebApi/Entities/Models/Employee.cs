using System.ComponentModel.DataAnnotations;

namespace Remake.WebApi.Entities.Models
{
    public class Employee
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Фото
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// Фамилия сотрудника
        /// </summary>
        [Required]
        public string? LastName { get; set; }

        /// <summary>
        /// Имя сотрудника
        /// </summary>
        [Required]
        public string? FirstName { get; set; }

        /// <summary>
        /// Отчество сотрудника
        /// </summary>
        [Required]
        public string? MiddleName { get; set; }
    }
}
