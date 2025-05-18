local args = {...}
local target = args[1] or "0"

local command = string.format("dotnet ef database update %s --project ../src/UserIdentity.Infra --startup-project ../src/UserIdentity.API", target)
print("Executando:", command)

os.execute(command)
