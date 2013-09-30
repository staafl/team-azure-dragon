using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LearningSystem.Models
{
    public class StringNotContainsAttribute : ValidationAttribute
    {
        private string text;

        public StringNotContainsAttribute(string text)
        {
            this.text = text;
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            if (valueAsString == null)
            {
                return true;
            }

            return !valueAsString.Contains(text);
        }
    }
}
