/**
 * RuleService
 *
 * @description :: Rules container
 */

module.exports = {
    actions: {
        allowed: ['move', 'attack', 'pass'],
        number: {
            move: {
                min: 1,
                max: 3,
            },
            attack: {
                min: 1,
                max: 1,
            },
            pass: {
                min: 0,
                max: 0,
            },
        },
        move: {
            distance: 1,
        },
        attack: {
            distance: 1,
        },
    },
    battle: {
        ratios: {
            humans: 1,
            creatures: 1.5,
        },
        random: {
            attackers: {
                humans: 1,
                creatures: 2,
            },
            defenders: {
                humans: 1,
                creatures: 3,
            },
        },
    },
    map: {
        humans: {
            numberByTile: {
                dice: 2,
                faces: 2,
            },
            tiles: {
                dice: 4,
                faces: 6,
            },
        },
        creatures: {
            dice: 4,
            faces: 2,
        },
        size: {
            dice: 10,
            faces: 5,
        }
    }
};
