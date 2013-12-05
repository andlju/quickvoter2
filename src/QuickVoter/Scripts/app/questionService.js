window.QuestionService = window.QuestionService | {};

window.QuestionService = function($) {

    var getQuestions = function() {
        return $.ajax('/api/questions');
    };

    var getQuestion = function(questionId) {
        return $.ajax('/api/questions/' + questionId);
    };

    var addQuestion = function(text) {
        return $.ajax('/api/questions', {
            type: "POST",
            data: { text: text }
        });
    };

    var addAnswer = function(questionId, text) {
        return $.ajax('/api/questions/' + questionId + '/answers', {            
            type: "POST",
            data: { text: text }
        });
    };

    var addVote = function(questionId, answerId) {
        return $.ajax('/api/questions/' + questionId + '/answers/' + answerId, {
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

