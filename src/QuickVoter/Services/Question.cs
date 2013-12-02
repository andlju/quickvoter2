using System.Collections.Generic;

namespace QuickVoter.Services
{
    public class Question
    {
        public string QuestionId { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; } 
    }
}