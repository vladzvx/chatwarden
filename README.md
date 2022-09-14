# chatwarden

Бот-модератор чатов для телеграма.  
В качестве БД, хранилища состояний и персистентной очереди используется СУБД Tarantool (кластер Cartridge, crud).  
  
Для запуска проекта требуется .env файл следующего содержания:  
  
INTERNAL_PORT = 80  
EXTERNAL_PORT = 5000  
TOKEN = 1234:dswdwdwdwdwdwdwd  
STATE_PASSWORD = 123456  
  
TARANTOOL_USER=root  
TARANTOOL_PWD=super_pwd  
TARANTOOL_HOST=tarantool  
TARANTOOL_INTERNAL_PORT=3301  
TARANTOOL_EXTERNAL_PORT=3301  
