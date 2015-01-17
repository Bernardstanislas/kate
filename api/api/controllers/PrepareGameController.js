/**
 * PrepareGameController
 *
 * @description :: Server-side logic for managing game preparation
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
                        var map;
                        if (parameters.map) {
                            map = parameters.map;
                        } else {
                            map = MapService.generateRandom();
                        }
                        MapService.parseMap(map, function(err) {
                            if (err) {
                                res.json(err.httpCode, {error: err.message});
                            } else {
                                Game.create({
                                    name: parameters.name, 
                                    width: map.width, 
                                    height: map.height
                                }).exec(function(err, game) {
                                    if (err) {
                                        var err = ErrorService.databaseError();
                                        res.json(err.httpCode, {error: err.message});
                                    } else {
                                        game.generateTiles(map, function(err) {
                                            if (err) {
                                                res.json(err.httpCode, {error: err.message});
                                            } else {
                                                res.ok();   
                                            }
                                        });
                                    }
                                });
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
            if (err) {
                var err = ErrorService.databaseError();
                res.json(err.httpCode, {error: err.message});
            } else {
                var gamesProperties = [];
                games.forEach(function(game) {
                    gamesPublicData.push(game.properties());
                });

                res.json(200, gamesProperties);
            }
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
                    } else if (game) {
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
                    } else {
                        var err = ErrorService.gameNotFound();
                        res.json(err.httpCode, {error: err.message});
                    }
                });
            }
        });
    },
};
