using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningSystem.AnswerHandlers.Reference
{
    /// <summary>
    /// Handles user input
    /// </summary>
    public interface IAnswerHandler
    {
        AnswerValidationResult ValidateInput(string input);

        string RenderInputHtml();       
    }
}
