local command = "dotnet ef database update --project ../src/UserIdentityService.Infrastructure --startup-project ../src/UserIdentityService.API"
print("Executando:", command)

os.execute(command)