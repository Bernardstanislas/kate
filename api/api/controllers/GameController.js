/**
 * GameController
 *
 * @description :: Server-side logic for managing game
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {
    /**
     * Game state
     */
    state: function(req, res) {
        Game.findOneById(req.param('id')).populate('tiles').populate('werewolfToken').populate('vampireToken').exec(function(err, game) {
            if (err) {
                var err = ErrorService.databaseError();
                res.json(err.httpCode, {error: err.message});
            } else if (game) {
                res.json(200, game.state());
            } else {
                var err = ErrorService.gameNotFound();
                res.json(err.httpCode, {error: err.message});
            }
        });
    },
    /**
     * Game properties
     */
    properties: function(req, res) {
        Game.findOneById(req.param('id')).populate('werewolfToken').populate('vampireToken').exec(function(err, game) {
            if (err) {
                var err = ErrorService.databaseError();
                res.json(err.httpCode, {error: err.message});
            } else if (game) {
                res.json(200, game.properties());
            } else {
                var err = ErrorService.gameNotFound();
                res.json(err.httpCode, {error: err.message});
            }
        });
    },
    /**
     * Game move
     */
    move: function(req, res) {
        if (null != req.options.game.victory) {
            var err = ErrorService.gameDone();
            res.json(err.httpCode, {error: err.message});
        } else if (game.availableTeams().length > 0) {
            var err = ErrorService.missingTeam(game.availableTeams()[0]);
            res.json(err.httpCode, {error: err.message});
        } else {
            ParameterService.check(req, ['move'], {}, function(err, parameters) {
                if (err) {
                    res.json(err.httpCode, {error: err.message});
                } else {
                    MoveService.check(parameters.move, function(err, move) {
                        if (err) {
                            res.json(err.httpCode, {error: err.message});
                        } else {
                            req.options.game.executeMove(move, function(err) {
                                if (err) {
                                    res.json(err.httpCode, {error: err.message});
                                } else {
                                    req.options.game.nextTurn(function(err) {
                                        if (err) {
                                            res.json(err.httpCode, {error: err.message});
                                        } else {
                                            res.json(200, req.options.game.state);
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        }
    }
}
