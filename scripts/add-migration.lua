local args = {...}
local migrationName = args[1]

if not migrationName then
    print("Uso: lua add-migration.lua NomeDaMigration")
    os.exit(1)
end

local command = string.format("dotnet ef migrations add %s --project ../src/Location404.Auth.Infrastructure --startup-project ../src/Location404.Auth.API", migrationName)
print("Executando:", command)

os.execute(command)
