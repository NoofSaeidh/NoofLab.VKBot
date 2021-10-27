using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoofLab.VKBot.Core.Extensions.Validation
{
    public static class ValidationExtensions
    {
        public static void Validate(this object instance)
        {
            if (instance is null)
                throw new ValidationException("Instance is null.");

            Validator.ValidateObject(instance, new ValidationContext(instance));
        }

        public static void Validate<T>(this T instance)
        {
            if (instance is null)
                throw new ValidationException($"Instance of type {typeof(T)} is null.");

            Validate((object)instance);
        }

        public static T Validated<T>(this T instance)
        {
            Validate(instance);
            return instance;
        }
    }
}
