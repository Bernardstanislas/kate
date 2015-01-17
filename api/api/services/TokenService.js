/**
 * TokenService
 *
 * @description :: Server-side utility for managing tokens
 */

module.exports = {
    /**
     * Create a token
     *
     * callback(err, value)
     */
    create: function(game, team, callback) {
        if (null !=  game[team + 'Token']) return callback(ErrorService.teamAlreadyExists(team));

        require('crypto').randomBytes(48, function(ex, buf) {
            var value = buf.toString('hex');
            Token.create({value: value, game: game.id}).exec(function(err, token) {
                if (err) return callback(ErrorService.databaseError());

                var updatedData = {};
                updatedData[team + 'Token'] = token.id;
                Game.update(game.id, updatedData).exec(function(err, game) {
                    if (err) return callback(ErrorService.databaseError());

                    return callback(null, value);
                });
            });
        });
    },
    /**
     * Check token authentication
     *
     * callback(err, game)
     */
    check: function(game, token, callback) {
        var bcrypt = require('bcrypt');
        var authenticatedAs = null;

        bcrypt.compare(token, game.vampireToken.value, function (err, match) {
            if (err) return callback(ErrorService.databaseError());
            if (match) {
                if (game.turn == 'vampire') return callback(null, game);
                return callback(ErrorService.notYourTurn());
            } else {
                bcrypt.compare(token, game.werewolfToken.value, function (err, match) {
                    if (err) return callback(ErrorService.databaseError());
                    if (match) {
                        if (game.turn == 'werewolf') return callback(null, game);
                        return callback(ErrorService.notYourTurn());
                    } else {
                        return callback(ErrorService.invalidToken());
                    }
                });
            }
        });
    },
};
