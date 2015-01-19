/**
 * MoveService
 *
 * @description :: Game move utilities
 */

module.exports = {
    /**
     * Check move parameters
     */
    check: function(move, callback) {
        var err = null;
        ['type', 'actions'].every(function(expectedKey) {
            if (!move.hasOwnProperty(expectedKey)) err = ErrorService.moveError(expectedKey + ' not found');
            return (null == err);
        });
        if (err) return callback(err);

        if (RuleService.actions.allowed.indexOf(move.type) == -1) {
            return callback(ErrorService.moveError(
                'bad type value: ' + move.type + '. Allowed values: ' + RuleService.actions.allowed.join(', ')
            ));
        }

        if (move.actions.constructor !== Array ) return callback(ErrorService.moveError('actions is not an array'));
        module.exports.checkActionsAmount(move, function(localErr) {
            err = localErr;
        });
        if (err) return callback(err);
        if (move.type == 'move') {
            move.actions.every(function(moveAction) {
                ['origin', 'target', 'count'].every(function(expectedKey) {
                    if (!moveAction.hasOwnProperty(expectedKey)) {
                        err = ErrorService.moveError(expectedKey + ' not found in one of the actions');
                    }
                    return (null == err);
                });
                if (err) return false;

                if (!MathService.isInteger(moveAction.count)) {
                    err = ErrorService.moveError('count is not an integer in one of the actions');
                } else {
                    ['origin', 'target'].every(function(key) {
                        ['x', 'y'].every(function(subKey) {
                            if (!moveAction[key].hasOwnProperty(subKey)) {
                                err = ErrorService.moveError(key + '.' + subKey + ' not found in one of the actions');  
                            } else if (!MathService.isInteger(moveAction[key][subKey])) {
                                err = ErrorService.moveError(
                                    key + '.' + subkey + ' is not an integer in one of the actions'
                                );
                            }
                            return (null == err);
                        });
                        return (null == err);
                    });
                }
                return (null == err);
            });
            if (err) return callback(err);

            var origins = [];
            var targets = [];
            move.actions.every(function(moveAction) {
                if (moveAction.count == 0) {
                    err = ErrorService.moveError('count is null in one of the actions'); 
                } else if (MapService.distance(moveAction.origin, moveAction.target) == 0) {
                    err = ErrorService.moveError('identical origin and target in one of the actions');
                } else {
                    targets.every(function(target) {
                        if (MapService.distance(moveAction.origin, target) == 0) err = ErrorService.moveError(
                            'tile [' + target.x + ', ' + target.y + '] is used as both a target and a location'
                        );
                        return (null == err);
                    });
                    origins.every(function(origin) {
                        if (MapService.distance(moveAction.target, origin) == 0) err = ErrorService.moveError(
                            'tile [' + target.x + ', ' + target.y + '] is used as both a target and a location'
                        );
                        return (null == err);
                    });

                    if (null == err) {
                        origins.push(moveAction.origin);
                        targets.push(moveAction.target);
                    }
                }
                return (null == err);
            });

            return callback(err);
        } else if (move.type == 'attack') {
            move.actions.every(function(attackAction) {
                ['x', 'y'].every(function(key) {
                    if (!attackAction.hasOwnProperty(key)) {
                        err = ErrorService.moveError(key + ' not found in one of the actions');
                    } else if (!MathService.isInteger(attackAction[key])) {
                        err = ErrorService.moveError(key + ' is not an integer in one of the actions');
                    }
                    return (null == err);
                });
                return (null == err);
            });
            return callback(err);
        } else {
            return callback(null);
        }

        return callback(ErrorService.moveError('parser error'));
    },
    /**
     * Check if a move has the correct amount of actions
     */
    checkActionsAmount: function(move, callback) {
        var length = move.actions.length;
        var max = RuleService.actions.number[move.type].max;
        var min = RuleService.actions.number[move.type].min;

        if (length > max) {
            return callback(ErrorService.moveError('more than ' + max + ' actions found for a ' + move.type));
        } else if (length < min) {
            return callback(ErrorService.moveError('less than ' + min + ' actions found for a ' + move.type));
        }
        
        return callback(null)
    },
    /**
     * Check if a location is valid given the tiles and expected controllers
     */
    checkWithMap: function(location, width, height, twoDimmensionnalTiles, expectedControllers, callback) {
        if (location.x >= width) return callback(
            ErrorService.moveError('x overflow for location [' + location.x + ', ' + location.y + ']')
        );
        if (location.y >= height) return callback(
            ErrorService.moveError('y overflow for location [' + location.x + ', ' + location.y + ']')
        );

        var tile = twoDimmensionnalTiles[location.x][location.y];
        var controllers = [];
        ['humans', 'vampires', 'werewolfs'].forEach(function(key) {
            if (tile[key] > 0) controllers.push(key);
        });
        if (controllers.length > 1) {
            return callback(ErrorService.integrityError(location.x, location.y));
        } else if (controllers.length == 0) {
            controllers.push('');
        }

        if (expectedControllers.indexOf(controllers[0]) == -1) {
            return callback(ErrorService.moveError(
                'expected ' + expectedControllers.join(', ') + ' on tile [' + location.x + ', ' + location.y + '], but found ' + 
                (controllers[0] == '' ? 'nobody' : controllers[0])
            ));
        }

        return callback(null);
    },
    /**
     * Fight
     */
    fight: function(tile, turn, otherTurn) { 
        if (tile.humans != 0) {
            if (tile[turn] >= RuleService.battle.ratios.humans *tile.humans) {
                tile[turn] += tile.humans; 
                tile.humans = 0;
            } else {
                randomBattle(tile, turn, 'humans');
            }
        }

        if (tile[otherTurn] != 0) {
            if (tile[turn] >= RuleService.battle.ratios.creatures * tile[otherTurn]) {
                tile[otherTurn] = 0; 
            } else {
                randomBattle(tile, turn, otherTurn);
            }
        }
    },
    /**
     * Random battle
     */
    randomBattle: function(tile, attackers, defenders) {
        var winner = ((Math.random() < (tile[attackers] / (tile[attackers] + tile[defenders]))) ? attackers : defenders);
        
        if (winner == attackers) {
            var attackersLeft = 0;
            var key = ((defenders == 'humans') ? 'humans' : 'creatues');
            var attackersRatio = battle.random.attackers[key];
            var defendersRatio = battle.random.defenders[key];
            for (var i; i < tile[attackers]; i++) {
                if (Math.random() < (attackersRatio * tile[attackers] / (defendersRatio * tile[defenders]))) attackersLeft += 1;
            }
            tile[attackers] = attackersLeft + ((defenders == 'humans') ? tile[defenders] : 0);
            tile[defenders] = 0;
        } else {
            tile[attackers] = 0; 
        }
    },
};
