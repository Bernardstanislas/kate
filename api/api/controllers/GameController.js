/**
 * GameController
 *
 * @description :: Server-side logic for managing game
 * @help        :: See http://links.sailsjs.org/docs/controllers
 */

module.exports = {
    /**
     * Game state
     */
    state: function(req, res) {
        Game.findOneByName(req.param('name')).populate('tiles').exec(function(err, game) {
            if (err) {
                var err = ErrorService.databaseError();
                res.json(err.httpCode, {error: err.message});
            } else if (game) {
                res.json(200, game.state());
            } else {
                var err = ErrorService.gameNotFound();
                res.json(err.httpCode, {error: err.message});
            }
        });
    } 
}
