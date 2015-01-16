/**
* Game.js
*
* @description :: TODO: You might write a short summary of how this model works and what it represents here.
* @docs        :: http://sailsjs.org/#!documentation/models
*/

module.exports = {
    attributes: {
        name: {
            type: 'string',
            unique: true,
        },
        vampireToken: {
            model: 'Token'
        },
        werewolfToken: {
            model: 'Token'
        },
        done: {
            type: 'boolean',
            defaultsTo: false,
        },
        publicData: function() {
            var availablePlayers = [];
            if (null == this.vampire) availablePlayers.push('vampire');
            if (null == this.werewolf) availablePlayers.push('werewolf');
   
            return {
                name: this.name,
                availablePlayers: availablePlayers,
                done: this.done,
            };
        },
    },
};
