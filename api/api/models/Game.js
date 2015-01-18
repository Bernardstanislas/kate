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
        },
        height: {
            type: 'integer',
        },
        tiles: {
            collection: "Tile",
            via: "game",
        },
        turn: {
            type: 'string',
            defaultsTo: 'vampires',
        },
        /**
         * Get other turn
         */
        otherTurn: function() {
            if (this.turn == 'vampires') {
                return 'werewolfs';
            } else {
                return 'vampires';
            }
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
        generateTiles: function(map) {
            this.width = map.width;
            this.height = map.height;

            var humans = MapService.twoDimmensionnal(map.humans, ['count']);
            var vampires = MapService.twoDimmensionnal(map.vampires, ['count']);
            var werewolfs = MapService.twoDimmensionnal(map.werewolfs, ['count']);

            for (var x = 0; x < map.width; x++) {
                for (var y = 0; y < map.height; y++) {
                    var tile = {x: x, y: y}
                    if (humans.hasOwnProperty(x) && humans[x].hasOwnProperty(y)) tile.humans = humans[x][y].count;
                    if (vampires.hasOwnProperty(x) && vampires[x].hasOwnProperty(y)) tile.vampires = vampires[x][y].count;
                    if (werewolfs.hasOwnProperty(x) && werewolfs[x].hasOwnProperty(y)) tile.werewolfs = werewolfs[x][y].count;

                    this.tiles.add(tile);
                }
            }
        },
        /**
         * Execute a move
         */
        executeMove: function(twoDimmensionnalTiles, move, callback) {
            if (move.type == 'pass') {
                return callback(null);
            } else {
                this.originTiles = {};
                this.targetTiles = {};
                var err = null;
                var game = this;

                if (move.type == 'move') {
                    move.actions.every(function(moveAction) {
                        game.executeMoveAction(twoDimmensionnalTiles, moveAction, function(moveErr) {
                            err = moveErr;
                        });
                        return (null == err);
                    });
                    if (err) return callback(err);
                } else {
                    move.actions.every(function(attackAction) {
                        game.executeAttackAction(twoDimmensionnalTiles, attackAction, function(attackErr) {
                            err = attackErr;
                        });
                        return (null == err);
                    });
                    if (err) return callback(err);
                }

                var tilesToUpdate = [];
                for (var x in this.originTiles) {
                    if (this.originTiles.hasOwnProperty(x)) {
                        for (var y in this.originTiles[x]) {
                            if (this.originTiles[x].hasOwnProperty(y)) {
                                var tileToUpdate = this.originTiles[x][y];
                                tileToUpdate.game = this.id;
                                tileToUpdate.x = x;
                                tileToUpdate.y = y;
                                tilesToUpdate.push(tileToUpdate);
                            }
                        }
                    }
                }
                for (var x in this.targetTiles) {
                    if (this.targetTiles.hasOwnProperty(x)) {
                        for (var y in this.targetTiles[x]) {
                            if (this.targetTiles[x].hasOwnProperty(y)) {
                                var tileToUpdate = this.targetTiles[x][y];

                                // @DEV
                                console.log('Before fight');
                                console.log(tileToUpdate);

                                MoveService.fight(tileToUpdate, game.turn, game.otherTurn());

                                // @DEV
                                console.log('After fight');
                                console.log(tileToUpdate);

                                tileToUpdate.game = this.id;
                                tileToUpdate.x = x;
                                tileToUpdate.y = y;
                                tilesToUpdate.push(tileToUpdate);
                            }
                        }
                    }
                }

                var game = this;
                this.tiles.forEach(function(tile) {
                    if (game.originTiles.hasOwnProperty(tile.x) && game.originTiles[tile.x].hasOwnProperty(tile.y)) {
                        tile.humans = game.originTiles[tile.x][tile.y].humans;
                        tile.vampires = game.originTiles[tile.x][tile.y].vampires;
                        tile.werewolfs = game.originTiles[tile.x][tile.y].werewolfs;
                    } else if (game.targetTiles.hasOwnProperty(tile.x) && game.targetTiles[tile.x].hasOwnProperty(tile.y)) {
                        tile.humans = game.targetTiles[tile.x][tile.y].humans;
                        tile.vampires = game.targetTiles[tile.x][tile.y].vampires;
                        tile.werewolfs = game.targetTiles[tile.x][tile.y].werewolfs;
                    }
                });

                var updateNextTile = function(tiles, index) {
                    var tile = tiles[index];

                    Tile.update(
                        {game: tile.game, x: tile.x, y: tile.y}, 
                        {humans: tile.humans, vampires: tile.vampires, werewolfs: tile.werewolfs}
                    ).exec(function(err) {
                        if (err) return callback(ErrorService.badDatabaseError());
                        if (index < tiles.length - 1) updateNextTile(tiles, index + 1);

                        return callback(null);
                    });
                }
                updateNextTile(tilesToUpdate, 0);
            }
        },
        /**
         * Execute a move action
         */
        executeMoveAction: function(twoDimmensionnalTiles, moveAction, callback) {
            var game = this;

            MoveService.checkWithMap(moveAction.origin, this.width, this.height, twoDimmensionnalTiles, [game.turn], function(err) {
                if (err) return callback(err);

                MoveService.checkWithMap(moveAction.target, game.width, game.height, twoDimmensionnalTiles, [game.turn, game.otherTurn(), 'humans', ''], function(err) {
                    if (err) return callback(err);

                    if (MapService.distance(moveAction.origin, moveAction.target) > RuleService.actions.move.distance) {
                        return callback(ErrorService.moveError('distance too large between origin and target'));
                    }

                    if (!game.originTiles.hasOwnProperty(moveAction.origin.x)) game.originTiles[moveAction.origin.x] = {};
                    if (!game.originTiles[moveAction.origin.x].hasOwnProperty(moveAction.origin.y)) game.originTiles[moveAction.origin.x][moveAction.origin.y] = 
                        twoDimmensionnalTiles[moveAction.origin.x][moveAction.origin.y];

                    if (game.originTiles[moveAction.origin.x][moveAction.origin.y][game.turn] < moveAction.count) return callback(
                        ErrorService.moveError('number of ' + game.turn + ' on the tile is too small for one of the moves')
                    );

                    if (!game.targetTiles.hasOwnProperty(moveAction.target.x)) game.targetTiles[moveAction.target.x] = {};
                    if (!game.targetTiles[moveAction.target.x].hasOwnProperty(moveAction.target.y)) game.targetTiles[moveAction.target.x][moveAction.target.y] = 
                        twoDimmensionnalTiles[moveAction.target.x][moveAction.target.y];

                    game.originTiles[moveAction.origin.x][moveAction.origin.y][game.turn] -= moveAction.count;
                    game.targetTiles[moveAction.target.x][moveAction.target.y][game.turn] += moveAction.count;

                    return callback(null);
                });
            });
        },
        /**
         * Execute an attack action
         */
        executeAttackAction: function(twoDimmensionnalTiles, attackAction, callback) {
            MoveService.checkWithMap(attackAction, this.width, this.height, twoDimmensionnalTiles, ['humans', this.otherTurn()], function(err) {
                if (err) return callback(err);

                var attackers = 0;
                for (var x = 0; x < game.width; x++) {
                    for (var y = 0; y < game.height; y++) {
                        if (MapService.distance(attackAction, {x: x, y: y}) == RuleService.actions.attack.distance) {
                            if (twoDimmensionnalTiles[x][y][game.turn] > 0) {
                                if (!game.originTiles.hasOwnProperty(x)) game.originTiles[x] = {};
                                game.originTiles[x][y] = twoDimmensionnalTiles[x][y];
                                attackers += game.originTiles[x][y][game.turn];
                                game.originTiles[x][y][game.turn] = 0;
                            }
                        }
                    }
                }
                if (attackers == 0) return callback(ErrorService.moveError('no ' + game.turn + ' found around target tile'));

                game.targetTiles[attackAction.x] = {};
                game.targetTiles[attackAction.x][attackAction.y] = twoDimmensionnalTiles[attackAction.x][attackAction.y];
                game.targetTiles[attackAction.x][attackAction.y][game.turn] = attackers;

                return callback(null);
            });
        },
        /**
         * Check victory, switch turns
         */
        nextTurn: function(twoDimmensionnalTiles, callback) {
            var left = {vampires: 0, werewolfs: 0};
            for (var x = 0; x < this.width; x++) {
                for (var y = 0; y < this.height; y++) {
                    left.vampires += twoDimmensionnalTiles[x][y].vampires;
                    left.werewolfs += twoDimmensionnalTiles[x][y].werewolfs;
                }
            }

            if (left.vampires == 0 || left.werewolfs == 0) {
                if (left.vampires > 0) { 
                    this.victory = 'vampires';
                } else if (left.werewolfs > 0) {
                    this.victory = 'werewolfs';
                } else {
                    this.victory = 'draw';
                }
            } else {
                //this.turn = this.otherTurn();
            }

            Game.update({id: this.id}, {victory: this.victory, turn: this.turn}).exec(function(err) {
                if (err) return callback(ErrorService.badDatabaseError());

                return callback(null);
            });
        },
    },
};
