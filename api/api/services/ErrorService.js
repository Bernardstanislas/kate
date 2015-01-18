/**
 * ErrorService
 *
 * @description :: Send http 400 if the required parameters are not found
 */

module.exports = {
    /**
     * Generic error
     */
    genericError: function(httpCode, errorMessage) {
        console.log('Error ' + httpCode + ' (' + errorMessage + ')')
        var error = new Error(errorMessage);
        error.httpCode = httpCode;

        return error;
    },
    /**
     * Database error
     */
    databaseError: function() {
        return module.exports.genericError(500, 'Database error');
    },
    /**
     * Bad database error (non-atomic partial update)
     */
    badDatabaseError: function() {
        return module.exports.genericError(500, 'Bad database error');
    },
    /**
     * Server error
     */
    serverError: function() {
        return module.exports.genericError(500, 'Server error');
    },
    /**
     * Missing parameter
     */
    missingParameter: function(parameter) {
        return module.exports.genericError(400, 'Missing ' + parameter + ' parameter');
    },
    /**
     * Bad parameter value
     */
    badParameter: function(parameter, value, allowedValues) {
        return module.exports.genericError(400, 
            'Bad ' + parameter + ' parameter value: ' + value + '. Allowed values: ' + allowedValues.join(', ')
        );
    },
    /**
     * Game not found
     */
    gameNotFound: function() {
        return module.exports.genericError(404, 'Game not found');
    },
    /**
     * Team already exists
     */
    teamAlreadyExists: function(team) {
        return module.exports.genericError(409, 'Team ' + team + ' already registered');
    },
    /**
     * Missing team
     */
    missingTeam: function(team) {
        return module.exports.genericError(400, 'Team ' + team + ' is missing');
    },
    /**
     * Map parse error
     */
    mapError: function(message) {
        return module.exports.genericError(400, 'Map error: ' + message);
    },
    /**
     * Move parse error
     */
    moveError: function(message) {
        return module.exports.genericError(400, 'Move error: ' + message);
    },
    /**
     * Invalid token
     */
    invalidToken: function() {
        return module.exports.genericError(400, 'Invalid token');
    },
    /**
     * Not your turn
     */
    notYourTurn: function() {
        return module.exports.genericError(400, 'Not your turn');
    },
    /**
     * Game done
     */
    gameDone: function() {
        return module.exports.genericError(400, 'Game is done');
    },
    /**
     * Integrity error
     */
    integrityError: function(x, y) {
        return module.exports.genericError(500, 'Integrity error on tile [' + x + ', ' + y + ']');
    },
};
