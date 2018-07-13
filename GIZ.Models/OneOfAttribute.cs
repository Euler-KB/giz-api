using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GIZ.Models
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class OneOfAttribute : ValidationAttribute
    {
        public List<object> Items { get; }

        public OneOfAttribute(params object[] items)
        {
            Items = new List<object>(items);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Invalid '{name}'. Must be one of the predefined values!";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            foreach (var item in Items)
            {
                if (item.Equals(value))
                    return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.MemberName));
        }

    }
}