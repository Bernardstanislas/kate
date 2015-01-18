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
                    if (err) return callback(ErrorService.badDatabaseError());

                    return callback(null, value);
                });
            });
        });
    },
    /**
     * Check token
     *
     * callback(err, game)
     */
    check: function(game, token, callback) {
        var bcrypt = require('bcrypt');
        var authenticatedAs = null;

        var vampireTokenValue = ((null == game.vampireToken) ? '' : game.vampireToken.value);
        var werewolfTokenValue = ((null == game.werewolfToken) ? '' : game.werewolfToken.value);
        bcrypt.compare(token, vampireTokenValue, function (err, match) {
            if (err) return callback(ErrorService.databaseError());
            if (match) {
                if (game.turn == 'vampires') return callback(null, game);
                return callback(ErrorService.notYourTurn());
            } else {
                bcrypt.compare(token, werewolfTokenValue, function (err, match) {
                    if (err) return callback(ErrorService.databaseError());
                    if (match) {
                        if (game.turn == 'werewolfs') return callback(null, game);
                        return callback(ErrorService.notYourTurn());
                    } else {
                        return callback(ErrorService.invalidToken());
                    }
                });
            }
        });
    },
};
