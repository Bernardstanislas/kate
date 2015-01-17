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
            required: true,
        },
        vampireToken: {
            model: 'Token',
        },
        werewolfToken: {
            model: 'Token',
        },
        done: {
            type: 'boolean',
            defaultsTo: false,
        },
        width: {
            type: 'integer',
            required: true,
        },
        height: {
            type: 'integer',
            required: true,
        },
        tiles: {
            collection: "Tile",
            via: "game",
        },
        turn: {
            type: 'string',
            defaultsTo: 'vampire',
        },
        properties: function() {
            var availableTeams = [];
            if (null == this.vampire) availableTeams.push('vampire');
            if (null == this.werewolf) availableTeams.push('werewolf');
   
            return {
                name: this.name,
                availableTeams: availableTeams,
                done: this.done,
                turn: this.turn, 
            };
        },
        state: function() {
            var state = this.properties();
            state.width = this.width;
            state.height = this.height;

            state.tiles = MapService.twoDimmensionnal(this.tiles, ['humans', 'vampires', 'werewolfs']);

            console.log(state);

            return state;
        },
        generateTiles: function(map, callback) {
            var humans = MapService.twoDimmensionnal(map.humans, ['count']);
            var vampires = MapService.twoDimmensionnal(map.vampires, ['count']);
            var werewolfs = MapService.twoDimmensionnal(map.werewolfs, ['count']);

            for (x = 0; x < map.width; x++) {
                for (y = 0; y < map.height; y++) {
                    var tile = {x: x, y: y}
                    
                    if (humans.hasOwnProperty(x) && humans[x].hasOwnProperty(y)) tile.humans = humans[x][y].count;
                    if (vampires.hasOwnProperty(x) && vampires[x].hasOwnProperty(y)) tile.vampires = vampires[x][y].count;
                    if (werewolfs.hasOwnProperty(x) && werewolfs[x].hasOwnProperty(y)) tile.werewolfs = werewolfs[x][y].count;

                    this.tiles.add(tile);
                }
            }

            this.save(function(err) {
                if (err) {
                    return callback(ErrorService.databaseError());
                } else {
                    return callback();
                }
            });
        },
    },
};
