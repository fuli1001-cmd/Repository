using System;
using System.Collections.Generic;
using System.Text;

namespace Meal.Domain.Exceptions
{
    public class MealDomainException : Exception
    {
        public MealDomainException()
        { }

        public MealDomainException(string message)
            : base(message)
        { }

        public MealDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
