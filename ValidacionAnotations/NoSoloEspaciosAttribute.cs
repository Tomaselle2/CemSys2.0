using System.ComponentModel.DataAnnotations;

namespace CemSys2.ValidacionAnotations
{
    public class NoSoloEspaciosAttribute : ValidationAttribute
    {
        public NoSoloEspaciosAttribute()
        {
            ErrorMessage = "El campo no puede estar vacío, tener solo espacios o contener espacios al inicio o al final.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (str != str.Trim())
                {
                    return new ValidationResult("El campo no debe tener espacios al inicio o al final.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
