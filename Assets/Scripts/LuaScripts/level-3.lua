
function start()
    boss = scene.entity("Boss-3");
    player = scene.entity("Player");
    camera.follow({player, boss});
    game.control(player);
end

function update(dt)

end

