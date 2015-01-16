/**
 * GameController
 *
 * @description :: Server-side logic for managing games
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {
    /**
     * Create a game
     */
    create: function(req, res) {
        ParameterService.check(req, ['name'], {}, function(err, parameters) {
            if (err) {
                res.json(err.httpCode, {error: err.message});
            } else {
                Game.findOneByName(parameters.name, function (err, game) {
                    if (err) {
                        var err = ErrorService.databaseError();
                        res.json(err.httpCode, {error: err.message});
                    } else if (game) {
                        var err = ErrorService.gameAlreadyExists();
                        res.json(err.httpCode, {error: err.message});
                    } else {
                        Game.create({name: name}).exec(function(err, game) {
                            if (err) {
                                var err = ErrorService.databaseError();
                                res.json(err.httpCode, {error: err.message});
                            } else {
                                res.ok();
                            }
                        });
                    }
                });
            }
        });
    },
    /**
     * Get all games
     */
    all: function(req, res) {
        Game.find().exec(function(err, games) {
            var err = ErrorService.databaseError();
            res.json(err.httpCode, {error: err.message});

            var gamesPublicData = [];
            games.foreach(function(game) {
                gamesPublicData.push(game.publicData);
            });

            res.json(200, gamesPublicData);
        });
    },
    /**
     * Register to a game
     */
    register: function(req, res) {
        ParameterService.check(req, ['name', 'alignement'], {alignement: ['vampire', 'werewolf']}, function(err, parameters) {
            if (err) {
                res.json(err.httpCode, {error: err.message});
            } else {
                Game.findOneByName(parameters.name).exec(function(err, game) {
                    if (err) {
                        var err = ErrorService.databaseError();
                        res.json(err.httpCode, {error: err.message});
                    } else {
                        if (null == game[parameters.alignement + 'Token']) {
                            TokenService.create(game, function(err, value) {
                                if (err) {
                                    res.json(err.httpCode, {error: err.message});
                                } else {
                                    res.json(200, {token: value});
                                }
                            });
                        } else {
                            var err = ErrorService.teamAlreadyExists(parameters.alignement);
                            res.json(err.httpCode, {error: err.message});
                        }
                    }
                });
            }
        });
    },
};

