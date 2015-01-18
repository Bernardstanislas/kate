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
            if (!map.hasOwnProperty(expectedKey)) err = ErrorService.mapError('key ' + expectedKey + ' not found');
            return (null == err);
        });
        if (err) return callback(err);

        ['width', 'height'].every(function(expectedKey) {
            if (!MathService.isInteger(map[expectedKey])) err = ErrorService.mapError(
                expectedKey + ' is not an integer'
            );
            return (null == err);
        });
        if (err) return callback(err);

        ['vampires', 'werewolfs', 'humans'].every(function(expectedKey) {
            if (map[expectedKey].constructor !== Array ) {
                err = ErrorService.mapError(expectedKey + ' is not an array');
            } else {
                map[expectedKey].every(function(position) {
                    ['x', 'y', 'count'].every(function(expectedSubKey) {
                        if (!position.hasOwnProperty(expectedSubKey)) {
                            err = ErrorService.mapError(
                                expectedSubKey + ' not found in one of ' + expectedKey + ' positions'
                            );
                        } else if (!MathService.isInteger(position[expectedSubKey])) { 
                            err = ErrorService.mapError(
                                expectedSubKey + ' is not an integer in one of ' + expectedKey + ' positions'
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

        ['width', 'height'].every(function(key) {
            if (map[key] == 0) err = ErrorService.mapError(key + ' cannot be 0');
        });

        ['humans', 'vampires', 'werewolfs'].every(function(key) {
            map[key].every(function(position) {
                if (
                    position.x < 0 || position.x > map.width || 
                    position.y < 0 || position.y > map.height 
                ) {
                    err = ErrorService.mapError(
                        'position overflow: [' + position.x + ', ' + position.y  + '] in one of ' + expectedKey + ' positions'
                    );
                } else {
                    ['vampires', 'werewolfs'].every(function(comparedKey) {
                        if (map[key] != map[comparedKey]) {
                            map[comparedKey].every(function(comparedPosition) {
                                if (module.exports.distance(position, comparedPosition) == 0) err = ErrorService.mapError(
                                    'duplicated position [' + position.x + ', ' + position.y  + '] between ' + key + ' and ' + comparedKey
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
        if (Math.pow(RuleService.map.size.dice, 2) < RuleService.map.humans.tiles.dice * RuleService.map.humans.tiles.faces + 2) {
            throw new Error(
                'The number of occupied tiles can be larger than the total number of tiles. Please correct the settings.'
            );
        }
        var map = {width: MathService.rollDice(RuleService.map.size), height: MathService.rollDice(RuleService.map.size)};
        var positions = [];
        var count = Math.round(MathService.rollDice(RuleService.map.humans.tiles) / 2) + 1;
        if (map.width >= map.height) {
            positions = MathService.distinctPositions(Math.floor(map.width / 2), map.height, count);
        } else {
            positions = MathService.distinctPositions(map.width, Math.floor(map.height / 2), count);
        }
        var creaturesPosition = positions[0];
        creaturesPosition.count = MathService.rollDice(RuleService.map.creatures);
        positions.splice(0, 1);
        positions.forEach(function(position) {
            position.count = MathService.rollDice(RuleService.map.humans.numberByTile);
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
    twoDimmensionnal: function(positions, keys, keepNull) {
        var twoDimmensionnalCounts = {};

        positions.forEach(function(position) {
            var data = {};
            keys.forEach(function(key) {
                if (keepNull || position[key] != 0) data[key] = position[key];
            });

            if (Object.keys(data).length != 0) { 
                if (!twoDimmensionnalCounts.hasOwnProperty(position.x)) twoDimmensionnalCounts[position.x] = {};
                twoDimmensionnalCounts[position.x][position.y] = data;
            }
        });

        return twoDimmensionnalCounts;
    },
    /**
     * Get distance between two locations
     */
    distance: function(location1, location2) {
        return Math.max(Math.abs(location1.x - location2.x), Math.abs(location1.y - location2.y));
    },
};
