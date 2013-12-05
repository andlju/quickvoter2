using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using QuickVoter.Modules;

namespace QuickVoter.Services
{
    public interface IQuestionService
    {
        Question AddQuestion(string text);
        Question GetQuestion(string questionId);
        IEnumerable<Question> GetQuestions();
        Answer AddAnswer(string questionId, string text);
        Answer AddVote(string questionId, long answerId);
        void DeleteAll();
    }
}