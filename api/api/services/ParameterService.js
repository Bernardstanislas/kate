/**
 * ParameterService
 *
 * @description :: Json input parameter checker
 */

module.exports = {
    /**
     * Check post parameters
     *
     * callback(err, parameters)
     */
    check: function(req, expectedParameters, allowedValues, callback) {
        var parameters = req.params.all();
        if (!parameters instanceof Object) return ErrorService.badFormat('parameters', 'object');

        var err = null;
        expectedParameters.every(function(parameterName) {
            if (!parameters.hasOwnProperty(parameterName)) {
                err = ErrorService.missingParameter(parameterName);
            } else {
                if (allowedValues.hasOwnProperty(parameterName)) {
                    if (allowedValues[parameterName].indexOf(parameters[parameterName]) == -1) {
                        err = ErrorService.badParameter(
                            parameterName, 
                            parameters[parameterName], 
                            allowedValues[parameterName]
                        );
                    }
                }
            }
            return (null == err);
        });
        
        return callback(err, parameters);
    },
};
