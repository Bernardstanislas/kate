/**
 * TokenAuthenticaion
 *
 * @module      :: Policy
 * @description :: Check token and turn
 *
 */
module.exports = function(req, res, next) {
    var response = null;

    ParameterService.check(req, ['id', 'token'], {}, function(err, parameters) {
        if (err) {
            response = res.json(err.httpCode, {error: err.message});
        } else {
            Game.findOneById(parameters.id).populate('vampireToken').populate('werewolfToken').exec(function(err, game) {
                if (err) {
                    var err = ErrorService.databaseError();
                    response = res.json(err.httpCode, {error: err.message});
                } else if (game) {
                    TokenService.check(game, parameters.token, function(err, game) {
                        if (err) {
                            response = res.json(err.httpCode, {error: err.message});
                        } else {
                            req.options.game = game;
                            return next();
                        }
                    });
                } else {
                    var err = ErrorService.gameNotFound();
                    response = res.json(err.httpCode, {error: err.message});
                }
            });
        }
    });

    return response;
};
