/**
* Tile.js
*
* @description :: TODO: You might write a short summary of how this model works and what it represents here.
* @docs        :: http://sailsjs.org/#!documentation/models
*/

module.exports = {
    attributes: {
        game: {
            model: 'Game',
            required: true,
        },
        x: {
            type: 'integer',
            required: true,
        },
        y: {
            type: 'integer',
            required: true,
        },
        humans: {
            type: 'integer',
            defaultsTo: 0,
        },
        vampires: {
            type: 'integer',
            defaultsTo: 0,
        },
        werewolfs: {
            type: 'integer',
            defaultsTo: 0,
        },
    },
};

