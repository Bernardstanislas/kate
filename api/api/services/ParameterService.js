/**
 * ParameterService
 *
 * @description :: Json input parameter checker
 */

module.exports = {
    /**
     * Check parameters
     *
     * callback(err, parameters)
     */
    check: function(req, expectedParameters, allowedValues, callback) {
        var parameters = req.params.all();

        expectedParameters.forEach(function(parameterName) {
            if (!parameters.hasOwnProperty(parameterName)) {
                return callback(ErrorService.missingParameter(parameterName));
            } else {
                if (allowedValues.hasOwnProperty(parameterName)) {
                    if (allowedValues[parameterName].indexOf(parameters[parameterName]) == -1) {
                        return callback(ErrorService.badParameter(
                            parameterName, 
                            parameters[parameterName], 
                            allowedValues[parameterName]
                        ));
                    }

                }
            }
        });

        return callback(null, parameters);
    },
};
