local args = {...}
local target = args[1] or "0"

local command = string.format("dotnet ef database update %s --project ../src/Location404.Auth.Infrastructure --startup-project ../src/Location404.Auth.API", target)
print("Executando:", command)

os.execute(command)
