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
            required: true,
        },
        vampireToken: {
            model: 'Token',
        },
        werewolfToken: {
            model: 'Token',
        },
        victory: {
            type: 'string',
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
        /**
         * Get available teams
         */
        availableTeams: function() {
            var teams = [];
            if (null == this.vampireToken) teams.push('vampire');
            if (null == this.werewolfToken) teams.push('werewolf');

            return teams;
        },
        /**
         * Get game properties
         */
        properties: function() {
            return {
                id: this.id,
                name: this.name,
                victory: this.victory,
                availableTeams: this.availableTeams(),
                turn: this.turn, 
            };
        },
        /**
         * Get game state
         */
        state: function() {
            var state = this.properties();
            state.width = this.width;
            state.height = this.height;

            state.tiles = MapService.twoDimmensionnal(this.tiles, ['humans', 'vampires', 'werewolfs']);

            return state;
        },
        /**
         * Generate game tiles from a map
         */
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
                if (err) return callback(ErrorService.databaseError());
                
                return callback();
            });
        },
        /**
         * Execute a move
         */
        executeMove: function(move, callback) {



        },
        /**
         * Check victory, switch turns
         */
        nextTurn: function(move, callback) {
            var tiles = MapService.twoDimmensionnal(this.tiles, ['humans', 'vampires', 'werewolfs']);

            var left = {vampires: 0, werewolfs: 0};
            for (x = 0; x < this.width; x++) {
                for (y = 0; y < this.height; y++) {
                    left.vampires += tiles[x][y].vampires;
                    left.werewolfs += tiles[x][y].werewolfs;
                }
            }

            if (vampires.left == 0 || werewolfs.left == 0) {
                if (vampires.left > 0) { 
                    this.victory = 'vampire';
                } else if (werewolfs.left > 0) {
                    this.victory = 'werewolf';
                } else {
                    this.victory = 'draw';
                }
            } else {
                if (this.turn == 'vampire') {
                    this.turn = 'werewolf';
                } else {
                    this.turn = 'vampire';
                }
            }

            this.save(function(err) {
                if (err) return callback(ErrorService.databaseError());
                
                return callback(null);
            });
        },
    },
};
