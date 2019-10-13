local player;
local boss;
local gameEnd = false;

function start()
    resources.loadAll("Boss/Boss-0/");
    resources.loadAll("Player/");
    resources.loadAll("Texture/UI/");
    resources.loadAll("Fonts/")
    resources.loadAll("Texture/Map/Scene-0");

    local prefab = resources.prefab("Player");
    player = scene.spawn(prefab, "Player", vec2(5, 2.2));
    boss = scene.spawn(resources.prefab("Boss-0"), "Boss", vec2(180, 3));
    --boss.setActive(true);
    camera.reset();
    camera.follow(player);

    boss.setActive(false);
    startCoroutine(tutorial);
    game.control(player);

    game.ready();

    -- Handle object pick up event
    player.on("PickUp", function()
        startCoroutine(function()
            console.log("pick up!")
            coroutine.yield(game.conversation(
                "${conv:turtorial-inventory}",
                {player},
                true
            ))
            coroutine.yield(waitForSeconds(.5))
            game.tips("${inventory-hint}", 3);
        end)
    end)
end

function update()
    if gameEnd then
        return;
    end
    if player.HP <= 0 then
        gameEnd = true;
        startCoroutine(playerDead);
        return;
    end
    if boss and boss.HP <= 0 then
        gameEnd = true;
        startCoroutine(bossDead);
        return;
    end
end

function playerDead()
    game.control(nil);
    game.setTarget(nil);
    game.exitAudio();
    --coroutine.yield(player.wait("action", _host));
    --coroutine.yield(waitForSeconds(1));
    game.over();
end

function bossDead()
    game.profile.completeTutorial = true;
    game.save();
    game.control(nil);
    game.setTarget(nil);
    game.exitAudio();
    coroutine.yield(boss.wait("action", _host));
    game.playAudio(resources.audio("Win"), 0.25);
    game.pass();
end

function tutorial()
    coroutine.yield(waitForSeconds(0.7));

    -- Movement hint
    if(not game.profile.completeTutorial) then
        game.tips("${movement-hint}", 3);
    end
    repeat coroutine.yield(nil); 
    until player.position.x > 25;
    coroutine.yield(game.conversation({
        "${conv:turtorial-1}"
    },{player}, true));

    -- Jump hint
    repeat coroutine.yield(nil); 
    until player.position.x > 43;
    if(not game.profile.completeTutorial) then
        game.tips("${jump-hint}", 2);
    end

    repeat coroutine.yield(nil);
    until player.position.x > 70;
    coroutine.yield(game.conversation({
        "${conv:turtorial-2}"
    },{player}, true));

    repeat coroutine.yield(nil);
    until player.position.x > 134;
    
    boss.setActive(true);

    camera.follow(boss);
    coroutine.yield(waitForSeconds(4));
    game.setTarget(boss, "Boss");
    game.playAudio(resources.audio("Boss"), 0.25, 1, true);
    camera.follow(player);
    game.tips("${battle-hint}", 5);

end
