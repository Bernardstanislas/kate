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
        ['width', 'height', 'vampires', 'werewolfs', 'humans'].forEach(function(expectedKey) {
            if (!map.hasOwnProperty(expectedKey)) return callback(ErrorService.mapError('Key ' + expectedKey + ' not found'));
        });

        ['width', 'height'].forEach(function(expectedKey) {
            if (map[expectedKey] != parseInt(map[expectedKey], 10)) return callback(ErrorService.mapError(
                'Value of key ' + expectedKey + ' is not an integer'
            ));
        });

        ['vampires', 'werewolfs', 'humans'].forEach(function(expectedKey) {
            map[expectedKey].forEach(function(position) {
                ['x', 'y', 'count'].forEach(function(expectedSubKey) {
                    if (!position.hasOwnProperty(expectedSubKey)) return callback(ErrorService.mapError(
                        'Key ' + expectedSubKey + ' not found in one of ' + expectedKey + ' positions'
                    ));

                    if (position[expectedSubKey] != parseInt(position[expectedSubKey], 10)) return callback(ErrorService.mapError(
                        'Value of key ' + expectedSubKey + ' is not an integer in one of ' + expectedKey + ' positions'
                    ));
                });
            });
        });

        ['humans', 'vampires', 'werewolfs'].forEach(function(key) {
            map[key].forEach(function(position) {
                if (
                    position.x < 0 || position.x > map.width || 
                    position.y < 0 || position.y > map.height 
                ) return callback(ErrorService.mapError(
                    'Position overflow: [' + position.x + ', ' + position.y  + '] in one of ' + expectedKey + ' positions'
                ));

                ['vampires', 'werewolfs'].forEach(function(comparedKey) {
                    if (map[key] != map[comparedKey]) {
                        map[comparedKey].forEach(function(comparedPosition) {
                            if (position.x == comparedPosition.x && position.y == comparedPosition.y) return callback(ErrorService.mapError(
                                'Duplicated position [' + position.x + ', ' + position.y  + '] between ' + key + ' and ' + comparedKey
                            ));
                        });
                    }
                });
            });
        });

        return callback();
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
            if (!twoDimmensionnalCounts.hasOwnProperty(position.x)) twoDimmensionnalCounts[position.x] = {};
            var data = {};
            keys.forEach(function(key) {
                data[key] = position[key];
            });

            twoDimmensionnalCounts[position.x][position.y] = data;
        });

        return twoDimmensionnalCounts;
    } 
};
