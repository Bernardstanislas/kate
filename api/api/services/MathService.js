/**
 * MathService
 *
 * @description :: Math utilities
 */

module.exports = {
    /**
     * Roll dice
     *
     * diceConfiguration = {dice: <int>, faces: <int>}
     */
    rollDice: function(diceConfiguration) {
        var result = 0;
        for (die = 0; die < diceConfiguration.dice; die++) {
            result += MathService.randomInteger(1, diceConfiguration.faces);
        }

        return result;
    },
    /**
     * Get random integer
     */
    randomInteger: function(min, max) {
        return Math.floor((Math.random() * (max - min + 1)) + min);
    },
    /**
     * Generate distinct positions in the given rectangle
     */
    distinctPositions: function(width, height, count) {
        var positions = [];
        for (x = 0; x < width; x++) {
            for (y = 0; y < width; y++) {
                positions.push({x: x, y: y});
            }
        }

        return MathService.shuffle(positions).slice(0, count);
    },
    /**
     * Shuffle
     */
    shuffle: function(array) {
        for(var j, x, i = array.length; i; j = Math.floor(Math.random() * i), x = array[--i], array[i] = array[j], array[j] = x);
        
        return array;
    },
};
