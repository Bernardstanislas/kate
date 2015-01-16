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
    create: function(game, callback) {
        require('crypto').randomBytes(48, function(ex, buf) {
            var value = buf.toString('hex');

            Token.create({value: value, game: game}).exec(function(err, token) {
                if (err) return callback(ErrorService.databaseError());

                return callback(null, value);
            });
        });
    },
};
