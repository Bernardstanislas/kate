/**
 * MapService
 *
 * @description :: Map utilities
 */

module.exports = {
    /**
     * Parse map
     */
    parseMap: function(map, callback) {
        var err = null;

        ['width', 'height', 'vampires', 'werewolfs', 'humans'].every(function(expectedKey) {
            if (!map.hasOwnProperty(expectedKey)) err = ErrorService.mapError('Key ' + expectedKey + ' not found');
            return (null == err);
        });

        ['width', 'height'].every(function(expectedKey) {
            if (map[expectedKey] != parseInt(map[expectedKey], 10)) err = ErrorService.mapError(
                'Value of key ' + expectedKey + ' is not an integer'
            );
            return (null == err);
        });

        ['vampires', 'werewolfs', 'humans'].every(function(expectedKey) {
            map[expectedKey].every(function(position) {
                ['x', 'y', 'count'].every(function(expectedSubKey) {
                    if (!position.hasOwnProperty(expectedSubKey)) {
                        err = ErrorService.mapError(
                            'Key ' + expectedSubKey + ' not found in one of ' + expectedKey + ' positions'
                        );
                    } else if (position[expectedSubKey] != parseInt(position[expectedSubKey], 10)) { 
                        err = ErrorService.mapError(
                            'Value of key ' + expectedSubKey + ' is not an integer in one of ' + expectedKey + ' positions'
                        );
                    }
                    return (null == err);
                });
                return (null == err);
            });
            return (null == err);
        });

        ['humans', 'vampires', 'werewolfs'].every(function(key) {
            map[key].every(function(position) {
                if (
                    position.x < 0 || position.x > map.width || 
                    position.y < 0 || position.y > map.height 
                ) {
                    err = ErrorService.mapError(
                        'Position overflow: [' + position.x + ', ' + position.y  + '] in one of ' + expectedKey + ' positions'
                    );
                } else {
                    ['vampires', 'werewolfs'].every(function(comparedKey) {
                        if (map[key] != map[comparedKey]) {
                            map[comparedKey].every(function(comparedPosition) {
                                if (position.x == comparedPosition.x && position.y == comparedPosition.y) err = ErrorService.mapError(
                                    'Duplicated position [' + position.x + ', ' + position.y  + '] between ' + key + ' and ' + comparedKey
                                );
                                return (null == err);
                            });
                        }
                        return (null == err);
                    });
                }
                return (null == err);
            });
            return (null == err);
        });

        return callback(err);
    },
    /**
     * Generate a random map
     */
    generateRandom: function() {
        var constants = {
            humans: {
                numberByTile: {
                    dice: 2,
                    faces: 2,
                },
                tiles: {
                    dice: 4,
                    faces: 6,
                },
            },
            creatures: {
                dice: 4,
                faces: 2,
            },
            size: {
                dice: 10,
                faces: 5,
            }
        }

        if (Math.pow(constants.size.dice, 2) < constants.humans.tiles.dice * constants.humans.tiles.faces + 2) {
            throw new Error(
                'The number of occupied tiles can be larger than the total number of tiles. Please correct the settings.'
            );
        }
        var map = {width: MathService.rollDice(constants.size), height: MathService.rollDice(constants.size)};
        var positions = [];
        var count = Math.round(MathService.rollDice(constants.humans.tiles) / 2) + 1;
        if (map.width >= map.height) {
            positions = MathService.distinctPositions(Math.floor(map.width / 2), map.height, count);
        } else {
            positions = MathService.distinctPositions(map.width, Math.floor(map.height / 2), count);
        }
        var creaturesPosition = positions[0];
        creaturesPosition.count = MathService.rollDice(constants.creatures);
        positions.splice(0, 1);
        positions.forEach(function(position) {
            position.count = MathService.rollDice(constants.humans.numberByTile);
        });
        map.vampires = [creaturesPosition];
        map.werewolfs = [{x: map.width - creaturesPosition.x - 1, y: map.height - creaturesPosition.y - 1, count: creaturesPosition.count}];
        map.humans = positions;
        positions.forEach(function(position) {
            map.humans.push({x: map.width - position.x - 1, y: map.height - position.y - 1, count: position.count});
        });

        return map;
    },
    /**
     * Two dimensionnal position array
     */
    twoDimmensionnal: function(positions, keys) {
        var twoDimmensionnalCounts = {};

        positions.forEach(function(position) {
            var data = {};
            keys.forEach(function(key) {
                if (position[key] != 0) data[key] = position[key];
            });

            if (Object.keys(data).length != 0) { 
                if (!twoDimmensionnalCounts.hasOwnProperty(position.x)) twoDimmensionnalCounts[position.x] = {};
                twoDimmensionnalCounts[position.x][position.y] = data;
            }
        });

        return twoDimmensionnalCounts;
    } 
};
