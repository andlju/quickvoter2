var QuickVoter = QuickVoter | {};

QuickVoter.QuestionService = function($) {

    var getQuestions = function() {
        return $.ajax('/questions');
    };

    var getQuestion = function(questionId) {
        return $.ajax('/questions/' + questionId);
    };

    var addQuestion = function(text) {
        return $.ajax('/questions', {
            type: "POST",
            data: { text: text }
        });
    };

    var addAnswer = function(questionId, text) {
        return $.ajax('/questions/' + questionId + '/answers', {            
            type: "POST",
            data: { text: text }
        });
    };

    var addVote = function(questionId, answerId) {
        return $.ajax('/questions/' + questionId + '/answers/' + answerId, {
            type: "POST",
        });
    };
    
    return {        
        getQuestions: getQuestions,
        getQuestion: getQuestion,
        addQuestion: addQuestion,
        addAnswer: addAnswer,
        addVote: addVote
    };
}(jQuery);

