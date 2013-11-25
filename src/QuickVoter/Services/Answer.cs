namespace QuickVoter.Services
{
    public class Answer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string Text { get; set; }
        public int NumberOfVotes { get; set; }
    }
}