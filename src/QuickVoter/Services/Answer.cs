namespace QuickVoter.Services
{
    public class Answer
    {
        public string QuestionId { get; set; }
        public long AnswerId { get; set; }
        public string Text { get; set; }
        public long NumberOfVotes { get; set; }
    }
}