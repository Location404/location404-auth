local command = "dotnet ef database update --project ../src/UserIdentity.Infra --startup-project ../src/UserIdentity.API"
print("Executando:", command)

os.execute(command)