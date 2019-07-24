StateReady = "ready";
StateBattle = "battle";
StateEnd = "end";
local levelState = StateReady;

function start()
    resources.loadAll("Boss/Boss-1/");
    resources.loadAll("Player/");
    resources.loadAll("Texture/UI/");
    resources.loadAll("Texture/Map/Scene-1/");
    resources.loadAll("Audio/Level-2")

    player = scene.entity("Player");
    boss = scene.entity("Boss-1");
    camera.follow(player);
    game.control(player);
    startCoroutine(level);
    game.ready();
end

function update(dt)
    if(levelState == StateEnd) then
        return
    elseif levelState == StateBattle then
        if(player.position.x > 27) then
            camera.follow({player, boss});
        else
            camera.follow(player);
        end
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

function level()
    repeat coroutine.yield(nil); 
    until player.position.x > 23;
    camera.follow(boss);
    coroutine.yield(waitForSeconds(3));
    boss.setActive(true);
    coroutine.yield(waitForSeconds(1.5));
    camera.follow(player);
    game.playAudio(resources.audio("Level-2/Level-2-BGM"), 0.25, 1, true);
    game.setTarget(boss, "Boss");
    levelState = StateBattle;
end

function playerDead()
    levelState = StateEnd;
    game.control(nil);
    game.setTarget(nil);
    game.exitAudio();
    --coroutine.yield(player.wait("action", _host));
    --coroutine.yield(waitForSeconds(1));
    game.over();
end

function bossDead()
    levelState = StateEnd;
    game.control(nil);
    game.setTarget(nil);
    game.exitAudio();
    coroutine.yield(boss.wait("action", _host));
    game.playAudio(resources.audio("Win"), 0.25);
    game.pass();
end