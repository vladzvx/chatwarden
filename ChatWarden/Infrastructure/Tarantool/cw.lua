local cartridge = require('cartridge')
local crud = require('crud')
local uuid = require('uuid')
local role_name = 'cw'


local function init(opts)
    box.cfg{
        memtx_memory= 256 * 1024 * 1024
    } 
    if opts.is_master then
        local bot_space = box.schema.space.create('bot_profiles', {
                format = {
                    {name = 'bot_id', type = 'integer'},--1
                    {name = 'chat_id', type = 'integer'},--2

                    {name = 'state', type = 'string'},--3
                    {name = 'help_text', type = 'string'},--4

                    {name = 'ban_replics', type = 'array'},--5
                    {name = 'media_replics', type = 'array'},--6
                    {name = 'restrict_replics', type = 'array'},--7

                    {name = 'bucket_id', type = 'unsigned'},--8
                },
                if_not_exists = true,
            });

        bot_space:create_index('id', {type = 'TREE',
            parts = { 
                {field ='bot_id', is_nullable = false},
                {field ='chat_id', is_nullable = false}
            },
            if_not_exists = true});

        bot_space:create_index('bucket_id', {
            parts = {'bucket_id'}, unique = false,
            if_not_exists = true
            });

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

        local messages = box.schema.space.create('messages', {
                format = {
                    {name = 'id', type = 'uuid'},--1
                    {name = 'user_id', type = 'integer'},--2
                    {name = 'chat_id', type = 'integer'},--3
                    {name = 'message_number', type = 'integer'},--4
                    {name = 'time', type = 'integer'},--5
                    {name = 'bucket_id', type = 'unsigned'},--6
                },
                if_not_exists = true,
            });
        messages:create_index('id', {
            parts = {'id'}, unique = true,
            if_not_exists = true});

        messages:create_index('ids', {type = 'TREE',
            parts = {
                {field ='user_id', is_nullable = false}, 
                {field ='chat_id', is_nullable = false}, 
                {field ='message_number', is_nullable = false}
            }, if_not_exists = true,unique = false});

        messages:create_index('bucket_id', {
            parts = {'bucket_id'}, unique = false,
            if_not_exists = true });
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

function add_chat(bot_id,chat_id,state,help_text)
    crud.insert_object("bot_profiles",{bot_id = bot_id,chat_id = chat_id,state = state,help_text = help_text
    ,ban_replics = {},media_replics = {},restrict_replics = {}})
end

function add_to_array(bot_id,chat_id,number,data)
    local tmp = get_field(bot_id,chat_id,{},number)
    table.insert(tmp,data)
    crud.update("bot_profiles",{bot_id,chat_id},{{'=', number, tmp}})
end

function add_ban_replic(bot_id,chat_id,data)
    add_to_array(bot_id,chat_id,5,data)
end

function add_media_replic(bot_id,chat_id,data)
    add_to_array(bot_id,chat_id,6,data)
end

function add_restrict_replic(bot_id,chat_id,data)
    add_to_array(bot_id,chat_id,7,data)
end

function get_ban_replics(bot_id,chat_id)
    return get_field(bot_id,chat_id,{},5)
end

function get_media_replics(bot_id,chat_id)
    return get_field(bot_id,chat_id,{},6)
end

function get_restrict_replics(bot_id,chat_id)
    return get_field(bot_id,chat_id,{},7)
end

function get_state (bot_id,chat_id)
    return get_field(bot_id,chat_id,"\0",3)
end

function set_state (bot_id,chat_id,state)
    crud.update("bot_profiles",{bot_id,chat_id},{{'=', 'state', state}})
end

function set_help (bot_id,chat_id,help_text)
    crud.update("bot_profiles",{bot_id,chat_id},{{'=', 'help_text', help_text}})
end

function get_help (bot_id,chat_id)
    return get_field(bot_id,chat_id,"",4)
end

function get_field(bot_id,chat_id,defa, numb)
    local res = crud.select("bot_profiles",{{'==', 'id', {bot_id,chat_id}}}).rows
    if  table.maxn(res) == 0 then
        return defa
    else 
        return res[1][numb]
    end
end

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

function add_message(user_id,chat_id,message_number,time)
    crud.insert_object("messages",{id = uuid.new(),user_id = user_id,chat_id = chat_id,message_number = message_number, time = time})
end

function get_messages(user_id,chat_id)
    local res = {}
    local tmp = crud.select("messages",{{'==', 'ids', {user_id,chat_id}}},{fields={"message_number"}})
    if tmp~=nil then
        local count = table.maxn(tmp.rows)
        if count>0 then
            for i=1,count,1 do
                table.insert(res,tmp.rows[i][1])
            end
        end
    end
    return res
end

return {
    init = init,
    stop = stop,
    role_name = role_name,

    test = test,
    set_status = set_status,
    get_status = get_status,

    add_message = add_message,
    get_messages = get_messages,
    set_help = set_help,
    add_ban_replic = add_ban_replic,
}