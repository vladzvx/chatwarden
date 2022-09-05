local cartridge = require('cartridge')
local crud = require('crud')
local role_name = 'cw'


local function init(opts)
    box.cfg{
        memtx_memory= 256 * 1024 * 1024
    } 
    if opts.is_master then

        local space = box.schema.space.create('profiles', {
                format = {
                    {name = 'id', type = 'string'},--1
                    {name = 'bucket_id', type = 'unsigned'},--22
                },
                if_not_exists = true,
            });

            space:create_index('id', {type = 'TREE',
                parts = { {field ='id', is_nullable = false} },
                if_not_exists = true,
            });

            space:create_index('bucket_id', {
                parts = {'bucket_id'}, unique = false,
                if_not_exists = true
            });
    end
end

local function stop()

end

function test()
    return "ok"
end


return {
    init = init,
    stop = stop,
    role_name = role_name,

    test = test,
}