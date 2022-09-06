local cartridge = require('cartridge')
local crud = require('crud')
local role_name = 'cw'


local function init(opts)
    box.cfg{
        memtx_memory= 256 * 1024 * 1024
    } 
    if opts.is_master then

        local users_space = box.schema.space.create('users', {
                format = {
                    {name = 'user_id', type = 'integer'},--1
                    {name = 'bot_id', type = 'integer'},--2
                    {name = 'chat_id', type = 'integer'},--3
                    {name = 'status', type = 'string', is_nullable=true},--4
                    {name = 'bucket_id', type = 'unsigned'},--5
                },
                if_not_exists = true,
            });

        users_space:create_index('users_ids', {type = 'TREE',
            parts = {{field ='user_id', is_nullable = false}, {field ='bot_id', is_nullable = false}, 
            {field ='chat_id', is_nullable = false}},
            if_not_exists = true});

        users_space:create_index('bucket_id', {
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

--function set_status(user_id,bot_id,chat_id,status)
 --   crud.replace_object("users",{user_id = user_id,bot_id = bot_id,chat_id = chat_id, status = status})
--end

function set_status(user_id,bot_id,chat_id,status)
    crud.upsert_object("users",{user_id = user_id,bot_id = bot_id,chat_id = chat_id,status = status},{{'=', 'status', status}})
end

function get_status(user_id,bot_id,chat_id)
    local res =  crud.select("users",{{'==', 'users_ids', {user_id,bot_id,chat_id}}}).rows
    if  table.maxn(res) == 0 then
        return "\0"
    else 
        return res[1][4]
    end
end

return {
    init = init,
    stop = stop,
    role_name = role_name,

    test = test,
    set_status = set_status,
    get_status = get_status,
}