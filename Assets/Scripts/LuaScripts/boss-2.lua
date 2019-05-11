Interval = 1.5;

local player
local lastAITime = 0
function start()
    player = scene.entity("Player")
    startCoroutine(bossIdle)
end

function bossIdle()
    while player.position.x <= 27 do
        coroutine.yield(waitForSeconds(Interval))
    end
    startCoroutine(bossAttack)
end

function bossAttack()
    while player.position.x > 27 do
        local skill = math.floor(math.random() * 4)
        entity.skill(skill, player)
        coroutine.yield(entity.wait("skill", _host))
        coroutine.yield(waitForSeconds(Interval))
    end
    startCoroutine(bossIdle)
end