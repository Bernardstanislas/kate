/**
* Token.js
*
* @description :: TODO: You might write a short summary of how this model works and what it represents here.
* @docs        :: http://sailsjs.org/#!documentation/models
*/

module.exports = {
    attributes: {
        value: {
            type: 'string',
            unique: true,
        },
        game: {
            model: 'Game',
            required: true,
        },
    },
    beforeCreate: function(token, cb) {
        var bcrypt = require('bcrypt');

        bcrypt.genSalt(10, function(err, salt) {
            if (err) return cb(err);
            bcrypt.hash(token.value, salt, function(err, hash) {
                if (err) return cb(err);
                token.value = hash;
            });

            cb();
        });
    },
};
