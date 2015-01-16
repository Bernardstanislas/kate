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
            'Bad ' + parameter + ' parameter value: ' + value + '. Allowed values: ' + allowedValues.toString()
        );
    },
    /**
     * Game already exists
     */
    gameAlreadyExists: function() {
        return module.exports.genericError(409, 'Game already exists');
    },
    /**
     * Team already exists
     */
    teamAlreadyExists: function(alignement) {
        return module.exports.genericError(409, 'Team ' + alignement + ' already registered');
    },
};
